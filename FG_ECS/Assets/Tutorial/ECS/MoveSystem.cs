using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

public class MoveSystem : SystemBase
{
    //OnUpdate runs on the main thread.
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities
            .WithName("MoveSystem")
            .ForEach((ref Translation translation, in Move move) =>
            {
                //move.velocity = math.sin(move.velocity);
                translation.Value += move.velocity * move.direction * deltaTime;
            }).ScheduleParallel();
    }
}
