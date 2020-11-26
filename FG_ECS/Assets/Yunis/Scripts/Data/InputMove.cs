using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct InputMove : IComponentData
{
    public KeyCode right;
    public KeyCode left;
}
