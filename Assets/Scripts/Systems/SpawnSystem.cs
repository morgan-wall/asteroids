using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class SpawnSystem : SystemBase
{
    EntityCommandBufferSystem memoryBarrier => World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>(); // MW_TODO: why =>

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        var randomGenPerThread = World.GetExistingSystem<RandomGenSystem>().RandomGenPerThread;
        var commandBuffer = memoryBarrier.CreateCommandBuffer();//.AsParallelWriter();

        Entities
            .WithNativeDisableParallelForRestriction(randomGenPerThread)
            .ForEach((int nativeThreadIndex, int entityInQueryIndex, ref Spawner spawner, in Translation translation) =>
        {
            if (spawner.timeUntilNextSpawn <= 0.0f)
            {
                // Reset the spawn timer
                var randomGen = randomGenPerThread[nativeThreadIndex];
                spawner.timeUntilNextSpawn = randomGen.NextFloat(spawner.minTimeBetweenSpawns, spawner.maxTimeBetweenSpawns);
                
                //// Setup the entity
                Entity spawnedEntity = commandBuffer.Instantiate(spawner.prefab);
                var spawnedTranslation = new Translation
                {
                    Value = translation.Value + randomGen.NextFloat3Direction() * spawner.maxOffset,
                };
                commandBuffer.SetComponent(spawnedEntity, translation);

                // Track the random generator changes
                randomGenPerThread[nativeThreadIndex] = randomGen;
            }

            spawner.timeUntilNextSpawn -= deltaTime;
        }).Schedule();

        memoryBarrier.AddJobHandleForProducer(Dependency);
    }
}
