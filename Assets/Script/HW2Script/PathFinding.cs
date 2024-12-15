using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GridSystem;

public class PathFinding : MonoBehaviour
{

    private GridSystem grid;
    private List<Vector3> currentPath;

    void Awake()
    {
        grid = GetComponent<GridSystem>();
    }

    public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        NodeGrid startNode = grid.NodeFromWorldPoint(startPos);
        NodeGrid targetNode = grid.NodeFromWorldPoint(targetPos);

        List<NodeGrid> openSet = new List<NodeGrid>();
        HashSet<NodeGrid> closedSet = new HashSet<NodeGrid>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            NodeGrid currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                currentPath = RetracePath(startNode, targetNode);
                return currentPath;
            }

            foreach (NodeGrid neighbor in grid.GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null;
    }

    List<Vector3> RetracePath(NodeGrid startNode, NodeGrid endNode)
    {
        List<Vector3> path = new List<Vector3>();
        NodeGrid currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.worldPosition);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        return path;
    }

    int GetDistance(NodeGrid nodeA, NodeGrid nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        int dstZ = Mathf.Abs(nodeA.gridZ - nodeB.gridZ);

        int remaining = Mathf.Abs(dstX - dstY);
        return 14 * Mathf.Min(dstX, dstY) + 10 * remaining + 10 * dstZ;
    }

    void OnDrawGizmos()
    {
        if (currentPath != null)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < currentPath.Count - 1; i++)
            {
                Gizmos.DrawLine(currentPath[i], currentPath[i + 1]);
            }
        }
    }
}
