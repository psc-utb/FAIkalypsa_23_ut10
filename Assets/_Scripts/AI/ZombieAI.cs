using Assets.Pool_Spawner_Pro_Upgrade;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class ZombieAI : MonoBehaviour, IReinitializable
{
    [SerializeField]
    private GameObject Player;
    HealthScript playerHealthScript;

    [SerializeField]
    private GameObject hands;

    Animator animator;
    NavMeshAgent navMeshAgent;

    HealthScript healthScript;

    bool dead = false;


    [SerializeField]
    float speedIdle = 0;

    [SerializeField]
    float speedWalk = 1.0f;

    [SerializeField]
    float speedRun = 3.5f;


    public UnityEvent<GameObject, bool> ZombieDied;


    CapsuleCollider capsule;

    float capsuleHeight;
    Vector3 capsuleCenter;


    public UnityEvent<GameObject> ZombieSpawned;


    void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        healthScript = gameObject.GetComponent<HealthScript>();

        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

        capsule = gameObject.GetComponent<CapsuleCollider>();
        capsuleHeight = capsule.height;
        capsuleCenter = capsule.center;

        playerHealthScript = Player.GetComponent<HealthScript>();
    }

    void Start()
    {
        if (animator != null)
        {
            GhoulIdleBehavior ghoulIdleBehavior = animator.GetBehaviour<GhoulIdleBehavior>();
            if (ghoulIdleBehavior != null)
            {
                ghoulIdleBehavior.Player = Player;
                ghoulIdleBehavior.Speed = speedIdle;
            }

            GhoulWalkBehavior ghoulWalkBehavior = animator.GetBehaviour<GhoulWalkBehavior>();
            if (ghoulWalkBehavior != null)
            {
                ghoulWalkBehavior.Player = Player;
                ghoulWalkBehavior.Speed = speedWalk;
            }

            GhoulRunBehavior ghoulRunBehavior = animator.GetBehaviour<GhoulRunBehavior>();
            if (ghoulRunBehavior != null)
            {
                ghoulRunBehavior.Player = Player;
                ghoulRunBehavior.Speed = speedRun;
            }

            GhoulAttackBehavior ghoulAttackBehavior = animator.GetBehaviour<GhoulAttackBehavior>();
            if (ghoulAttackBehavior != null)
            {
                ghoulAttackBehavior.Player = Player;
                ghoulAttackBehavior.Hands = hands;
            }
        }

    }

    void SetZombieBehavior(EnemyBehaviorBase zombie, AudioClip[] audioClips)
    {
        if (zombie != null)
        {
            zombie.Player = Player;
            //zombie.Hands = hands;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Player != null && dead == false)
        {
            float distance = Vector3.Distance(Player.transform.position, transform.position);
            animator.SetFloat("PlayerDistance", distance);


            
            //simple eyes
            Vector3 position = this.transform.position;
            position.y += 1.5f;
            Ray ray = new Ray(position, transform.TransformDirection(Vector3.forward));
            RaycastHit rayCastHit = new RaycastHit();
            Physics.Raycast(ray, out rayCastHit, 10.0f);
            //if (Physics.SphereCast(ray, 2f, out rayCastHit, 10))
            if (rayCastHit.collider != null && rayCastHit.collider.gameObject == Player)
            {
                animator.SetBool("PlayerVisible", true);
            }

            Debug.DrawRay(position, transform.TransformDirection(Vector3.forward) * rayCastHit.distance, Color.yellow);
            


            bool playerIsAlive = true;
            if (playerHealthScript != null)
            {
                playerIsAlive = playerHealthScript.IsAlive();
            }
            animator.SetBool("PlayerAlive", playerIsAlive);


            if (healthScript?.IsAlive() == false)
            {
                dead = true;
                animator.SetTrigger("Dead");

                navMeshAgent.baseOffset = -2;
                navMeshAgent.ResetPath();
                navMeshAgent.speed = 0;
                navMeshAgent.enabled = false;

                capsule.height = 0.5f;
                capsule.center = new Vector3(0, 2, 0);

                ZombieDied?.Invoke(this.gameObject, true);
            }
        }
    }


    public void PlayerVisible(bool isVisible)
    {
        animator.SetBool("PlayerVisible", isVisible);
    }


    public void Dead()
    {
        //just for demonstration of Animation event -> respawn can be called earlier than the animation event is called - in that case, the following code will not run.
        //ZombieDied?.Invoke(this.gameObject, true);
    }


    public void Reinitialization()
    {
        healthScript.Health = healthScript.Max_Health;

        capsule.height = capsuleHeight;
        capsule.center = capsuleCenter;

        navMeshAgent.baseOffset = 0;
        navMeshAgent.enabled = true;

        //just to remove warning in Unity editor about animator setting while gameobject is inactive
        gameObject.SetActive(true);

        animator.ResetTrigger("Dead");
        animator.SetBool("PlayerVisible", false);
        animator.SetFloat("PlayerDistance", float.PositiveInfinity);
        animator.SetBool("PlayerAlive", playerHealthScript.IsAlive());
        animator.Play("Idle", 0, 0);

        dead = false;


        ZombieSpawned?.Invoke(this.gameObject);
    }
}
