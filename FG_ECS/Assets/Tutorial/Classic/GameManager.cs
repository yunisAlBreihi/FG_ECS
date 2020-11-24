using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region GameManagerDefault
    public static GameManager GM = null;

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

    private void Start()
    {
        AddShips(shipCount);
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
            AddShips(sipIncrement);
    }

    void AddShips(int amount) 
    {
        for (int i = 0; i < amount; i++)
        {
            float xVal = Random.Range(leftBound, rightBound);
            float zVal = Random.Range(0.0f, 10.0f);

            Vector3 pos = new Vector3(xVal, 0.0f, zVal + topBound);
            Quaternion rot = Quaternion.Euler(0.0f, 180.0f, 0.0f);

            var obj = Instantiate(shipPrefab, pos, rot);
        }

        count += amount;
        Debug.Log(count);
    }
}
