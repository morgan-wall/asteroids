using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using System.Diagnostics.Tracing;

[AlwaysUpdateSystem]
[UpdateInGroup(typeof(PresentationSystemGroup))]
public class GameStateSystem : SystemBase
{
    public delegate void GameOverDelegate();

    public event GameOverDelegate OnGameOver;

    protected override void OnUpdate()
    {
        EntityQuery playerQuery = GetEntityQuery(ComponentType.ReadOnly<Player>());
        if (playerQuery.CalculateEntityCount() > 0)
        {
            return;
        }

        var entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        var entityCommandBuffer = entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        Entities.ForEach((int entityInQueryIndex, Entity entity) =>
        {
            entityCommandBuffer.AddComponent<Disabled>(entityInQueryIndex, entity);
        }).ScheduleParallel();

        entityCommandBufferSystem.AddJobHandleForProducer(Dependency);

        if (OnGameOver != null)
        {
            OnGameOver();
        }
    }
}
