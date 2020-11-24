using UnityEngine;

public class Movement : MonoBehaviour
{
    void Update()
    {
        Vector3 pos = transform.position;
        pos += transform.forward * GameManager.GM.shipSpeed * Time.deltaTime;

        if (pos.z < GameManager.GM.botBound)
            pos.z = GameManager.GM.topBound;

        transform.position = pos;
    }
}
