using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using UnityEngine.SceneManagement;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class CollisionSystem : JobComponentSystem
{
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;
    private EndSimulationEntityCommandBufferSystem commandBufferSystem;
    private const int sceneIndex = 0;

    protected override void OnCreate()
    {
        base.OnCreate();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    //[BurstCompile]
    public struct CollisionJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<DeathTriggerTag> deathTriggerGroup;
        [ReadOnly] public ComponentDataFromEntity<EnemyBulletTag> enemyBulletGroup;
        [ReadOnly] public ComponentDataFromEntity<PlayerBulletTag> playerBulletGroup;
        [ReadOnly] public ComponentDataFromEntity<PlayerTag> playerGroup;
        [ReadOnly] public ComponentDataFromEntity<EnemyTag> enemyGroup;
        public EntityCommandBuffer commandBuffer;
        [ReadOnly] public int sceneIndex;

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
                commandBuffer.DestroyEntity(entityA);
                commandBuffer.DestroyEntity(entityB);

                SceneManager.LoadScene(sceneIndex);
            }
            if (isEntityBEnemyBullet && isEntityAPlayer)
            {
                commandBuffer.DestroyEntity(entityA);
                commandBuffer.DestroyEntity(entityB);

                SceneManager.LoadScene(sceneIndex);

            }

            //Death player/enemy
            if (isEntityAEnemy && isEntityBPlayer)
            {
                commandBuffer.DestroyEntity(entityA);
                commandBuffer.DestroyEntity(entityB);

                SceneManager.LoadScene(sceneIndex);
                //entityManager.DestroyEntity(entityManager.UniversalQuery);
            }
            if (isEntityBEnemy && isEntityAPlayer)
            {
                commandBuffer.DestroyEntity(entityA);
                commandBuffer.DestroyEntity(entityB);

                SceneManager.LoadScene(sceneIndex);
            }

            //Death playerBullet/Enemy
            if (isEntityAPlayerBullet && isEntityBEnemy)
            {
                commandBuffer.DestroyEntity(entityA);
                commandBuffer.DestroyEntity(entityB);
            }
            if (isEntityBPlayerBullet && isEntityAEnemy)
            {
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
        job.sceneIndex = sceneIndex;

        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
        commandBufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}
