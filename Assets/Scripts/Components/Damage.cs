using Unity.Entities;

public struct Damage : IComponentData
{
    public float value;
    public bool destroySelfOnApply;
    public uint physicsCategoryMask;
}
