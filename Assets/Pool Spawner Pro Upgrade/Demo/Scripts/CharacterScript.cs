using Assets.Pool_Spawner_Pro_Upgrade;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterScript : MonoBehaviour, IDespawnable, IReinitializable
{

    public event Action<GameObject, bool> Despawn;

    public bool IsAlive
    {
        get
        {
            return currentHealth > 0;
        }
    }


    [SerializeField]
    private int maxHealth = 10;
    [SerializeField]
    private int currentHealth = 10;

    // Start is called before the first frame update
    void Start()
    {
        CheckIfDied();
    }

    [SerializeField]
    float timeToDecreaseHealth = 1;
    float currentTimeToDecreaseHealth = 0;

    [SerializeField]
    int damage = 1;


    // Update is called once per frame
    void Update()
    {
        currentTimeToDecreaseHealth += Time.deltaTime;
        if (currentTimeToDecreaseHealth > timeToDecreaseHealth )
        {
            currentTimeToDecreaseHealth = 0;
            DecreaseHealth(damage);
        }
    }

    public void DecreaseHealth(int damage)
    {
        currentHealth -= damage;
        CheckIfDied();
    }

    private void CheckIfDied()
    {
        if (IsAlive == false)
        {
            Despawn?.Invoke(this.gameObject, false);
        }
    }

    public void Reinitialization()
    {
        currentTimeToDecreaseHealth = 0;
        currentHealth = maxHealth;
    }
}
