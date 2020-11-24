using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;

public struct MovementJob : IJobParallelForTransform
{
    public float moveSpeed;
    public float topBound;
    public float botBound;
    public float deltaTime;

    public void Execute(int index, UnityEngine.Jobs.TransformAccess transform)
    {
        Vector3 pos = transform.position;
        pos += moveSpeed * deltaTime * (transform.rotation * new Vector3(0.0f, 0.0f, 1.0f));

        if (pos.z < botBound)
            pos.z = topBound;

        transform.position = pos;
    }
}
