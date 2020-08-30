using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

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
        Entity player = players[0];
        if (!HasComponent<Translation>(player))
        {
            return;
        }

        Entities.WithAll<Enemy>().ForEach((ref Movable movable, in Translation translation) =>
        {
            float3 desiredDirection = GetComponent<Translation>(player).Value - translation.Value;
            movable.direction = desiredDirection;
        }).Schedule();

        players.Dispose();
    }
}
