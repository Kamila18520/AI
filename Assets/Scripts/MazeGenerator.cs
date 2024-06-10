using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public GameObject wallPrefab;
    public GameObject floorPrefab;


    public int[,] maze;

    public int[,] Maze => maze;

    void Start()
    {

        GenerateMaze();
        DrawMaze();
    }

    void GenerateMaze()
    {
        maze = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = 1; 
            }
        }

        DFS(1, 1);
    }

    void DFS(int x, int y)
    {
        int[] dx = { 0, 1, 0, -1 };
        int[] dy = { -1, 0, 1, 0 };
        maze[x, y] = 0; 

        for (int i = 0; i < 4; i++)
        {
            int rand = Random.Range(0, 4);
            int temp = dx[i];
            dx[i] = dx[rand];
            dx[rand] = temp;

            temp = dy[i];
            dy[i] = dy[rand];
            dy[rand] = temp;
        }

        for (int i = 0; i < 4; i++)
        {
            int nx = x + dx[i] * 2;
            int ny = y + dy[i] * 2;
            if (IsInMaze(nx, ny) && maze[nx, ny] == 1)
            {
                maze[x + dx[i], y + dy[i]] = 0;
                DFS(nx, ny);
            }
        }
    }

    bool IsInMaze(int x, int y)
    {
        return x > 0 && x < width - 1 && y > 0 && y < height - 1;
    }

    void DrawMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject prefab = maze[x, y] == 1 ? wallPrefab : floorPrefab;
                Instantiate(prefab, new Vector3(x, 0, y), Quaternion.identity);
            }
        }
    }

    public Vector2Int GetRandomPoint()
    {
        while (true)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            if (maze[x, y] == 0)
            {
                return new Vector2Int(x, y);
            }
        }
    }


}
