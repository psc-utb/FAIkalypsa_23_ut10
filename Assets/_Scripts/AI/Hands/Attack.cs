using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField]
    private float damage;
    public float Damage { get => damage; set => damage = value; }

    private float timeStart = 0;

    [SerializeField]
    private float cooldown = 0.5f;

    private void OnEnable()
    {
        //timeStart = Time.fixedTime;
        //attacked = false;
    }

    bool attacked = false;
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if (Time.fixedTime - timeStart >= cooldown)
            {
                timeStart = Time.fixedTime;
                attacked = false;
            }

            if (Time.fixedTime - timeStart == 0 && attacked == false)
            {
                GameObject player = collider.gameObject;
                HealthScript healthScript = player.GetComponent<HealthScript>();
                if (healthScript != null)
                {
                    healthScript.DecreaseHealth(damage);
                    attacked = true;
                }
            }

            return;
        }
    }

}
