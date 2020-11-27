using Unity.Entities;

[GenerateAuthoringComponent]
public struct EnemySpawnBullets : IComponentData
{
    public Entity bulletPrefab;
    public float spawnCooldown;
    [System.NonSerialized] public float spawnTimer;
}
