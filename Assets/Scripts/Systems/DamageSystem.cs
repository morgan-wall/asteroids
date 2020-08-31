using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;

[UpdateAfter(typeof(CollisionSystem))]
public class DamageSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        var entityCommandBuffer = entityCommandBufferSystem.CreateCommandBuffer();

        Entities.ForEach((int entityInQueryIndex, Entity entity, DynamicBuffer<CollisionBuffer> collisionBuffer, ref Health health, in PhysicsCollider collider) =>
        {
            uint belongsTo = collider.Value.Value.Filter.BelongsTo;
            for (int i = 0; i < collisionBuffer.Length; ++i)
            {
                Entity damagingEntity = collisionBuffer[i].entity;
                if (HasComponent<Damage>(damagingEntity))
                {
                    var damage = GetComponent<Damage>(damagingEntity);
                    if ((belongsTo & damage.physicsCategoryMask) != 0)
                    {
                        health.value -= damage.value;
                        if (damage.destroySelfOnApply)
                        {
                            entityCommandBuffer.DestroyEntity(damagingEntity);
                        }
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
