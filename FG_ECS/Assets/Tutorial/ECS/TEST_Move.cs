using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct TEST_Move : IComponentData
{
    public float3 direction;
    public float3 velocity;
}
