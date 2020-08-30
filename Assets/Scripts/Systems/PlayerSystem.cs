using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateBefore(typeof(MovableSystem))]
public class PlayerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var gamepad = Gamepad.current;
        
        // Process movement
        Vector2 direction = gamepad.leftStick.ReadValue();
        direction.Normalize();
        Vector2 lookDirection = gamepad.rightStick.ReadValue();
        lookDirection.Normalize();
        Entities.WithAll<Player>().ForEach((ref Movable movable) =>
        {
            movable.direction = new float3(direction.x, 0.0f, direction.y);
            if (lookDirection != Vector2.zero)
            {
                movable.lookDirection = new float3(lookDirection.x, 0.0f, lookDirection.y);
            }
        }).Schedule();

        // Process attacks
        bool fire = gamepad.rightTrigger.wasPressedThisFrame;
        Entities.WithAll<Player>().ForEach((ref Weapon weapon) =>
        {
            weapon.fire = fire;
        }).Schedule();
    }
}
