using Unity.Entities;

public struct Damage : IComponentData
{
    public float value;
    public uint physicsCategoryMask;
}
