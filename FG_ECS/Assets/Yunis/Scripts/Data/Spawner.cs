using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Spawner : IComponentData
{
    public Entity prefab;
    public float2 spawnPosXRange;
    public float2 spawnPosZRange;
    public float2 spawnTimeRange;
    public int2 spawnedPerWaveRange;

    [System.NonSerialized] public float spawnsPerWave;
    [System.NonSerialized] public float spawnCooldown;
    [System.NonSerialized] public float spawnTimer;
}