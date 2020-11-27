using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

[AlwaysSynchronizeSystem]
public class PlayerShootSystem : SystemBase
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
            .WithAll<PlayerTag>()
            .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .ForEach((Entity entity, int entityInQueryIndex,ref Translation translation, ref PlayerShoot playerShoot, in LocalToWorld location) =>
            {
                if (playerShoot.pressedShoot == true)
                {
                    if (playerShoot.shootTimer >= playerShoot.shootCooldownTime)
                    {
                        var instance = commandBuffer.Instantiate(entityInQueryIndex, playerShoot.bulletPrefab);
                        commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation { Value = translation.Value });
                        playerShoot.shootTimer -= playerShoot.shootCooldownTime;
                    }
                }

                if (playerShoot.shootTimer <= playerShoot.shootCooldownTime) 
                    playerShoot.shootTimer += deltaTime;

            }).ScheduleParallel();
    }
}
