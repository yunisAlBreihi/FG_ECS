using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class DeathTriggerSystem : JobComponentSystem
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
    public struct DeathTriggerJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<DeathTrigger> deathTriggerGroup;
        [ReadOnly] public ComponentDataFromEntity<BulletTag> bulletGroup;
        public EntityCommandBuffer commandBuffer;

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            bool isBodyATrigger = deathTriggerGroup.HasComponent(entityA);
            bool isBodyBTrigger = deathTriggerGroup.HasComponent(entityB);
            bool isEntityABullet = bulletGroup.HasComponent(entityA);
            bool isEntityBBullet = bulletGroup.HasComponent(entityB);

            if (isBodyATrigger && isEntityBBullet)
            {
                Debug.Log("Trigger A");
                commandBuffer.DestroyEntity(entityB);
            }
            else if(isBodyBTrigger && isEntityABullet)
            {
                Debug.Log("Trigger B");
                commandBuffer.DestroyEntity(entityA);
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new DeathTriggerJob();
        job.deathTriggerGroup = GetComponentDataFromEntity<DeathTrigger>(true);
        job.bulletGroup = GetComponentDataFromEntity<BulletTag>(true);
        job.commandBuffer = commandBufferSystem.CreateCommandBuffer();
        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);

        commandBufferSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }
}
