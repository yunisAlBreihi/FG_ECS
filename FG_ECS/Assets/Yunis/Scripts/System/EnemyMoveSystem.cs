using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class EnemyMoveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        float sinTimer = (float)Time.ElapsedTime;

        Entities
            .WithName("EnemyMoveSystem")
            .ForEach((ref Translation translation, in EnemyMove move) =>
            {
                float3 sinMovement = (math.sin(move.waveDirection * translation.Value.z * move.waveSpeed) * move.waveMagnitude) * deltaTime;
                translation.Value += move.velocity * deltaTime + sinMovement;
            }).ScheduleParallel();
    }
}
