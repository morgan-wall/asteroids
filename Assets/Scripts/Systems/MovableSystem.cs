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
        
        //Entities.ForEach((ref Translation translation, ref Rotation rotation, in Movable movable) =>
        //{
        //    translation.Value += deltaTime * movable.translationSpeed * movable.direction;
        //    rotation.Value = math.mul(rotation.Value.value, quaternion.AxisAngle(movable.rotationAxis, deltaTime * movable.rotationSpeed));
        //}).Schedule();

        Entities.ForEach((ref PhysicsVelocity physicsVelocity, in Movable movable) =>
        {
            physicsVelocity.Linear = movable.translationSpeed * movable.direction;
        }).Schedule();
    }
}
