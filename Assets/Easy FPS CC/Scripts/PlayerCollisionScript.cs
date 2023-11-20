using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCollisionScript : MonoBehaviour
{

    public UnityEvent<bool> GroundedChanged;

    public UnityEvent<bool> RayCastGroundedChanged;


    [Tooltip("Put 'Player' layer here")]
    [Header("Shooting Properties")]
    private LayerMask ignoreLayer;//to ignore player layer

    /*
	 * Getting the Players CharacterController component.
	 * And grabbing the mainCamera from Players child transform.
	 */
    void Awake()
    {
        ignoreLayer = 1 << LayerMask.NameToLayer("Player");

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RayCastGrounded();
    }

    /*
	* checks if our player is contacting the ground in the angle less than 60 degrees
	*	if it is, set groudede to true
	*/
    void OnCollisionEnter(Collision other)
    {
        ContactPointsCheck(other.contacts);
    }

    void OnCollisionStay(Collision other)
    {
        ContactPointsCheck(other.contacts);
    }

    void ContactPointsCheck(ContactPoint[] contactPoints)
    {
        foreach (ContactPoint contact in contactPoints)
        {
            if (Vector2.Angle(contact.normal, Vector3.up) < 60)
            {
                GroundedChanged?.Invoke(true);
            }
        }
    }


    /*
	* On collision exit set grounded to false
	*/
    void OnCollisionExit()
    {
        GroundedChanged?.Invoke(false);
    }


    /*
	* Raycasts down to check if we are grounded along the gorunded method() because if the
	* floor is curvy it will go ON/OFF constatly this assures us if we are really grounded
	*/
    private bool RayCastGrounded()
    {
        RaycastHit groundedInfo;
        if (Physics.Raycast(transform.position, transform.up * -1f, out groundedInfo, 1, ~ignoreLayer))
        {
            Debug.DrawRay(transform.position, transform.up * -1f, Color.red, 0.0f);
            if (groundedInfo.transform != null)
            {
                //print ("vracam true");
                RayCastGroundedChanged?.Invoke(true);
                return true;
            }
            else
            {
                //print ("vracam false");
                RayCastGroundedChanged?.Invoke(false);
                return false;
            }
        }
        //print ("nisam if dosao");

        RayCastGroundedChanged?.Invoke(false);
        return false;
    }
}
