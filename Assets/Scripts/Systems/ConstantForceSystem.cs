using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateAfter(typeof(FreezePositionSystem))]
[UpdateBefore(typeof(StepPhysicsWorld))]
public class ConstantForceSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref PhysicsVelocity physicsVelocity, in ConstantForce constantForce) =>
        {
            float3 normalizedVelocity = physicsVelocity.Linear;
            normalizedVelocity = math.normalize(normalizedVelocity);
            physicsVelocity.Linear = normalizedVelocity * constantForce.magnitude;
        }).Schedule();
    }
}
