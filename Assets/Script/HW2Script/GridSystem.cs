using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

public class GridSystem : MonoBehaviour
{
    public Vector3 gridWorldSize;
    public float nodeRadius;
    public LayerMask unwalkableMask;
    public int gridHeight = 3; 

    private NodeGrid[,,] grid;
    private float nodeDiameter;
    private int gridSizeX, gridSizeY, gridSizeZ;

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        gridSizeZ = gridHeight;
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new NodeGrid[gridSizeX, gridSizeY, gridSizeZ];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2 - Vector3.up * gridWorldSize.z / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius) + Vector3.up * (z * nodeDiameter + nodeRadius);
                    bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                    grid[x, y, z] = new NodeGrid(walkable, worldPoint, x, y, z);
                }
            }
        }
    }

    public NodeGrid NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x - (transform.position.x - gridWorldSize.x / 2)) / gridWorldSize.x;
        float percentY = (worldPosition.z - (transform.position.z - gridWorldSize.y / 2)) / gridWorldSize.y;
        float percentZ = (worldPosition.y - (transform.position.y - gridWorldSize.z / 2)) / gridWorldSize.z;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        percentZ = Mathf.Clamp01(percentZ);

        int x = Mathf.FloorToInt((gridSizeX - 1) * percentX);
        int y = Mathf.FloorToInt((gridSizeY - 1) * percentY);
        int z = Mathf.FloorToInt((gridSizeZ - 1) * percentZ);

        Debug.Log($"World Position: {worldPosition}, Translated Grid Indices: ({x}, {y}, {z})");

        return grid[x, y, z];
    }


    public List<NodeGrid> GetNeighbors(NodeGrid node)
    {
        List<NodeGrid> neighbors = new List<NodeGrid>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && y == 0 && z == 0)
                        continue;

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;
                    int checkZ = node.gridZ + z;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && checkZ >= 0 && checkZ < gridSizeZ)
                    {
                        neighbors.Add(grid[checkX, checkY, checkZ]);
                    }
                }
            }
        }

        return neighbors;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.z, gridWorldSize.y));

        if (grid != null)
        {
            foreach (NodeGrid node in grid)
            {
                Gizmos.color = (node.walkable) ? Color.white : Color.red;
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }
}

public class NodeGrid
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;
    public int gridZ;

    public int gCost;
    public int hCost;
    public NodeGrid parent;

    public NodeGrid(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _gridZ)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        gridZ = _gridZ;
    }

    public int fCost
    {
        get { return gCost + hCost; }
    }
}
