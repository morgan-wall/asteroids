using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public struct Damage : IComponentData
{
    public float value;
    public uint physicCategoryMask;
}
