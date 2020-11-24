using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;


public class GameManagerJobs : MonoBehaviour
{
    #region GameManagerDefault
    public static GameManagerJobs GM = null;

    [Header("Simulation Settings")]
    public float topBound = 20.0f;
    public float botBound = -20.0f;
    public float rightBound = 20.0f;
    public float leftBound = -20.0f;

    [Header("Enemy Settings")]
    public GameObject shipPrefab = null;
    public float shipSpeed = 15.0f;

    [Header("Spawn Settings")]
    public int shipCount = 1;
    public int sipIncrement = 1;

    int count = 0;

    private void Awake()
    {
        if (GM == null)
        {
            GM = this;
        }
        else if (GM != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    TransformAccessArray transforms;
    MovementJob moveJob;
    JobHandle moveHandle;

    private void OnDisable()
    {
        moveHandle.Complete();
        transforms.Dispose();
    }

    private void Start()
    {
        transforms = new TransformAccessArray(0, -1);

        AddShips(shipCount);
    }

    private void Update()
    {
        moveHandle.Complete();

        if (Input.GetKeyDown("space"))
            AddShips(sipIncrement);

        moveJob = new MovementJob()
        {
            moveSpeed = shipSpeed,
            topBound = topBound,
            botBound = botBound,
            deltaTime = Time.deltaTime
        };

        moveHandle = moveJob.Schedule(transforms);

        JobHandle.ScheduleBatchedJobs();
    }

    void AddShips(int amount) 
    {
        moveHandle.Complete();

        transforms.capacity = transforms.length + amount;

        for (int i = 0; i < amount; i++)
        {
            float xVal = Random.Range(leftBound, rightBound);
            float zVal = Random.Range(0.0f, 10.0f);

            Vector3 pos = new Vector3(xVal, 0.0f, zVal + topBound);
            Quaternion rot = Quaternion.Euler(0.0f, 180.0f, 0.0f);

            var obj = Instantiate(shipPrefab, pos, rot);
            transforms.Add(obj.transform);
        }

        count += amount;
        Debug.Log(count);
    }
}
