using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    NavMeshAgent _agent;
    Animator _animator;

    int waypointIndex;
    Vector3 target;
    bool isMoving = false;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        UpdateDestination();
    }

    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, target) < 1 && isMoving)
        {
            isMoving = false;
            _animator.SetBool("isMoving", isMoving);
            StartCoroutine(MoveAround());
        }
    }

    void UpdateDestination()
    {
        isMoving = true;
        _animator.SetBool("isMoving", isMoving);
        target = waypoints[waypointIndex].position;
        _agent.SetDestination(target);
    }

    void SetWaypoint()
    {
        waypointIndex++;
        if (waypointIndex == waypoints.Length)
        {
            waypointIndex = 0;
        }
    }

    IEnumerator MoveAround()
    {
        yield return new WaitForSeconds(1f);
        yield return null;

        SetWaypoint();
        UpdateDestination();
        yield break;
    }
}
