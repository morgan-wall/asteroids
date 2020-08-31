using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Collections;

[UpdateAfter(typeof(SpawnPointSystem))]
public class SpawnerSystem : SystemBase
{
    EntityCommandBufferSystem m_memoryBarrier = null;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_memoryBarrier = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        var commandBuffer = m_memoryBarrier.CreateCommandBuffer();
        var randomGenPerThread = World.GetExistingSystem<RandomGenSystem>().RandomGenPerThread;

        EntityQuery spawnPointQuery = GetEntityQuery(typeof(SpawnPoint), typeof(Translation));
        NativeArray<Entity> spawnPoints = spawnPointQuery.ToEntityArray(Allocator.TempJob);

        Entities
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
            
            // Determine if a valid spawn point is available
            int targetIndex = -1;
            int lastIndex = randomGen.NextInt(0, spawnPoints.Length);
            int currentIndex = (lastIndex + 1) % spawnPoints.Length;
            while (currentIndex != lastIndex)
            {
                var spawnPoint = GetComponent<SpawnPoint>(spawnPoints[currentIndex]);
                if (!spawnPoint.occluded)
                {
                    targetIndex = currentIndex;
                    break;
                }
                currentIndex = (currentIndex + 1) % spawnPoints.Length;
            }
            if (targetIndex < 0)
            {
                // Track the random generator changes
                randomGenPerThread[nativeThreadIndex] = randomGen;  
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
        }).Run();

        m_memoryBarrier.AddJobHandleForProducer(Dependency);

        spawnPoints.Dispose();
    }
}
