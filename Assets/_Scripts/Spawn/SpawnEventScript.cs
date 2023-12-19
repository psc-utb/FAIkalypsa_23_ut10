using Assets.Pool_Spawner_Pro_Upgrade;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEventScript : MonoBehaviour, ISpawnEventable
{
    public event Action SpawnEvent;

    [SerializeField]
    private GameObject CollisionTriggerGO;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject == CollisionTriggerGO)
            SpawnEvent?.Invoke();
    }
}
