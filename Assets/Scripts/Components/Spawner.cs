using Unity.Entities;
using UnityEngine;

public struct Spawner : IComponentData
{
    public Entity prefab;
    public float maxOffset;
    public float timeUntilNextSpawn;
    public float minTimeBetweenSpawns;
    public float maxTimeBetweenSpawns;
    public float spawnPointsPerMetre;
    public float spawnRingsPerMetre;
}
