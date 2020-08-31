using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(WeaponSystem))]
public class EnemySystem : SystemBase
{
    protected override void OnUpdate()
    {
        EntityQuery playerQuery = GetEntityQuery(ComponentType.ReadOnly<Player>(), ComponentType.ReadOnly<Translation>());
        var playerPositions = playerQuery.ToComponentDataArrayAsync<Translation>(Allocator.TempJob, out var jobHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, jobHandle);

        Entities
            .WithAll<Enemy>()
            .WithDisposeOnCompletion(playerPositions)
            .ForEach((ref Movable movable, in Translation translation) =>
        {
            int closestPlayerIndex = -1;
            float sqDistanceToClosestPlayer = float.MaxValue;
            for (int i = 0; i < playerPositions.Length; ++i)
            {
                Translation playerPosition = playerPositions[i];
                float sqDistance = math.distancesq(playerPosition.Value, translation.Value);
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
        
            float3 desiredDirection = playerPositions[closestPlayerIndex].Value - translation.Value;
            movable.direction = desiredDirection;
        }).Schedule();
    }
}
