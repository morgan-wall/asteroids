using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateAfter(typeof(MovableSystem))]
[UpdateBefore(typeof(ConstantForceSystem))]
public class FreezePositionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref PhysicsVelocity physicsVelocity, ref Movable movable, in FreezePosition freezePosition) =>
        {
            if (freezePosition.x)
            {
                movable.direction.x = 0.0f;
                physicsVelocity.Linear.x = 0.0f;
            }
            if (freezePosition.y)
            {
                movable.direction.y = 0.0f;
                physicsVelocity.Linear.y = 0.0f;
            }
            if (freezePosition.z)
            {
                movable.direction.z = 0.0f;
                physicsVelocity.Linear.z = 0.0f;
            }
        }).Schedule();
    }
}
