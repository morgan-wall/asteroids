using Unity.Entities;

public struct Damage : IComponentData
{
    public float value;
    public bool canDestroy;
    public bool destroySelfOnApply;
    public uint physicsCategoryMask;
}
