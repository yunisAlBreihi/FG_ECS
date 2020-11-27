using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class CollisionSystem : JobComponentSystem
{
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;
    private EndSimulationEntityCommandBufferSystem commandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    [BurstCompile]
    public struct CollisionJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<DeathTriggerTag> deathTriggerGroup;
        [ReadOnly] public ComponentDataFromEntity<EnemyBulletTag> enemyBulletGroup;
        [ReadOnly] public ComponentDataFromEntity<PlayerBulletTag> playerBulletGroup;
        [ReadOnly] public ComponentDataFromEntity<PlayerTag> playerGroup;
        [ReadOnly] public ComponentDataFromEntity<EnemyTag> enemyGroup;
        public EntityCommandBuffer commandBuffer;

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            bool isBodyATrigger = deathTriggerGroup.HasComponent(entityA);
            bool isBodyBTrigger = deathTriggerGroup.HasComponent(entityB);
            bool isEntityAEnemyBullet = enemyBulletGroup.HasComponent(entityA);
            bool isEntityBEnemyBullet = enemyBulletGroup.HasComponent(entityB);
            bool isEntityAPlayerBullet = playerBulletGroup.HasComponent(entityA);
            bool isEntityBPlayerBullet = playerBulletGroup.HasComponent(entityB);
            bool isEntityAPlayer = playerGroup.HasComponent(entityA);
            bool isEntityBPlayer = playerGroup.HasComponent(entityB);
            bool isEntityAEnemy = enemyGroup.HasComponent(entityA);
            bool isEntityBEnemy = enemyGroup.HasComponent(entityB);

            //if ((isEntityAEnemyBullet && isEntityBPlayer) || (isEntityBEnemyBullet && isEntityAPlayer) ||
            //    (isEntityAPlayerBullet && isEntityBEnemy) || (isEntityBPlayerBullet && isEntityAEnemy) ||
            //    (isEntityAPlayer && isEntityBEnemy) || (isEntityBPlayer && isEntityAEnemy))
            //{
            //    Debug.Log("Trigger A");
            //    commandBuffer.DestroyEntity(entityA);
            //    commandBuffer.DestroyEntity(entityB);
            //}

            //Kill whatever hits the death box
            if (isBodyATrigger &&
               (isEntityBEnemyBullet || isEntityBPlayerBullet || isEntityBEnemy))
            {
                commandBuffer.DestroyEntity(entityB);
            }
            if (isBodyBTrigger &&
               (isEntityAEnemyBullet || isEntityAPlayerBullet || isEntityAEnemy))
            {
                commandBuffer.DestroyEntity(entityA);
            }

            //Death player/enemyBullet
            if (isEntityAEnemyBullet && isEntityBPlayer)
            {
                Debug.Log("death of bullet!");
                commandBuffer.DestroyEntity(entityA);
                commandBuffer.DestroyEntity(entityB);
            }
            if (isEntityBEnemyBullet && isEntityAPlayer)
            {
                Debug.Log("death of bullet 2!");
                commandBuffer.DestroyEntity(entityA);
                commandBuffer.DestroyEntity(entityB);
            }

            //Death player/enemy
            if (isEntityAEnemy && isEntityBPlayer)
            {
                Debug.Log("death of enemy!");
                commandBuffer.DestroyEntity(entityA);
                commandBuffer.DestroyEntity(entityB);
            }
            if (isEntityBEnemy && isEntityAPlayer)
            {
                Debug.Log("death of enemy 2!");
                commandBuffer.DestroyEntity(entityA);
                commandBuffer.DestroyEntity(entityB);
            }

            //Death playerBullet/Enemy
            if (isEntityAPlayerBullet && isEntityBEnemy)
            {
                Debug.Log("death of bullet!");
                commandBuffer.DestroyEntity(entityA);
                commandBuffer.DestroyEntity(entityB);
            }
            if (isEntityBPlayerBullet && isEntityAEnemy)
            {
                Debug.Log("death of bullet 2!");
                commandBuffer.DestroyEntity(entityA);
                commandBuffer.DestroyEntity(entityB);
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new CollisionJob();
        job.deathTriggerGroup = GetComponentDataFromEntity<DeathTriggerTag>(true);
        job.enemyBulletGroup = GetComponentDataFromEntity<EnemyBulletTag>(true);
        job.playerBulletGroup = GetComponentDataFromEntity<PlayerBulletTag>(true);
        job.enemyGroup = GetComponentDataFromEntity<EnemyTag>(true);
        job.playerGroup = GetComponentDataFromEntity<PlayerTag>(true);
        job.commandBuffer = commandBufferSystem.CreateCommandBuffer();

        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
        commandBufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}
