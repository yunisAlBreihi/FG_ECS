using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Vector2 spawnPosXRange = Vector2.zero;
    public Vector2 spawnPosZRange = Vector2.zero;
    public Vector2 spawnTimeRange = Vector2.zero;
    public Vector2Int spawnedPerWaveRange = Vector2Int.zero;
    public GameObject prefab = null;

    private GameObjectConversionSettings settings;
    private Entity prefabEntity;
    private EntityManager entityManager;

    private int spawnedPerWave = 0;
    private float maxSpawnTime = 0.2f;
    private float spawnTimer = 1.0f;



    private void Start()
    {
        settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        prefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        spawnedPerWave = UnityEngine.Random.Range(spawnedPerWaveRange.x, spawnedPerWaveRange.y);
        spawnTimer = maxSpawnTime;
    }

    private void Update()
    {
        if (spawnTimer >= maxSpawnTime)
        {
            for (int i = 0; i < spawnedPerWave; i++)
            {
                float randomXPos = UnityEngine.Random.Range(spawnPosXRange.x + transform.position.x, spawnPosXRange.y + transform.position.x);
                float randomZPos = UnityEngine.Random.Range(spawnPosZRange.x + transform.position.z, spawnPosZRange.y + transform.position.z);

                var instance = entityManager.Instantiate(prefabEntity);
                entityManager.SetComponentData(instance,
                        new Translation { Value = new float3(randomXPos, transform.position.y, randomZPos) });
            }
            maxSpawnTime = UnityEngine.Random.Range(spawnTimeRange.x, spawnTimeRange.y);
            spawnedPerWave = UnityEngine.Random.Range(spawnedPerWaveRange.x, spawnedPerWaveRange.y);
            spawnTimer = 0.0f;
        }
        spawnTimer += Time.deltaTime;
    }
}
