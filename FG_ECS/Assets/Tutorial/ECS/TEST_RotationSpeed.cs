using System;
using Unity.Entities;

// ReSharper disable once InconsistentNaming
[GenerateAuthoringComponent]
public struct TEST_RotationSpeed : IComponentData
{
    public float speed;
}
