using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public int[,] level;
    public int levelWidth;
    public int levelHeight;
    public float startX;
    public float startY;
    public Vector3 posOffset;

    private void Start()
    {
        // Default grid layout, in case the Level Generator shits itself
        if (level == null)
        {
            level = new int[,]
            {
                { 1,2,2,2,2,2,2,2,2,2,2,2,2,7},
                { 2,5,5,5,5,5,5,5,5,5,5,5,5,4},
                { 2,5,3,4,4,3,5,3,4,4,4,3,5,4},
                { 2,6,4,0,0,4,5,4,0,0,0,4,5,4},
                { 2,5,3,4,4,3,5,3,4,4,4,3,5,3},
                { 2,5,5,5,5,5,5,5,5,5,5,5,5,5},
                { 2,5,3,4,4,3,5,3,3,5,3,4,4,4},
                { 2,5,3,4,4,3,5,4,4,5,3,4,4,3},
                { 2,5,5,5,5,5,5,4,4,5,5,5,5,4},
                { 1,2,2,2,2,1,5,4,3,4,4,3,0,4},
                { 0,0,0,0,0,2,5,4,3,4,4,3,0,3},
                { 0,0,0,0,0,2,5,4,4,0,0,0,0,0},
                { 0,0,0,0,0,2,5,4,4,0,3,4,4,0},
                { 2,2,2,2,2,1,5,3,3,0,4,0,0,0},
                { 0,0,0,0,0,0,5,0,0,0,4,0,0,0},
            };
            CreateLevelArrayFromQuarter(level);

        }
    }

    public void CreateLevelArrayFromQuarter(int[,] levelMap)
    {
        // Get size of corner template
        int mapHeight = levelMap.GetLength(0);
        int mapWidth = levelMap.GetLength(1);

        // Create larger level out of that
        level = new int[mapWidth * 2, (mapHeight * 2) - 1];
        levelWidth = level.GetLength(0);
        levelHeight = level.GetLength(1);

        // For each row...
        for (int y = 0; y < mapHeight; y++)
        {
            // For each cell...
            for (int x = 0; x < mapWidth; x++)
            {
                // Top left
                level[x, y] = levelMap[y, x];

                // Top right
                level[levelWidth - x - 1, y] = levelMap[y, x];

                if (y != levelHeight - 1)
                {
                    // Bottom left
                    level[x, levelHeight - y - 1] = levelMap[y, x];

                    // Bottom right
                    level[levelWidth - x - 1, levelHeight - y - 1] = levelMap[y, x];
                }
            }
        }

        // Get actual Unity coordinates of each cell
        startX = ((float)mapWidth * -1f) + 0.5f + posOffset.x;
        startY = (float)mapHeight - 0.5f + posOffset.y;
    }

    public Vector2 GetCoordinates(Vector3 pos)
    {
        Vector2 coords = new Vector2(0f, 0f);


        return coords;
    }

    public int GetKey(int x, int y)
    {
        if (0 <= x && x < levelWidth &&
            0 <= y && y < levelHeight)
        {
            return level[x, y];
        }
        return 0;
    }

    public bool IsPellet(int i)
    {
        return i == 5 || i == 6;
    }

    public bool IsInner(int i)
    {
        return i == 3 || i == 4;
    }

    public bool IsOuter(int i)
    {
        return i == 1 || i == 2;
    }

    public bool IsCorner(int i)
    {
        return i == 1 || i == 3;
    }

    public bool IsEdge(int i)
    {
        return i == 2 || i == 4;
    }

    public bool IsJunction(int i)
    {
        return i == 7;
    }

    public bool IsWall(int i)
    {
        return IsCorner(i) || IsEdge(i) || IsJunction(i);
    }
}
