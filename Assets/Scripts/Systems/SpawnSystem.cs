using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class SpawnSystem : SystemBase
{
    EntityCommandBufferSystem m_memoryBarrier = null;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_memoryBarrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        var commandBuffer = m_memoryBarrier.CreateCommandBuffer().AsParallelWriter();
        var randomGenPerThread = World.GetExistingSystem<RandomGenSystem>().RandomGenPerThread;

        Entities
            .WithNativeDisableParallelForRestriction(randomGenPerThread)
            .ForEach((int nativeThreadIndex, int entityInQueryIndex, ref Spawner spawner, in Translation translation) =>
        {
            if (spawner.timeUntilNextSpawn <= 0.0f)
            {
                // Reset the spawn timer
                var randomGen = randomGenPerThread[nativeThreadIndex];
                spawner.timeUntilNextSpawn = randomGen.NextFloat(spawner.minTimeBetweenSpawns, spawner.maxTimeBetweenSpawns);
                
                // Setup the entity
                Entity spawnedEntity = commandBuffer.Instantiate(entityInQueryIndex, spawner.prefab);
                var spawnedTranslation = new Translation
                {
                    Value = translation.Value + randomGen.NextFloat3Direction() * spawner.maxOffset,
                };
                commandBuffer.SetComponent(entityInQueryIndex, spawnedEntity, translation);

                // Track the random generator changes
                randomGenPerThread[nativeThreadIndex] = randomGen;
            }

            spawner.timeUntilNextSpawn -= deltaTime;
        }).ScheduleParallel();

        m_memoryBarrier.AddJobHandleForProducer(Dependency);
    }
}
