using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateBefore(typeof(WeaponSystem))]
public class PlayerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var gamepad = Gamepad.current;
        bool fire = gamepad.rightTrigger.wasPressedThisFrame;
        
        Vector2 direction = gamepad.leftStick.ReadValue();
        direction.Normalize();
        
        Vector2 lookDirection = gamepad.rightStick.ReadValue();
        lookDirection.Normalize();
        
        Entities.WithAll<Player>().ForEach((ref Weapon weapon, ref Movable movable) =>
        {
            weapon.fire = fire;
            movable.direction = new float3(direction.x, 0.0f, direction.y);
            if (lookDirection != Vector2.zero)
            {
                movable.lookDirection = new float3(lookDirection.x, 0.0f, lookDirection.y);
            }
        }).Schedule();
    }
}
