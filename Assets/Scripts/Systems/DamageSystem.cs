using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(CollisionSystem))]
public class DamageSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        var entityCommandBuffer = entityCommandBufferSystem.CreateCommandBuffer(); // MW_TODO: .AsParallelWriter();

        Entities.ForEach((Entity entity, DynamicBuffer<CollisionBuffer> collisionBuffer, ref Health health) =>
        {
            bool destroyDamager = HasComponent<DestroyDamager>(entity);
            for (int i = 0; i < collisionBuffer.Length; ++i)
            {
                Entity damagingEntity = collisionBuffer[i].entity;
                if (HasComponent<Damage>(damagingEntity))
                {
                    health.value -= GetComponent<Damage>(damagingEntity).value;
                    if (destroyDamager)
                    {
                        entityCommandBuffer.DestroyEntity(damagingEntity);
                    }
                }
            }

            if (health.value <= 0.0f)
            {
                entityCommandBuffer.DestroyEntity(entity);
            }
        }).Schedule();

        entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
