using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct MovePlayer : IComponentData
{
    public float3 moveHorizontal;
    public float moveSpeed;
}
