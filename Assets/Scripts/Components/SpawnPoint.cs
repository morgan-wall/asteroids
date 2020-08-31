using Unity.Entities;

[GenerateAuthoringComponent]
public struct SpawnPoint : IComponentData
{
    public bool occluded;
}
