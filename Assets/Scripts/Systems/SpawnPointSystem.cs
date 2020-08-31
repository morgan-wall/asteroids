using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(SpawnSystem))]
public class SpawnPointSystem : SystemBase
{
    private static readonly int s_minSpawnRings = 1;
    private static readonly int s_minSpawnPointsPerRing = 1;
    private static readonly float3 s_basisVector = new float3(0.0f, 0.0f, 1.0f);
    private static readonly float3 s_axisVector = new float3(0.0f, 1.0f, 0.0f);

    EntityCommandBufferSystem m_memoryBarrier = null;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_memoryBarrier = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = m_memoryBarrier.CreateCommandBuffer().AsParallelWriter();
        
        Entities.ForEach((DynamicBuffer<SpawnPointBuffer> spawnPointBuffer, in Spawner spawner) =>
        {
            spawnPointBuffer.Clear();
        }).Run();

        Entities.ForEach((int entityInQueryIndex, Entity entity, DynamicBuffer<SpawnPointBuffer> spawnPointBuffer, in Spawner spawner) =>
        {
            int spawnRings = math.max(s_minSpawnRings, (int)math.round(spawner.maxOffset / spawner.spawnRingsPerMetre));
            float spawnRingDelta = spawner.maxOffset / spawnRings;
            for (int i = 0; i < spawnRings; ++i)
            {
                float ringRadius = math.lerp(0.0f, spawner.maxOffset, (i + 1) / (float)spawnRings);
                float ringCircumference = 2.0f * math.PI * ringRadius;
                int spawnPointsForRing = math.max(s_minSpawnPointsPerRing, (int)math.round(ringCircumference / spawner.spawnPointsPerMetre));
                for (int j = 0; j < spawnPointsForRing; ++j)
                {
                    float angleOffset = math.lerp(0.0f, 360.0f, (j + 1) / (float)spawnPointsForRing);
                    float3 position = math.mul(quaternion.AxisAngle(s_axisVector, angleOffset), s_basisVector) * ringRadius;
                    commandBuffer.AppendToBuffer(entityInQueryIndex, entity, new SpawnPointBuffer
                    {
                        position = position,
                    });
                }
            }
        }).ScheduleParallel();
        
        m_memoryBarrier.AddJobHandleForProducer(Dependency);
        
        Enabled = false;
    }
}
