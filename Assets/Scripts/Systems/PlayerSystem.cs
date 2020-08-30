using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

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

        bool fire = Input.GetButtonDown("Fire1");
        Entities.WithAll<Player>().ForEach((ref Weapon weapon) =>
        {
            weapon.fire = fire;
        }).Schedule();
    }
}
