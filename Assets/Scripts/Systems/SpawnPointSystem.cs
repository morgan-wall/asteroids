using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(SpawnerSystem))]
public class SpawnPointSystem : SystemBase
{
    protected override void OnUpdate()
    {
        EntityQuery occluderQuery = GetEntityQuery(typeof(SpawnPointOccluder), typeof(Translation));
        NativeArray<Entity> occluders = occluderQuery.ToEntityArray(Allocator.TempJob);

        Entities.ForEach((ref SpawnPoint spawnPoint, in Translation translation) =>
        {
            spawnPoint.occluded = false;
            float3 spawnPointPosition = translation.Value;
            for (int i = 0; i < occluders.Length; ++i)
            {
                Entity occluder = occluders[i];
                float occluderRadius = GetComponent<SpawnPointOccluder>(occluder).radius;
                float3 occluderPosition = GetComponent<Translation>(occluder).Value;
                if (math.distancesq(occluderPosition, spawnPointPosition) < (occluderRadius * occluderRadius))
                {
                    spawnPoint.occluded = true;
                    break;
                }
            }
        }).Run();

        occluders.Dispose();
    }
}
