using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public GameObject floorPrefab;

    private MazeGenerator mazeGenerator;
    private int width, height;
    private int[,] maze;

    private Vector2Int start;
    private Vector2Int end;
    private List<Vector2Int> path;

    private void Start()
    {
        StartCoroutine(WaitForMazeGeneration());
    }

    private IEnumerator WaitForMazeGeneration()
    {
        mazeGenerator = GetComponent<MazeGenerator>();
        while (mazeGenerator.Maze == null)
        {
            yield return null;
        }

        width = mazeGenerator.width;
        height = mazeGenerator.height;
        maze = mazeGenerator.Maze;

        start = FindStartPoint();
        end = FindEndPoint();

        path = FindPath(start, end);
        if (path != null)
        {
            DrawPath(path);
        }
        else
        {
            Debug.Log("No path found");
        }
    }

    Vector2Int FindStartPoint()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (maze[x, y] == 0)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return Vector2Int.zero; 
    }

    Vector2Int FindEndPoint()
    {
        for (int x = width - 1; x >= 0; x--)
        {
            for (int y = height - 1; y >= 0; y--)
            {
                if (maze[x, y] == 0)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return Vector2Int.zero; 
    }

    List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
    {
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
        PriorityQueue<Node> openSet = new PriorityQueue<Node>();

        Node startNode = new Node(start, null, 0, GetHeuristic(start, end));
        openSet.Enqueue(startNode);

        while (openSet.Count > 0)
        {
            Node current = openSet.Dequeue();
            if (current.Position == end)
            {
                return ReconstructPath(current);
            }

            closedSet.Add(current.Position);

            foreach (Vector2Int direction in GetDirections())
            {
                Vector2Int neighborPos = current.Position + direction;
                if (IsInMaze(neighborPos) && maze[neighborPos.x, neighborPos.y] == 0 && !closedSet.Contains(neighborPos))
                {
                    float tentativeGScore = current.G + 1;
                    Node neighborNode = new Node(neighborPos, current, tentativeGScore, GetHeuristic(neighborPos, end));
                    if (!openSet.Contains(neighborNode))
                    {
                        openSet.Enqueue(neighborNode);
                    }
                }
            }
        }

        return null; 
    }

    List<Vector2Int> ReconstructPath(Node node)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        while (node != null)
        {
            path.Add(node.Position);
            node = node.Parent;
        }
        path.Reverse();
        return path;
    }

    float GetHeuristic(Vector2Int a, Vector2Int b)
    {
        return Vector2Int.Distance(a, b);
    }

    bool IsInMaze(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    Vector2Int[] GetDirections()
    {
        return new Vector2Int[] {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };
    }

    void DrawPath(List<Vector2Int> path)
    {
        foreach (Vector2Int pos in path)
        {
            Instantiate(floorPrefab, new Vector3(pos.x, 0.1f, pos.y), Quaternion.identity);
        }
    }

    private void OnDrawGizmos()
    {
        if (path != null)
        {
            Gizmos.color = Color.red;
            foreach (Vector2Int pos in path)
            {
                Gizmos.DrawSphere(new Vector3(pos.x, 0.5f, pos.y), 0.2f);
            }
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(new Vector3(start.x, 0.5f, start.y), 0.3f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(new Vector3(end.x, 0.5f, end.y), 0.3f);
    }

    class Node : IComparable<Node>
    {
        public Vector2Int Position;
        public Node Parent;
        public float G; 
        public float F; 

        public Node(Vector2Int position, Node parent, float g, float h)
        {
            Position = position;
            Parent = parent;
            G = g;
            F = g + h;
        }

        public int CompareTo(Node other)
        {
            return F.CompareTo(other.F);
        }
    }
}
