using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

[UpdateAfter(typeof(PlayerSystem))]
[UpdateAfter(typeof(EnemySystem))]
[UpdateBefore(typeof(MovableSystem))]
public class WeaponSystem : SystemBase
{
    EntityCommandBufferSystem m_entityCommandBufferSystem = null;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = m_entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        Entities.ForEach((int entityInQueryIndex, ref Weapon weapon, in Translation translation, in Rotation rotation, in LocalToWorld localToWorld) =>
        {
            if (!weapon.fire)
            {
                return;
            }
            weapon.fire = false;
            
            float3 muzzleForward = math.mul(localToWorld.Rotation, weapon.muzzleDirection);
            Entity instance = commandBuffer.Instantiate(entityInQueryIndex, weapon.projectilePrefab);
            commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation
            {
                Value = translation.Value + (muzzleForward * weapon.muzzleOffset),
            });
            commandBuffer.SetComponent(entityInQueryIndex, instance, new PhysicsVelocity
            {
                Linear = muzzleForward,
                Angular = float3.zero,
            });
        }).ScheduleParallel();

        m_entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
