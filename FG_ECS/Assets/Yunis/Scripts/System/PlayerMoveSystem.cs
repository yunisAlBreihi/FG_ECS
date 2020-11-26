using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

[AlwaysSynchronizeSystem]
public class PlayerMoveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.
            WithAll<PlayerTag>().
            ForEach((ref Translation translation, in MovePlayer moveData) =>
            {
                translation.Value += moveData.moveHorizontal * moveData.moveSpeed * deltaTime;
            }).Run();
    }
}
