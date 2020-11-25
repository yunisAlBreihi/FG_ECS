using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class TEST_RotationSpeedSystem : SystemBase
{
    //OnUpdate runs on the main thread.
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities
            .WithName("TEST_RotationSpeedSystem")
            .ForEach((ref Rotation rotation, in TEST_RotationSpeed rotationSpeed) =>
            {
                rotation.Value = math.mul(
                    math.normalize(rotation.Value),
                    quaternion.AxisAngle(math.up(), rotationSpeed.speed * deltaTime));
            }).ScheduleParallel();
    }
}