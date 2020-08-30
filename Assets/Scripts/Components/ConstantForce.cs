using Unity.Entities;

[GenerateAuthoringComponent]
public struct ConstantForce : IComponentData
{
    public float magnitude;
}
