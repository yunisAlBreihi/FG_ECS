using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

public class TEST_MoveSystem : SystemBase
{
    //OnUpdate runs on the main thread.
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities
            .WithName("TEST_MoveSystem")
            .ForEach((ref Translation translation, in TEST_Move move) =>
            {
                //move.velocity = math.sin(move.velocity);
                translation.Value += move.velocity * move.direction * deltaTime;
            }).ScheduleParallel();
    }
}
