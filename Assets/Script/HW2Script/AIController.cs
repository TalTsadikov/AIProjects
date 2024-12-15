using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{

        public Transform target;
        private PathFinding pathfinding;
        private PathFollower pathFollower;
        private Agent agent;
        private List<Vector3> currentPath;

    void Start()
        {
            pathfinding = FindObjectOfType<PathFinding>();
            pathFollower = GetComponent<PathFollower>();
            agent = GetComponent<Agent>();
            FindNewPath();
        }

        void Update()
        {
            if (pathFollower.ReachedEndOfPath())
            {
                FindNewPath();
            }
        }

        void FindNewPath()
        {
            Vector3 targetPosition = target.position;
            List<Vector3> newPath = pathfinding.FindPath(transform.position, targetPosition);
            if (newPath != null)
            {
                pathFollower.SetPath(newPath);
            }
        }

        void OnDrawGizmos()
        {
            if (currentPath != null)
            {
                Gizmos.color = Color.blue;
                for (int i = 0; i < currentPath.Count - 1; i++)
                {
                    Gizmos.DrawLine(currentPath[i], currentPath[i + 1]);
                }
            }
        }
}
