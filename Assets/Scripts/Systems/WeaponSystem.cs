using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

public class WeaponSystem : SystemBase
{
    EntityCommandBufferSystem m_memoryBarrier = null;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_memoryBarrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = m_memoryBarrier.CreateCommandBuffer().AsParallelWriter();

        Entities.ForEach((int entityInQueryIndex, ref Weapon weapon, in Translation translation, in LocalToWorld localToWorld) =>
        {
            if (!weapon.fire)
            {
                return;
            }
            weapon.fire = false;
            
            Entity instance = commandBuffer.Instantiate(entityInQueryIndex, weapon.projectilePrefab);
            
            var instanceTranslation = new Translation
            {
                Value = translation.Value + weapon.muzzleOffset,
            };
            commandBuffer.SetComponent(entityInQueryIndex, instance, instanceTranslation);

            var physicsVelocity = new PhysicsVelocity
            {
                Linear = weapon.muzzleDirection, // MW_TODO
                Angular = float3.zero,
            };
            commandBuffer.SetComponent(entityInQueryIndex, instance, physicsVelocity);
        }).ScheduleParallel();

        m_memoryBarrier.AddJobHandleForProducer(Dependency);
    }
}
