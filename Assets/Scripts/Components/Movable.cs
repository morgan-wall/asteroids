using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Movable : IComponentData
{
    public float translationSpeed;
    public float3 direction;
    public float rotationSpeed;
    public float3 rotationAxis;
}
