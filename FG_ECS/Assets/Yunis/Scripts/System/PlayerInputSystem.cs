using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[AlwaysSynchronizeSystem]
public class PlayerInputSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.
            WithAll<PlayerTag>().
            ForEach((ref MovePlayer moveData,ref PlayerShoot playerShoot, in InputMove inputData) =>
            {
                bool isRightKeyPressed = Input.GetKey(inputData.right);
                bool isLeftKeyPressed = Input.GetKey(inputData.left);

                moveData.moveHorizontal.x = (isRightKeyPressed) ? 1 : 0;
                moveData.moveHorizontal.x -= (isLeftKeyPressed) ? 1 : 0;

                playerShoot.pressedShoot = Input.GetKey(playerShoot.shootButton);
            }).Run();
    }
}
