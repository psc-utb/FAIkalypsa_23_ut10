using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ConeCollider))]
public class ZombieEyes : MonoBehaviour
{
    ConeCollider coneCollider;

    [SerializeField]
    GameObject Player;

    public UnityEvent<bool> PlayerIsVisible;


    void Awake()
    {
        coneCollider = GetComponent<ConeCollider>();
    }


    public void CheckPlayerVisibility(GameObject gameObj)
    {
        if (gameObj == Player)
        {
            Vector3 position = this.transform.position;
            Vector3 direction = (Player.transform.position - position).normalized;

            Debug.DrawRay(position, direction * coneCollider.Distance, Color.blue);
            //Debug.DrawLine(position, position + direction * coneCollider.Distance, Color.red, Mathf.Infinity);

            Ray ray = new Ray(position, direction);
            RaycastHit rayCastHit = new RaycastHit();
            Physics.Raycast(ray, out rayCastHit, coneCollider.Distance);
            //Physics.SphereCast(ray, 1f, out rayCastHit, coneCollider.Distance)

            if (rayCastHit.collider != null && rayCastHit.collider.gameObject == Player)
            {
                //Debug.Log("Player is visible");
                PlayerIsVisible?.Invoke(true);
            }
            else
            {
                //Debug.Log("Player is not visible");
                PlayerIsVisible?.Invoke(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckPlayerVisibility(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        CheckPlayerVisibility(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == Player)
        {
            PlayerIsVisible?.Invoke(false);
        }
    }

}
