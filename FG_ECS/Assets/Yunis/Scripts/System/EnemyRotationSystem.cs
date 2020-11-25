using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class EnemyRotationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        float sinTimer = (float)Time.ElapsedTime;

        Entities
            .WithName("EnemyRotationSystem")
            .ForEach((ref Rotation rotation, in EnemyRotation enemyRotation) =>
            {
                rotation.Value = math.mul(
                    math.normalize(rotation.Value),
                    quaternion.AxisAngle(math.up(), enemyRotation.speed * deltaTime));
            }).ScheduleParallel();
    }
}