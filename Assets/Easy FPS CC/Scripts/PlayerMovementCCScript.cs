using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementCCScript : MonoBehaviour
{
    CharacterController characterController;

    [Tooltip("Current player's speed")]
    [SerializeField]
    private float currentSpeed;
    public float CurrentSpeed
    {
        get { return currentSpeed; }
        set { currentSpeed = value; }
    }


    [Tooltip("The maximum Player's speed  during moving")]
    [SerializeField]
    private int maxMoveSpeed = 5;
    public int MaxMoveSpeed
    {
        get { return maxMoveSpeed; }
        set { maxMoveSpeed = value; }
    }

    [Tooltip("gravity acceleration")]
    [SerializeField]
    private float gravity = 5.0f; //9.81f;

    [Tooltip("Maximum jump duration")]
    [SerializeField]
    float jumpTimeMax = 0.3f;

    [Tooltip("Maximum jump height")]
    [SerializeField]
    private float jumpHeight = 0.7f;


    [Tooltip("Assign player's camera here")]
    private Transform cameraMain;

    [Tooltip("Position of the camera inside the player")]
    public Vector3 CameraPosition { get; set; }

    /*
	 * Getting the Players CharacterController component.
	 * And grabbing the mainCamera from Players child transform.
	 */
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        cameraMain = transform.Find("Main Camera").transform;
        bulletSpawn = cameraMain.Find("BulletSpawn").transform;
        ignoreLayer = 1 << LayerMask.NameToLayer("Player");

    }

    //private Vector3 slowdownV;

    /*
	* Raycasting for meele attacks and input movement handling here.
	*/
    void FixedUpdate()
    {
        RaycastForMeleeAttacks();

        PlayerMovementLogic();
    }

    /*
	* Accordingly to input adds force and if magnitude is bigger it will clamp it.
	* If player leaves keys it will deaccelerate
	*/
    void PlayerMovementLogic()
    {
        currentSpeed = characterController.velocity.magnitude;


        // Apply gravity (so the object will fall if not grounded)
        characterController.Move(new Vector3(0, -gravity * Time.deltaTime, 0));

        /*if (grounded)
        {
            characterController.Move(Vector3.SmoothDamp(characterController.velocity,
                new Vector3(0, characterController.velocity.y, 0),
                ref slowdownV,
                deaccelerationSpeed));
        }*/

        if (grounded)
        {
            // Determine how much should move in the x-direction
            Vector3 movementX = Input.GetAxis("Horizontal") * Vector3.right * maxMoveSpeed * Time.deltaTime;

            // Determine how much should move in the z-direction
            Vector3 movementZ = Input.GetAxis("Vertical") * Vector3.forward * maxMoveSpeed * Time.deltaTime;

            // Convert combined Vector3 from local space to world space based on the position of the current gameobject (player)
            Vector3 movement = transform.TransformDirection(movementZ + movementX);

            if (!jumpPressed)
                // Apply gravity (so the object will fall if not grounded)
                movement.y -= gravity * Time.deltaTime;

            //Debug.Log("Movement Vector = " + movement);

            // Actually move the character controller in the movement direction
            characterController.Move(movement);
            //characterController.AddRelativeForce(Input.GetAxis("Horizontal") * accelerationSpeed * Time.deltaTime, 0, Input.GetAxis("Vertical") * accelerationSpeed * Time.deltaTime);
        }
        else
        {

            // Determine how much should move in the x-direction
            Vector3 movementX = Input.GetAxis("Horizontal") * Vector3.right * maxMoveSpeed * Time.deltaTime;

            // Determine how much should move in the z-direction
            Vector3 movementZ = Input.GetAxis("Vertical") * Vector3.forward * maxMoveSpeed * Time.deltaTime;

            // Convert combined Vector3 from local space to world space based on the position of the current gameobject (player)
            Vector3 movement = transform.TransformDirection(movementZ / 2 + movementX / 2);
            //Vector3 movement = transform.TransformDirection(new Vector3(0, gravity * Time.deltaTime, 0));

            if (!jumpPressed)
                // Apply gravity (so the object will fall if not grounded)
                movement.y -= gravity * Time.deltaTime;

            //Debug.Log("Movement Vector = " + movement);

            // Actually move the character controller in the movement direction
            characterController.Move(movement);
            //characterController.AddRelativeForce(Input.GetAxis("Horizontal") * accelerationSpeed / 2 * Time.deltaTime, 0, Input.GetAxis("Vertical") * accelerationSpeed / 2 * Time.deltaTime);
        }

        /*
		 * Slippery issues fixed here
		 */
       /* if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            deaccelerationSpeed = 0.5f;
        }
        else
        {
            deaccelerationSpeed = 0.1f;
        }*/
    }

    /*
	* Handles jumping and ads the force and sounds.
	*/
    bool jumpPressed = false;
    Vector3 jumpVectorLinear;
    float jumpTime = 0;
    void Jumping()
    {
        Vector3 startPosition = new Vector3(0, 0, 0);
        Vector3 jumpVector = new Vector3(0, jumpHeight * Time.deltaTime, 0);
        jumpVectorLinear = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(0, jumpHeight, 0), Time.deltaTime * 15);
        if (Input.GetButtonDown("Jump") && grounded && jumpTime < jumpTimeMax)
        {
            //characterController.attachedArticulationBody.AddRelativeForce(Vector3.up * jumpForce);
            //characterController.Move(Vector3.SmoothDamp(characterController.velocity, new Vector3(0, 200, 0), ref currentSpeed, 1));
            //characterController.Move(Vector3.Lerp(characterController.velocity, new Vector3(0, 200, 0), Time.deltaTime * 15));
            //jumpVector.y += Mathf.Sqrt(jumpHeightPerSecond * -3.0f * gravity);

            jumpPressed = true;
            jumpTime += Time.deltaTime;
            characterController.Move(jumpVectorLinear);

            if (_jumpSound)
                _jumpSound.Play();
            else
                print("Missig jump sound.");
            _walkSound.Stop();
            _runSound.Stop();

            //Debug.Log("Position during jump: " + characterController.gameObject.transform.position);
        }
        else if (jumpPressed && Input.GetButton("Jump") && jumpTime < jumpTimeMax)
        {
            jumpTime += Time.deltaTime;
            characterController.Move(jumpVectorLinear);

            //Debug.Log("Position during jump: " + characterController.gameObject.transform.position);
        }
        else
        {
            jumpTime = 0;
            jumpPressed = false;
        }
    }

    /*
	* Update loop calling other stuff
	*/
    void Update()
    {


        Jumping();

        Crouching();

        WalkingSound();


    }//end update


    public void SetGround(bool grounded)
    {
        this.grounded = grounded;
    }


    public void SetRayCastGrounded(bool rayCastGrounded)
    {
        this.rayCastGrounded = rayCastGrounded;
    }


    /*
	* Checks if player is grounded and plays the sound accorindlgy to his speed
	*/
    void WalkingSound()
    {
        if (_walkSound && _runSound)
        {
            if (rayCastGrounded)
            { //for walk sounsd using this because suraface is not straigh			
                if (currentSpeed > 1)
                {
                    //				print ("unutra sam");
                    if (maxMoveSpeed == 3)
                    {
                        //	print ("tu sem");
                        if (!_walkSound.isPlaying)
                        {
                            //	print ("playam hod");
                            _walkSound.Play();
                            _runSound.Stop();
                        }
                    }
                    else if (maxMoveSpeed == 5)
                    {
                        //	print ("NE tu sem");

                        if (!_runSound.isPlaying)
                        {
                            _walkSound.Stop();
                            _runSound.Play();
                        }
                    }
                }
                else
                {
                    _walkSound.Stop();
                    _runSound.Stop();
                }
            }
            else
            {
                _walkSound.Stop();
                _runSound.Stop();
            }
        }
        else
        {
            print("Missing walk and running sounds.");
        }

    }


    /*
	* If player toggle the crouch it will scale the player to appear that is crouching
	*/
    void Crouching()
    {
        if (Input.GetKey(KeyCode.C))
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 0.6f, 1), Time.deltaTime * 15);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 1, 1), Time.deltaTime * 15);
        }
    }


    //[Tooltip("The higher the number the faster it will stop")]
    //public float deaccelerationSpeed = 15.0f;


    //[Tooltip("Force that is applied when moving forward or backward")]
    //public float accelerationSpeed = 50000.0f;


    [Tooltip("Tells us whether the player is grounded or not (by using another script)")]
    [SerializeField]
    private bool grounded;

    [Tooltip("Tells us whether the player is grounded or not (by using raycast from another script)")]
    [SerializeField]
    private bool rayCastGrounded;






    RaycastHit hitInfo;
    private float meleeAttack_cooldown;
    private string currentWeapo;
    [Tooltip("Put 'Player' layer here")]
    [Header("Shooting Properties")]
    private LayerMask ignoreLayer;//to ignore player layer
    Ray ray1, ray2, ray3, ray4, ray5, ray6, ray7, ray8, ray9;
    private float rayDetectorMeeleSpace = 0.15f;
    private float offsetStart = 0.05f;
    [Tooltip("Put BulletSpawn gameobject here, palce from where bullets are created.")]
    [HideInInspector]
    public Transform bulletSpawn; //from here we shoot a ray to check where we hit him;
    /*
	* This method casts 9 rays in different directions. ( SEE scene tab and you will see 9 rays differently coloured).
	* Used to widley detect enemy infront and increase meele hit detectivity.
	* Checks for cooldown after last preformed meele attack.
	*/


    public bool been_to_meele_anim = false;
    private void RaycastForMeleeAttacks()
    {




        if (meleeAttack_cooldown > -5)
        {
            meleeAttack_cooldown -= 1 * Time.deltaTime;
        }


        if (GetComponent<GunInventoryCC>().currentGun)
        {
            if (GetComponent<GunInventoryCC>().currentGun.GetComponent<GunCCScript>())
                currentWeapo = "gun";
        }

        //middle row
        ray1 = new Ray(bulletSpawn.position + (bulletSpawn.right * offsetStart), bulletSpawn.forward + (bulletSpawn.right * rayDetectorMeeleSpace));
        ray2 = new Ray(bulletSpawn.position - (bulletSpawn.right * offsetStart), bulletSpawn.forward - (bulletSpawn.right * rayDetectorMeeleSpace));
        ray3 = new Ray(bulletSpawn.position, bulletSpawn.forward);
        //upper row
        ray4 = new Ray(bulletSpawn.position + (bulletSpawn.right * offsetStart) + (bulletSpawn.up * offsetStart), bulletSpawn.forward + (bulletSpawn.right * rayDetectorMeeleSpace) + (bulletSpawn.up * rayDetectorMeeleSpace));
        ray5 = new Ray(bulletSpawn.position - (bulletSpawn.right * offsetStart) + (bulletSpawn.up * offsetStart), bulletSpawn.forward - (bulletSpawn.right * rayDetectorMeeleSpace) + (bulletSpawn.up * rayDetectorMeeleSpace));
        ray6 = new Ray(bulletSpawn.position + (bulletSpawn.up * offsetStart), bulletSpawn.forward + (bulletSpawn.up * rayDetectorMeeleSpace));
        //bottom row
        ray7 = new Ray(bulletSpawn.position + (bulletSpawn.right * offsetStart) - (bulletSpawn.up * offsetStart), bulletSpawn.forward + (bulletSpawn.right * rayDetectorMeeleSpace) - (bulletSpawn.up * rayDetectorMeeleSpace));
        ray8 = new Ray(bulletSpawn.position - (bulletSpawn.right * offsetStart) - (bulletSpawn.up * offsetStart), bulletSpawn.forward - (bulletSpawn.right * rayDetectorMeeleSpace) - (bulletSpawn.up * rayDetectorMeeleSpace));
        ray9 = new Ray(bulletSpawn.position - (bulletSpawn.up * offsetStart), bulletSpawn.forward - (bulletSpawn.up * rayDetectorMeeleSpace));

        Debug.DrawRay(ray1.origin, ray1.direction, Color.cyan);
        Debug.DrawRay(ray2.origin, ray2.direction, Color.cyan);
        Debug.DrawRay(ray3.origin, ray3.direction, Color.cyan);
        Debug.DrawRay(ray4.origin, ray4.direction, Color.red);
        Debug.DrawRay(ray5.origin, ray5.direction, Color.red);
        Debug.DrawRay(ray6.origin, ray6.direction, Color.red);
        Debug.DrawRay(ray7.origin, ray7.direction, Color.yellow);
        Debug.DrawRay(ray8.origin, ray8.direction, Color.yellow);
        Debug.DrawRay(ray9.origin, ray9.direction, Color.yellow);

        if (GetComponent<GunInventoryCC>().currentGun)
        {
            if (GetComponent<GunInventoryCC>().currentGun.GetComponent<GunCCScript>().meeleAttack == false)
            {
                been_to_meele_anim = false;
            }
            if (GetComponent<GunInventoryCC>().currentGun.GetComponent<GunCCScript>().meeleAttack == true && been_to_meele_anim == false)
            {
                been_to_meele_anim = true;
                //	if (isRunning == false) {
                StartCoroutine("MeeleAttackWeaponHit");
                //	}
            }
        }

    }

    /*
	 *Method that is called if the waepon hit animation has been triggered the first time via Q input
	 *and if is, it will search for target and make damage
	 */
    IEnumerator MeeleAttackWeaponHit()
    {
        if (Physics.Raycast(ray1, out hitInfo, 2f, ~ignoreLayer) || Physics.Raycast(ray2, out hitInfo, 2f, ~ignoreLayer) || Physics.Raycast(ray3, out hitInfo, 2f, ~ignoreLayer)
            || Physics.Raycast(ray4, out hitInfo, 2f, ~ignoreLayer) || Physics.Raycast(ray5, out hitInfo, 2f, ~ignoreLayer) || Physics.Raycast(ray6, out hitInfo, 2f, ~ignoreLayer)
            || Physics.Raycast(ray7, out hitInfo, 2f, ~ignoreLayer) || Physics.Raycast(ray8, out hitInfo, 2f, ~ignoreLayer) || Physics.Raycast(ray9, out hitInfo, 2f, ~ignoreLayer))
        {
            //Debug.DrawRay (bulletSpawn.position, bulletSpawn.forward + (bulletSpawn.right*0.2f), Color.green, 0.0f);
            if (hitInfo.transform.tag == "Dummie")
            {
                Transform _other = hitInfo.transform.root.transform;
                if (_other.transform.tag == "Dummie")
                {
                    print("hit a dummie");
                }
                InstantiateBlood(hitInfo, false);
            }
        }
        yield return new WaitForEndOfFrame();
    }

    [Header("BloodForMelleAttaacks")]
    RaycastHit hit;//stores info of hit;
    [Tooltip("Put your particle blood effect here.")]
    public GameObject bloodEffect;//blod effect prefab;
    /*
	* Upon hitting enemy it calls this method, gives it raycast hit info 
	* and at that position it creates our blood prefab.
	*/
    void InstantiateBlood(RaycastHit _hitPos, bool swordHitWithGunOrNot)
    {

        if (currentWeapo == "gun")
        {
            GunCCScript.HitMarkerSound();

            if (_hitSound)
                _hitSound.Play();
            else
                print("Missing hit sound");

            if (!swordHitWithGunOrNot)
            {
                if (bloodEffect)
                    Instantiate(bloodEffect, _hitPos.point, Quaternion.identity);
                else
                    print("Missing blood effect prefab in the inspector.");
            }
        }
    }
    private GameObject myBloodEffect;


    [Header("Player SOUNDS")]
    [Tooltip("Jump sound when player jumps.")]
    public AudioSource _jumpSound;
    [Tooltip("Sound while player makes when successfully reloads weapon.")]
    public AudioSource _freakingZombiesSound;
    [Tooltip("Sound Bullet makes when hits target.")]
    public AudioSource _hitSound;
    [Tooltip("Walk sound player makes.")]
    public AudioSource _walkSound;
    [Tooltip("Run Sound player makes.")]
    public AudioSource _runSound;
}
