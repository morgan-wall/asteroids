using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(MovableSystem))]
public class PlayerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Entities.WithAll<Player>().ForEach((ref Movable movable) =>
        {
            movable.direction = new float3(x, 0.0f, z);
        }).Schedule();
    }
}
