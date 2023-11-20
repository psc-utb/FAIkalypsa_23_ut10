using Assets.Pool_Spawner_Pro_Upgrade;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FixedSpawnCharacter : MonoBehaviour
{
    [Header("Spawned Object Holder (Optional)")]
    [TextArea]
    public string note = "Leave Parent Object empty if you don't want objects to travel with it and instead attach a gameObject to Position Point in Object Properties. Follow this demo and configuration if you are confused.";
    [SerializeField] private GameObject parentObject;

    [System.Serializable]
    public class ObjectProperties
    {
        public GameObject poolObject;
        public int count;
        public float spawnChance;
        public GameObject positionPoint;
        public Vector3 customPosition;
        public Vector3 randPosFactor;
        public Vector3 rotation;
        public Vector3 randRotFactor;
    }
    [System.Serializable]
    public class Distance
    {
        public bool disableByDistance;
        public float distance;
    }
    [System.Serializable]
    public class SpawnType
    {
        public bool continuousSpawn;
        public KeyCode spawnKey;
        [Tooltip("Optional; assign a reference in the Editor that implements ISpawnEventable interface")]
        public GameObject SpawnEventableGO;
    }
    [Header("Spawn By Key Press or Continuous Spawn?")]
    [SerializeField] private SpawnType spawnType;

    [Header("Spawned object properties")]
    [SerializeField] private ObjectProperties objectProperties;

    [Header("Object Disabling")]
    [SerializeField] private Distance disableByDistance;

    //This list is used to keep track of all the spawned objects' position
    //to disable them when they are behing the deactivation point.
    private Queue<GameObject> objectPool = new Queue<GameObject>();
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private Dictionary<GameObject, Vector3> initPos = new Dictionary<GameObject, Vector3>();

    private void Awake()
    {
        Initialize();

        //reinitialization of the gameobject
        if (spawnType.SpawnEventableGO != null)
        {
            ISpawnEventable spawnEventableObj = spawnType.SpawnEventableGO.GetComponent<ISpawnEventable>();
            if (spawnEventableObj != null)
            {
                spawnEventableObj.SpawnEvent += this.SpawnObject;
            }
        }
    }

    private void Update()
    {
        if (!spawnType.continuousSpawn)
        {
            if (Input.GetKeyDown(spawnType.spawnKey))
            {
                SpawnObject(objectProperties.customPosition, objectProperties.randPosFactor, objectProperties.rotation, objectProperties.randRotFactor, objectProperties.spawnChance);
            }
        }
        else
        {
            SpawnObject(objectProperties.customPosition, objectProperties.randPosFactor, objectProperties.rotation, objectProperties.randRotFactor, objectProperties.spawnChance);
        }

        if (disableByDistance.disableByDistance)
        {
            CheckDistance();
        }
    }

    //Create objects before the game starts to make the ready to be reused.
    private void Initialize()
    {
        for (int i = 0; i < objectProperties.count; i++)
        {
            GameObject obj = Instantiate(objectProperties.poolObject);
            obj.SetActive(false);
            if (parentObject)
            {
                obj.transform.parent = parentObject.transform;
            }
            initPos.Add(obj, Vector3.zero);
            objectPool.Enqueue(obj);
            spawnedObjects.Add(obj);
        }

        if(objectProperties.count == 0 && objectProperties.poolObject != null)
        {
            //connect method with the despawn event if no count is set but there is a pool object
            IDespawnable despawnableObj = objectProperties.poolObject.GetComponent<IDespawnable>();
            if (despawnableObj != null)
            {
                despawnableObj.Despawn += this.DeSpawntoPool;
            }
        }
    }

    //Returns an object from the queue based on the given ID.
    public GameObject SpawnfromPool()
    {
        if (objectPool.Count > 0)
        {
            GameObject objecttoSpawn = objectPool.Dequeue();

            return objecttoSpawn;
        }
        return null;
    }

    public void SpawnObject()
    {
        SpawnObject(objectProperties.customPosition, objectProperties.randPosFactor, objectProperties.rotation, objectProperties.randRotFactor, objectProperties.spawnChance);
    }

    //Manages the spawning of objects based on the input parameter values of object properties.
    private void SpawnObject(Vector3 position, Vector3 randPosFactor, Vector3 rotation, Vector3 randRotFactor, float spawnChance)
    {
        //Generate random value and spawn based on it.
        if (Random.value <= spawnChance)
        {
            GameObject obj = SpawnfromPool();

            if (obj != null)
            {

                Vector3 randomPos = new Vector3(Random.Range(-randPosFactor.x, randPosFactor.x),
                                             Random.Range(-randPosFactor.y, randPosFactor.y),
                                             Random.Range(-randPosFactor.z, randPosFactor.z));

                Vector3 randomRot = new Vector3(Random.Range(-randRotFactor.x, randRotFactor.x),
                                             Random.Range(-randRotFactor.y, randRotFactor.y),
                                             Random.Range(-randRotFactor.z, randRotFactor.z));

                obj.transform.localRotation = Quaternion.Euler(rotation + randomRot);


                //connect method with the despawn event
                IDespawnable despawnableObj = obj.GetComponent<IDespawnable>();
                if (despawnableObj != null)
                {
                    despawnableObj.Despawn += this.DeSpawntoPool;
                }


                #region For Demonstration Purposes
                IPooledObject interfaceObj = obj.GetComponent<IPooledObject>();

                if (interfaceObj != null)
                {
                    interfaceObj.LaunchBlock();
                }
                #endregion

                //set position of the spawned object
                if (parentObject)
                {
                    obj.transform.localPosition = position + randomPos;
                }
                else
                {
                    if (!objectProperties.positionPoint)
                    {
                        obj.transform.position = position + randomPos;
                    }
                    else
                    {
                        obj.transform.position = objectProperties.positionPoint.transform.position;
                    }
                }


                //reinitialization of the gameobject
                IReinitializable reinitializableObj = obj.GetComponent<IReinitializable>();
                if (reinitializableObj != null)
                {
                    reinitializableObj.Reinitialization();
                }


                obj.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Pool with objects is empty!");
            }
        }
    }

    //Disable when the objects pass the set maximum distance in forward direction.
    private void CheckDistance()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (parentObject)
            {
                if (obj.transform.localPosition.z >= disableByDistance.distance)
                {
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }
            }
            else if (objectProperties.positionPoint)
            {
                if (obj.transform.position.z >= objectProperties.positionPoint.transform.position.z + disableByDistance.distance)
                {
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }
            }
        }
    }

    public void DeSpawntoPool(GameObject obj)
    {
        //disconnect method with the despawn event
        DeSpawntoPool(obj, false);
    }


    public void DeSpawntoPool(GameObject obj, bool setActive)
    {
        //disconnect method with the despawn event
        IDespawnable despawnableObj = obj.GetComponent<IDespawnable>();
        if (despawnableObj != null)
        {
            despawnableObj.Despawn -= this.DeSpawntoPool;
        }
        obj.SetActive(setActive);
        objectPool.Enqueue(obj);
    }
}