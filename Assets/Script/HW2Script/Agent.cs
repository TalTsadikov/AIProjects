using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{

    public float speed = 5f;
    public float acceleration = 2f;
    public float turningRadius = 1f;

    private PathFollower pathFollower;
    private Vector3 currentVelocity;

    void Start()
    {
        pathFollower = GetComponent<PathFollower>();
    }

    void Update()
    {
        if (!pathFollower.ReachedEndOfPath())
        {
            Vector3 targetPosition = pathFollower.GetNextWaypoint();
            Vector3 desiredVelocity = (targetPosition - transform.position).normalized * speed;
            currentVelocity = Vector3.MoveTowards(currentVelocity, desiredVelocity, acceleration * Time.deltaTime);

            transform.position += currentVelocity * Time.deltaTime;

            if (Vector3.Distance(transform.position, targetPosition) < turningRadius)
            {
                pathFollower.AdvanceToNextWaypoint();
            }
        }
    }
}
