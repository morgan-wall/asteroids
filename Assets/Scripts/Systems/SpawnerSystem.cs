using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Collections;

[UpdateAfter(typeof(SpawnPointSystem))]
public class SpawnerSystem : SystemBase
{
    EntityCommandBufferSystem m_entityCommandBufferSystem = null;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        var commandBuffer = m_entityCommandBufferSystem.CreateCommandBuffer();
        var randomGenPerThread = World.GetExistingSystem<RandomGenSystem>().RandomGenPerThread;

        EntityQuery spawnPointQuery = GetEntityQuery(ComponentType.ReadOnly<SpawnPoint>(), ComponentType.ReadOnly<Translation>());
        var spawnPoints = spawnPointQuery.ToEntityArrayAsync(Allocator.TempJob, out var jobHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, jobHandle);

        Entities
            .WithDisposeOnCompletion(spawnPoints)
            .WithNativeDisableParallelForRestriction(randomGenPerThread)
            .ForEach((int nativeThreadIndex, int entityInQueryIndex, ref Spawner spawner) =>
        {
            spawner.timeUntilNextSpawn -= deltaTime;
            if (spawner.timeUntilNextSpawn > 0.0f)
            {
                return;
            }

            // Reset the spawn timer
            var randomGen = randomGenPerThread[nativeThreadIndex];
            spawner.timeUntilNextSpawn += randomGen.NextFloat(spawner.minTimeBetweenSpawns, spawner.maxTimeBetweenSpawns);
            
            // Generate a random sampling of indices
            NativeArray<int> spawnPointIndices = new NativeArray<int>(spawnPoints.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < spawnPointIndices.Length; ++i)
            {
                spawnPointIndices[i] = i;
            }
            for (int i = spawnPointIndices.Length - 1; i > 0; --i)
            {
                int temp = spawnPointIndices[i];
                int swapIndex = randomGen.NextInt(0, i + 1);
                spawnPointIndices[i] = spawnPointIndices[swapIndex];
                spawnPointIndices[swapIndex] = temp;
            }

            // Retrieve a valid spawn point
            int targetIndex = -1;
            for (int i = 0; i < spawnPointIndices.Length; ++i)
            {
                var spawnPoint = GetComponent<SpawnPoint>(spawnPoints[spawnPointIndices[i]]);
                if (!spawnPoint.occluded)
                {
                    targetIndex = i;
                    break;
                }
            }
            if (targetIndex < 0)
            {
                // Track the random generator changes
                randomGenPerThread[nativeThreadIndex] = randomGen;  
                spawnPointIndices.Dispose();
                return;
            }

            // Randomly set the spawn position
            Entity spawnedEntity = commandBuffer.Instantiate(spawner.prefab);
            commandBuffer.SetComponent(spawnedEntity, new Translation
            {
                Value = GetComponent<Translation>(spawnPoints[targetIndex]).Value,
            });
        
            // Randomly set the projectile forces
            float2 groundDirection = math.normalize(randomGen.NextFloat2Direction());
            float3 linearVelocity = new float3(groundDirection.x, 0.0f, groundDirection.y);
            commandBuffer.SetComponent(spawnedEntity, new PhysicsVelocity
            {
                Linear = linearVelocity,
                Angular = float3.zero,
            });
        
            // Track the random generator changes
            randomGenPerThread[nativeThreadIndex] = randomGen;    
            spawnPointIndices.Dispose();
        }).Schedule();

        m_entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
