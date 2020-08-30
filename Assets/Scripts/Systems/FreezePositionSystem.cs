using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Physics;

[UpdateAfter(typeof(MovableSystem))]
[UpdateBefore(typeof(ConstantForceSystem))]
public class FreezePositionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Movable movable, ref PhysicsVelocity physicsVelocity, ref Translation translation, in FreezePosition freezePosition) =>
        {
            if (freezePosition.x)
            {
                movable.direction.x = 0.0f;
                physicsVelocity.Linear.x = 0.0f;
                translation.Value.x = freezePosition.position.x;
            }
            if (freezePosition.y)
            {
                movable.direction.y = 0.0f;
                physicsVelocity.Linear.y = 0.0f;
                translation.Value.y = freezePosition.position.y;
            }
            if (freezePosition.z)
            {
                movable.direction.z = 0.0f;
                physicsVelocity.Linear.z = 0.0f;
                translation.Value.z = freezePosition.position.z;
            }
        }).Schedule();
    }
}
