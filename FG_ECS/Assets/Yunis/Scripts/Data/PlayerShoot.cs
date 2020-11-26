using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct PlayerShoot : IComponentData
{
    public Entity bulletPrefab;
    public KeyCode shootButton;
    public float shootCooldownTime;
    [System.NonSerialized] public bool pressedShoot;
    [System.NonSerialized] public float shootTimer;
}
