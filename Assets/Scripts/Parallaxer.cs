using UnityEngine;

public class Parallaxer : MonoBehaviour
{

    [System.Serializable]
    public struct YSpawnRange
    {
        public float min;
        public float max;
    }

	[System.Serializable]
    public struct XSpawnRate
    {
        public float min;
        public float max;
    }

    public GameObject Prefab;
    public int poolSize;
    public float shiftSpeed;
    

    public YSpawnRange ySpawnRange;
    public Vector3 defaultSpawnPos;
    public bool spawnImmediate; // Particle prewarm
    public Vector3 immediateSpawnPos;
    public Vector2 targetAspectRatio;
	public XSpawnRate xSpawnRate;

    private float spawnTimer;
	private float spawnRate;
    private float targetAspect;
    private PoolObject[] poolObjects;
    private GameManager gameManager;

    private void Awake()
    {
        Configure();
    }

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    private void OnEnable()
    {
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    private void OnDisable()
    {
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    private void Update()
    {
        if (gameManager.GameOver) return;

        Shift();
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnRate)
        {
			spawnRate = Random.Range(xSpawnRate.min * 10, xSpawnRate.max * 10) / 10f;
            Spawn();
            spawnTimer = 0;
        }
    }

    private void Configure()
    {
        // spawning pool objects
        targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        poolObjects = new PoolObject[poolSize];
        for (int i = 0; i < poolObjects.Length; i++)
        {
            GameObject go = Instantiate(Prefab) as GameObject;
            Transform t = go.transform;
            t.SetParent(transform);
            t.position = Vector3.one * 1000;
            poolObjects[i] = new PoolObject(t);
        }

        if (spawnImmediate)
        {
            SpawnImmediate();
        }

        spawnTimer = 0;
		spawnRate = Random.Range(xSpawnRate.min, xSpawnRate.max);
    }

    private void OnGameOverConfirmed()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].Dispose();
            poolObjects[i].transform.position = Vector3.one * 1000;
        }

        Configure();
    }

    private void Spawn()
    {
        Transform t = GetPoolObject();
        if (t == null) return; // if true, this indicates that poolSize is to small

        Vector3 pos = Vector3.zero;
        pos.x = (defaultSpawnPos.x * Camera.main.aspect) / targetAspect;
        pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        t.position = pos;
    }

    private void SpawnImmediate()
    {
        Transform t = GetPoolObject();
        if (t == null) return;

        Vector3 pos = Vector3.zero;
        pos.x = (immediateSpawnPos.x * Camera.main.aspect) / targetAspect;
        pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        t.position = pos;

        Spawn();
    }

    private void Shift()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].transform.position += Vector3.right * shiftSpeed * Time.deltaTime;
            CheckDisposeObject(poolObjects[i]);
        }
    }

    private void CheckDisposeObject(PoolObject poolObject)
    {
        // place objects off screen
        if (poolObject.transform.position.x < (-defaultSpawnPos.x * Camera.main.aspect) / targetAspect)
        {
            poolObject.Dispose();
            poolObject.transform.position = Vector3.one * 1000;
        }
    }

    private Transform GetPoolObject()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            if (!poolObjects[i].inUse)
            {
                poolObjects[i].Use();
                return poolObjects[i].transform;
            }
        }
        return null;
    }





    class PoolObject
    {
        public Transform transform;
        public bool inUse;

        public PoolObject(Transform t)
        {
            transform = t;
        }

        public void Use()
        {
            inUse = true;
        }

        public void Dispose()
        {
            inUse = false;
        }
    }

}
