using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

public class EnemySpawnBulletsSystem : SystemBase
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

    }

    protected override void OnUpdate()
    {
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        float deltaTime = Time.DeltaTime;

        Entities
            .WithAll<EnemyTag>()
            .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref EnemySpawnBullets enemySpawnBullets, in LocalToWorld location) =>
            {
                if (enemySpawnBullets.spawnTimer >= enemySpawnBullets.spawnCooldown)
                {
                        var instance = commandBuffer.Instantiate(entityInQueryIndex, enemySpawnBullets.bulletPrefab);
                        commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation { Value = translation.Value });
                    enemySpawnBullets.spawnTimer -= enemySpawnBullets.spawnCooldown;
                }
                if (enemySpawnBullets.spawnTimer <= enemySpawnBullets.spawnCooldown)
                {
                    enemySpawnBullets.spawnTimer += deltaTime;
                }
            }).ScheduleParallel();
    }
}
