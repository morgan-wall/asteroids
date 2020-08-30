using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

[UpdateAfter(typeof(PlayerSystem))]
[UpdateAfter(typeof(EnemySystem))]
[UpdateBefore(typeof(FreezePositionSystem))]
[UpdateBefore(typeof(ConstantForceSystem))]
public class MovableSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        
        Entities.ForEach((ref PhysicsVelocity physicsVelocity, ref Rotation rotation, in Movable movable) =>
        {
            physicsVelocity.Linear = movable.translationSpeed * movable.direction;
            rotation.Value = quaternion.LookRotation(movable.lookDirection, movable.up);
        }).Schedule();
    }
}
