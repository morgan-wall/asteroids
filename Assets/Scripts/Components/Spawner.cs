using Unity.Entities;

public struct Spawner : IComponentData
{
    public Entity prefab;
    public float maxOffset;
    public float timeUntilNextSpawn;
    public float minTimeBetweenSpawns;
    public float maxTimeBetweenSpawns;
}
