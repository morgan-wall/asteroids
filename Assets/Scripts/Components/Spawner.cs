using Unity.Entities;
using UnityEngine;

public struct Spawner : IComponentData
{
    public Entity prefab;
    public float timeUntilNextSpawn;
    public float minTimeBetweenSpawns;
    public float maxTimeBetweenSpawns;
}
