#define USE_SPAWN_POINTS

using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Jobs;
using Unity.Transforms;

public class SpawnSystem : SystemBase
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
        var commandBuffer = m_memoryBarrier.CreateCommandBuffer().AsParallelWriter();
        var randomGenPerThread = World.GetExistingSystem<RandomGenSystem>().RandomGenPerThread;

#if USE_SPAWN_POINTS
        Entities
            .WithNativeDisableParallelForRestriction(randomGenPerThread)
            .ForEach((int nativeThreadIndex, int entityInQueryIndex, DynamicBuffer<SpawnPointBuffer> spawnPointBuffer, ref Spawner spawner) =>
        {
            if (spawner.timeUntilNextSpawn <= 0.0f)
            {
                // Reset the spawn timer
                var randomGen = randomGenPerThread[nativeThreadIndex];
                spawner.timeUntilNextSpawn = randomGen.NextFloat(spawner.minTimeBetweenSpawns, spawner.maxTimeBetweenSpawns);
                
                Entity spawnedEntity = commandBuffer.Instantiate(entityInQueryIndex, spawner.prefab);

                // Randomly set the spawn position
                int spawnPointIndex = randomGen.NextInt(0, spawnPointBuffer.Length);
                commandBuffer.SetComponent(entityInQueryIndex, spawnedEntity, new Translation
                {
                    Value = spawnPointBuffer[spawnPointIndex].position,
                });

                // Randomly set the projectile forces
                float2 groundDirection = math.normalize(randomGen.NextFloat2Direction());
                float3 linearVelocity = new float3(groundDirection.x, 0.0f, groundDirection.y);
                commandBuffer.SetComponent(entityInQueryIndex, spawnedEntity, new PhysicsVelocity
                {
                    Linear = linearVelocity,
                    Angular = float3.zero,
                });

                // Track the random generator changes
                randomGenPerThread[nativeThreadIndex] = randomGen;
            }

            spawner.timeUntilNextSpawn -= deltaTime;
        }).ScheduleParallel();
#else
        Entities
            .WithNativeDisableParallelForRestriction(randomGenPerThread)
            .ForEach((int nativeThreadIndex, int entityInQueryIndex, ref Spawner spawner, in Translation translation) =>
        {
            if (spawner.timeUntilNextSpawn <= 0.0f)
            {
                // Reset the spawn timer
                var randomGen = randomGenPerThread[nativeThreadIndex];
                spawner.timeUntilNextSpawn = randomGen.NextFloat(spawner.minTimeBetweenSpawns, spawner.maxTimeBetweenSpawns);
                
                Entity spawnedEntity = commandBuffer.Instantiate(entityInQueryIndex, spawner.prefab);

                // Randomly set the spawn position
                float offsetDistance = randomGen.NextFloat(0.0f, spawner.maxOffset);
                float2 groundOffset = math.normalize(randomGen.NextFloat2Direction());
                float3 offset = new float3(groundOffset.x, 0.0f, groundOffset.y) * offsetDistance;
                commandBuffer.SetComponent(entityInQueryIndex, spawnedEntity, new Translation
                {
                    Value = translation.Value + offset,
                });

                // Randomly set the projectile forces
                float2 groundDirection = math.normalize(randomGen.NextFloat2Direction());
                float3 linearVelocity = new float3(groundDirection.x, 0.0f, groundDirection.y);
                commandBuffer.SetComponent(entityInQueryIndex, spawnedEntity, new PhysicsVelocity
                {
                    Linear = linearVelocity,
                    Angular = float3.zero,
                });

                // Track the random generator changes
                randomGenPerThread[nativeThreadIndex] = randomGen;
            }

            spawner.timeUntilNextSpawn -= deltaTime;
        }).ScheduleParallel();
#endif // USE_SPAWN_POINTS

        m_memoryBarrier.AddJobHandleForProducer(Dependency);
    }
}
