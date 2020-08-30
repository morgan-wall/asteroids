using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[AlwaysUpdateSystem]
[UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
public class GameStateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        EntityQuery collectableQuery = GetEntityQuery(ComponentType.ReadOnly<Collectable>());
        if (collectableQuery.CalculateEntityCount() <= 0)
        {
            Debug.Log("You won!");
        }
        else
        {
            EntityQuery playerQuery = GetEntityQuery(ComponentType.ReadOnly<Player>());
            if (playerQuery.CalculateEntityCount() <= 0)
            {
                Debug.Log("You lose!");
            }
        }
    }
}
