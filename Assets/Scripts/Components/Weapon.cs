using Unity.Entities;
using Unity.Mathematics;

public struct Weapon : IComponentData
{
    public bool fire;
    public Entity projectilePrefab;
    public float muzzleOffset;
    public float3 muzzleDirection;
}
