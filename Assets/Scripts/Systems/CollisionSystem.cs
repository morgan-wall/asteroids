using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
[UpdateBefore(typeof(CollectionSystem))]
[UpdateBefore(typeof(DamageSystem))]
public class CollisionSystem : SystemBase
{
    [BurstCompile]
    private struct CollisionJob : ICollisionEventsJob
    {
        public BufferFromEntity<CollisionBuffer> m_collisions;

        public void Execute(CollisionEvent collisionEvent)
        {
            if (m_collisions.HasComponent(collisionEvent.EntityA))
            {
                var buffer = new CollisionBuffer()
                {
                    entity = collisionEvent.EntityB,
                };
                m_collisions[collisionEvent.EntityA].Add(buffer);
            }

            if (m_collisions.HasComponent(collisionEvent.EntityB))
            {
                var buffer = new CollisionBuffer()
                {
                    entity = collisionEvent.EntityA,
                };
                m_collisions[collisionEvent.EntityB].Add(buffer);
            }
        }
    }

    [BurstCompile]
    private struct TriggerJob : ITriggerEventsJob
    {
        public BufferFromEntity<TriggerBuffer> m_triggers;

        public void Execute(TriggerEvent triggerEvent)
        {
            if (m_triggers.HasComponent(triggerEvent.EntityA))
            {
                var buffer = new TriggerBuffer()
                {
                    entity = triggerEvent.EntityB,
                };
                m_triggers[triggerEvent.EntityA].Add(buffer);
            }

            if (m_triggers.HasComponent(triggerEvent.EntityB))
            {
                var buffer = new TriggerBuffer()
                {
                    entity = triggerEvent.EntityA,
                };
                m_triggers[triggerEvent.EntityB].Add(buffer);
            }
        }
    }

    protected override void OnUpdate()
    {
        PhysicsWorld physicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>().PhysicsWorld;
        ISimulation sim = World.GetOrCreateSystem<StepPhysicsWorld>().Simulation;

        // Process all the collisions
        Entities.ForEach((DynamicBuffer<CollisionBuffer> collisions) =>
        {
            collisions.Clear();
        }).Run();

        var collisionJob = new CollisionJob()
        {
            m_collisions = GetBufferFromEntity<CollisionBuffer>(),
        };
        JobHandle collisionJobHandle = collisionJob.Schedule(sim, ref physicsWorld, Dependency);
        collisionJobHandle.Complete();

        // Process all the triggers
        Entities.ForEach((DynamicBuffer<TriggerBuffer> triggers) =>
        {
            triggers.Clear();
        }).Run();

        var triggerJob = new TriggerJob()
        {
            m_triggers = GetBufferFromEntity<TriggerBuffer>(),
        };
        JobHandle triggerJobHandle = triggerJob.Schedule(sim, ref physicsWorld, Dependency);
        triggerJobHandle.Complete();
    }
}
