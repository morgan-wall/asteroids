using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct TriggerBuffer : IBufferElementData
{
    public Entity entity;
}
