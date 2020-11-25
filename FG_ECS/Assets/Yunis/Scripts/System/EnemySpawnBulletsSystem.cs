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
        double time = Time.ElapsedTime;

        Entities
            .WithName("SpawnerSystem_FromEntity")
            .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, in EnemySpawnBullets enemySpawnBullets, in LocalToWorld location) =>
            {
                if (math.round(time % enemySpawnBullets.spawnCooldown) == 0.0f)
                {
                    Debug.Log("Shoot!");
                    var instance = commandBuffer.Instantiate(entityInQueryIndex, enemySpawnBullets.bulletPrefab);
                    commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation { Value = translation.Value });
                }

                //commandBuffer.DestroyEntity(entityInQueryIndex, entity);
            }).ScheduleParallel();
    }
}
