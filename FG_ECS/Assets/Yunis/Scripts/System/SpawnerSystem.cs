using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

[AlwaysSynchronizeSystem]
public class SpawnerSystem : SystemBase
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        uint randomSeed = (uint)UnityEngine.Random.Range(0, 1000000);
        Unity.Mathematics.Random random = new Unity.Mathematics.Random(randomSeed);

        float deltaTime = Time.DeltaTime;

        Entities
            .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref Spawner spawner, in LocalToWorld location) => 
            {
                if (spawner.spawnTimer >= spawner.spawnCooldown)
                {
                    for (int i = 0; i <= spawner.spawnsPerWave; i++)
                    {
                        float spawnRandPosX =random.NextFloat(spawner.spawnPosXRange.x, spawner.spawnPosXRange.y);
                        float spawnRandPosZ =random.NextFloat(spawner.spawnPosZRange.x, spawner.spawnPosZRange.y);
                        float3 spawnPos = new float3(translation.Value.x + spawnRandPosX, translation.Value.y, translation.Value.z + spawnRandPosZ);

                        var instance = commandBuffer.Instantiate(entityInQueryIndex, spawner.prefab);
                        commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation { Value = spawnPos });
                    }
                    spawner.spawnTimer = 0.0f;
                    spawner.spawnCooldown = random.NextFloat(spawner.spawnTimeRange.x, spawner.spawnTimeRange.y);
                    spawner.spawnsPerWave = random.NextInt(spawner.spawnedPerWaveRange.x, spawner.spawnedPerWaveRange.y);
                }

                if (spawner.spawnTimer <= spawner.spawnCooldown)
                {
                    spawner.spawnTimer += deltaTime;
                }
            }).ScheduleParallel();
    }
}
