
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridPathfinding
{
    private readonly Tilemap floorTilemap;
    private readonly Tilemap wallsTilemap;

    private static readonly Vector3Int[] directions =
    {
        new Vector3Int(0, 1, 0),  // Вверх
        new Vector3Int(0, -1, 0), // Вниз
        new Vector3Int(-1, 0, 0), // Влево
        new Vector3Int(1, 0, 0)   // Вправо
    };

    private class Node
    {
        public Vector3Int Position;
        public int GCost;
        public int HCost;
        public int FCost => GCost + HCost;
        public Node Parent;

        public Node(Vector3Int position)
        {
            Position = position;
            GCost = int.MaxValue;
        }
    }

    public GridPathfinding(Tilemap floor, Tilemap walls)
    {
        floorTilemap = floor;
        wallsTilemap = walls;
    }

    public bool IsWalkable(Vector3Int position)
    {
        return floorTilemap.cellBounds.Contains(position)
            && floorTilemap.HasTile(position)
            && !wallsTilemap.HasTile(position);
    }

    public List<Vector3Int> FindPath(
        Vector3Int start,
        Vector3Int target)
    {
        if (!IsWalkable(start) || !IsWalkable(target))
            return null;

        Dictionary<Vector3Int, Node> nodes = new();
        List<Node> openList = new();
        HashSet<Vector3Int> closedSet = new();

        Node startNode = new Node(start);
        startNode.GCost = 0;
        startNode.HCost = GetDistance(start, target);

        nodes.Add(start, startNode);
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = GetLowestCostNode(openList);

            if (currentNode.Position == target)
                return RetracePath(currentNode);

            openList.Remove(currentNode);
            closedSet.Add(currentNode.Position);

            foreach (Vector3Int direction in directions)
            {
                Vector3Int neighborPosition =
                    currentNode.Position + direction;

                if (!IsWalkable(neighborPosition)
                    || closedSet.Contains(neighborPosition))
                    continue;

                if (!nodes.TryGetValue(
                    neighborPosition, out Node neighborNode))
                {
                    neighborNode = new Node(neighborPosition);
                    nodes.Add(neighborPosition, neighborNode);
                }

                int newCost = currentNode.GCost + 1;

                if (newCost >= neighborNode.GCost)
                    continue;

                neighborNode.GCost = newCost;
                neighborNode.HCost =
                    GetDistance(neighborPosition, target);
                neighborNode.Parent = currentNode;

                if (!openList.Contains(neighborNode))
                    openList.Add(neighborNode);
            }
        }

        return null;
    }

    private Node GetLowestCostNode(List<Node> nodes)
    {
        Node bestNode = nodes[0];

        foreach (Node node in nodes)
        {
            if (node.FCost < bestNode.FCost ||
                (node.FCost == bestNode.FCost &&
                 node.HCost < bestNode.HCost))
            {
                bestNode = node;
            }
        }

        return bestNode;
    }

    private int GetDistance(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) +
               Mathf.Abs(a.y - b.y);
    }

    private List<Vector3Int> RetracePath(Node endNode)
    {
        List<Vector3Int> path = new();

        Node currentNode = endNode;

        while (currentNode != null)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }
}
