using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        Debug.Log($"Start Node: World Position {startNode.worldPosition}, Grid ({startNode.gridX}, {startNode.gridY}, {startNode.gridZ}), Walkable: {startNode.walkable}");
        Debug.Log($"Target Node: World Position {targetNode.worldPosition}, Grid ({targetNode.gridX}, {targetNode.gridY}, {targetNode.gridZ}), Walkable: {targetNode.walkable}");

        if (!startNode.walkable || !targetNode.walkable)
        {
            Debug.LogWarning("Start or target node is not walkable!");
            return null;
        }

        List<NodeGrid> openSet = new List<NodeGrid>();
        HashSet<NodeGrid> closedSet = new HashSet<NodeGrid>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            NodeGrid currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
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
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        Debug.LogWarning("Path not found!");
        return null;
    }


    private List<Vector3> RetracePath(NodeGrid startNode, NodeGrid endNode)
    {
        List<Vector3> path = new List<Vector3>();
        NodeGrid currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.worldPosition);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        currentPath = path;

        Debug.Log($"Retraced Path: {string.Join(" -> ", path)}");

        return path;
    }

    private int GetDistance(NodeGrid nodeA, NodeGrid nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        int distZ = Mathf.Abs(nodeA.gridZ - nodeB.gridZ);

        if (distX > distY && distX > distZ)
        {
            return 14 * Mathf.Min(distY, distZ) + 10 * (distX - Mathf.Min(distY, distZ));
        }
        else if (distY > distX && distY > distZ)
        {
            return 14 * Mathf.Min(distX, distZ) + 10 * (distY - Mathf.Min(distX, distZ));
        }
        else
        {
            return 14 * Mathf.Min(distX, distY) + 10 * (distZ - Mathf.Min(distX, distY));
        }
    }

    void OnDrawGizmos()
    {
        if (currentPath != null)
        {
            Gizmos.color = Color.cyan;
            foreach (Vector3 point in currentPath)
            {
                Gizmos.DrawSphere(point, 0.2f);
            }
        }
    }
}
