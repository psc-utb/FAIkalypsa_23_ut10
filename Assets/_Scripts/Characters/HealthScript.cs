using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthScript : MonoBehaviour
{
    [SerializeField]
    private float health;
    public float Health
    {
        get => health;
        set
        {
            if (value <= 0)
            {
                health = 0;

                if (diedInvoked == false)
                {
                    diedInvoked = true;
                    Died?.Invoke(this.gameObject, ActiveAfterDeath);
                }
            }
            else
            { 
                health = value;
                diedInvoked = false;
            }
        }
    }

    [SerializeField]
    private float max_health;
    public float Max_Health { get => max_health; set => max_health = value; }

    [SerializeField]
    private bool activeAfterDeath = true;
    public bool ActiveAfterDeath { get => activeAfterDeath; set => activeAfterDeath = value; }

    public UnityEvent<GameObject, bool> Died;
    private bool diedInvoked = false;

    public bool IsAlive()
    {
        return Health > 0 ? true : false;
    }

    public void DecreaseHealth(float damage)
    {
        if (Health - damage < 0)
            Health = 0;
        else
            Health -= damage;
    }

    public void IncreaseHealth(float healing)
    {
        if (Health + healing > max_health)
            Health = max_health;
        else
            Health += healing;
    }
}
