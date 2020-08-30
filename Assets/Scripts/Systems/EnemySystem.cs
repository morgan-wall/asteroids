﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(PlayerSystem))]
[UpdateBefore(typeof(MovableSystem))]
public class EnemySystem : SystemBase
{
    protected override void OnUpdate()
    {
        EntityQuery playerQuery = GetEntityQuery(ComponentType.ReadOnly<Player>());
        if (playerQuery.CalculateEntityCount() <= 0)
        {
            return;
        }

        NativeArray<Entity> players = playerQuery.ToEntityArray(Allocator.TempJob);
        if (players.Length <= 0)
        {
            return;
        }

        Entities.WithAll<Enemy>().ForEach((ref Movable movable, in Translation translation) =>
        {
            int closestPlayerIndex = -1;
            float sqDistanceToClosestPlayer = float.MaxValue;
            for (int i = 0; i < players.Length; ++i)
            {
                Entity player = players[i];
                if (!HasComponent<Translation>(player))
                {
                    continue;
                }

                float sqDistance = math.distancesq(GetComponent<Translation>(player).Value, translation.Value);
                if (closestPlayerIndex < 0
                    || sqDistance < sqDistanceToClosestPlayer)
                {
                    closestPlayerIndex = i;
                    sqDistanceToClosestPlayer = sqDistance;
                }
            }
            if (closestPlayerIndex < 0)
            {
                return;
            }

            float3 desiredDirection = GetComponent<Translation>(players[closestPlayerIndex]).Value - translation.Value;
            movable.direction = desiredDirection;
        }).Schedule();

        players.Dispose();
    }
}
