using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAid : MonoBehaviour
{
    [SerializeField]
    private float healingValue = 10;

    [SerializeField]
    private string charactersLayerName = "Player";

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(charactersLayerName))
        {
            collision.gameObject.GetComponent<HealthScript>().IncreaseHealth(healingValue);
            Destroy(gameObject);
        }
    }

}
