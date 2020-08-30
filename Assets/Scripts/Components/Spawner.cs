using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Spawner : IComponentData
{
    public Entity prefab;
    public float maxOffset;
    public float timeUntilNextSpawn;
    public float minTimeBetweenSpawns;
    public float maxTimeBetweenSpawns;
}
