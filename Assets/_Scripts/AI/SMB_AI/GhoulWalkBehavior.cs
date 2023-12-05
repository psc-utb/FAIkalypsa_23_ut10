using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhoulWalkBehavior : EnemyBehaviorBase
{

    private Waypoints wayPoints;
    public Waypoints WayPoints { get => wayPoints; set => wayPoints = value; }

    Transform currentWaypoint;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        WayPoints = animator.gameObject.GetComponent<Waypoints>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (WayPoints != null && wayPoints.WayPoints != null && wayPoints.WayPoints.Length > 0 && currentWaypoint == null
            && navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            int index = Random.Range(0, wayPoints.WayPoints.Length);
            currentWaypoint = wayPoints.WayPoints[index].transform;
            navMeshAgent.SetDestination(currentWaypoint.transform.position);
        }
        else if (currentWaypoint != null)
        {
            float distanceFromWaypoint = Vector3.Distance(animator.gameObject.transform.position, currentWaypoint.transform.position);
            float distanceFromDestination = Vector3.Distance(animator.gameObject.transform.position, navMeshAgent.destination);
            float distance = Mathf.Abs(distanceFromWaypoint - distanceFromDestination);
            if (distanceFromWaypoint < 0.5 || distance > 1)
            {
                currentWaypoint = null;
                animator.SetTrigger("WaypointReached");
            }
        }
        navMeshAgent.speed = Speed;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
