using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(CollisionSystem))]
public class CollectionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        var entityCommandBuffer = entityCommandBufferSystem.CreateCommandBuffer();

        Entities.WithAll<Player>().ForEach((DynamicBuffer<TriggerBuffer> triggerBuffer) =>
        {
            for (int i = 0; i < triggerBuffer.Length; ++i)
            {
                Entity collectableEntity = triggerBuffer[i].entity;
                if (HasComponent<Collectable>(collectableEntity))
                {
                    entityCommandBuffer.DestroyEntity(collectableEntity);
                }
            }
        }).Schedule();

        entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
