using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct EnemyMove : IComponentData
{
    public float3 velocity;
    public float3 waveDirection;
    public float waveSpeed;
    public float waveMagnitude;
}