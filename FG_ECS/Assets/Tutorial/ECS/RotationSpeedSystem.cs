using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class RotationSpeedSystem : SystemBase
{
    //OnUpdate runs on the main thread.
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities
            .WithName("RotationSpeedSystem")
            .ForEach((ref Rotation rotation, in RotationSpeed rotationSpeed) =>
            {
                rotation.Value = math.mul(
                    math.normalize(rotation.Value),
                    quaternion.AxisAngle(math.up(), rotationSpeed.speed * deltaTime));
            }).ScheduleParallel();
    }
}