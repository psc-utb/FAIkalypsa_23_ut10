using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;


public class EnemyBehaviorBase : StateMachineBehaviour
{
    private GameObject player;
    public GameObject Player { get => player; set => player = value; }


    protected NavMeshAgent navMeshAgent;

    float speed;
    public float Speed { get => speed; set => speed = value; }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        navMeshAgent = animator.gameObject.GetComponent<NavMeshAgent>();
    }
}