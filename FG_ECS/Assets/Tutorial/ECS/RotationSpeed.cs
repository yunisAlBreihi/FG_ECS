using System;
using Unity.Entities;

// ReSharper disable once InconsistentNaming
[GenerateAuthoringComponent]
public struct RotationSpeed : IComponentData
{
    public float speed;
}
