using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct FreezePosition : IComponentData
{
    public bool x;
    public bool y;
    public bool z;
}
