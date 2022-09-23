using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    enum Dir
    {
        E,
        NE,
        N,
        NW,
        W,
        SW,
        S,
        SE
    }

    // ------- NOTE -------
    // Coordinates are (y,x) NOT (x,y)
    private int[,] levelMap =
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

    [SerializeField] private GameObject wallInnerCorner;
    [SerializeField] private GameObject wallInnerEdge;
    [SerializeField] private GameObject wallOuterCorner;
    [SerializeField] private GameObject wallOuterEdge;
    [SerializeField] private GameObject pellet;
    [SerializeField] private GameObject powerPellet;
    [SerializeField] private GameObject wallOuterJunction;

    [SerializeField] private Vector3 posOffset;

    [SerializeField] private GameObject manualMap;


    // Start is called before the first frame update
    void Start()
    {
        // Get rid of the manual map. Cast away hard labour in favour of even harder labour.
        Object.Destroy(manualMap);

        // Get size of corner template
        int mapHeight = levelMap.GetLength(0);
        int mapWidth = levelMap.GetLength(1);

        // Create larger level out of that
        int[,] level = new int[mapWidth * 2, (mapHeight * 2) - 1];
        int levelWidth = level.GetLength(0);
        int levelHeight = level.GetLength(1);

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

        // ------- NOTE -------
        // Coordinates are now in the correct orientation, (x,y)

        float startX = ((float)mapWidth * -1f) + 0.5f + posOffset.x;
        float startY = (float)mapHeight - 0.5f + posOffset.y;

        // FOR EVERY CELL
        for (int x = 0; x < levelWidth; x++)
        {
            for (int y = 0; y < levelHeight; y++)
            {
                // Get the ID of the piece we are going to place
                int key = level[x, y];

                // Check it isn't empty
                if (key != 0)
                {
                    bool[] boundary = GetBoundaries(levelWidth, levelHeight, x, y);
                    Quaternion rot = Quaternion.identity;

                    // Check it isn't a pellet
                    if (!isPellet(key))
                    {
                        // check for edge
                        if (isEdge(key))
                        {
                            // edges are horizontal by default.
                            // so we must only check for vertical edges
                            if (CheckWallDir(boundary, (int)Dir.N, level, x, y) &&
                                CheckWallDir(boundary, (int)Dir.S, level, x, y))
                            {
                                rot = Quaternion.Euler(0f, 0f, 90f);
                            }
                        }

                        // check for corner
                        else if (isCorner(key))
                        {
                            // Corners have this branching path shit.
                            // If there's a wall in one direction, but not to one perpedicular side...
                            // ...then it must be facing the other side.

                            if (CheckWallDir(boundary, (int)Dir.N, level, x, y) &&
                                CheckWallDir(boundary, (int)Dir.W, level, x, y) &&
                                !CheckWallDir(boundary, (int)Dir.E, level, x, y))
                            {
                                // NW
                                rot = Quaternion.Euler(0f, 0f, 180f);
                            }
                            else
                            if (CheckWallDir(boundary, (int)Dir.N, level, x, y) &&
                                CheckWallDir(boundary, (int)Dir.E, level, x, y) &&
                                !CheckWallDir(boundary, (int)Dir.W, level, x, y))
                            {
                                // NE
                                rot = Quaternion.Euler(0f, 0f, 90f);
                            }
                            else
                            if (CheckWallDir(boundary, (int)Dir.S, level, x, y) &&
                                CheckWallDir(boundary, (int)Dir.W, level, x, y) &&
                                !CheckWallDir(boundary, (int)Dir.E, level, x, y))
                            {
                                // SW
                                rot = Quaternion.Euler(0f, 0f, 270f);
                            }
                            else
                            if (CheckWallDir(boundary, (int)Dir.S, level, x, y) &&
                                CheckWallDir(boundary, (int)Dir.E, level, x, y) &&
                                !CheckWallDir(boundary, (int)Dir.W, level, x, y))
                            {
                                // SE
                                rot = Quaternion.Euler(0f, 0f, 0f);
                            }
                            else
                            if (CheckWallDir(boundary, (int)Dir.N, level, x, y) &&
                                CheckWallDir(boundary, (int)Dir.W, level, x, y) &&
                                !CheckWallDir(boundary, (int)Dir.NW, level, x, y))
                            {
                                // NW
                                rot = Quaternion.Euler(0f, 0f, 180f);
                            }
                            else
                            if (CheckWallDir(boundary, (int)Dir.N, level, x, y) &&
                                CheckWallDir(boundary, (int)Dir.E, level, x, y) &&
                                !CheckWallDir(boundary, (int)Dir.NE, level, x, y))
                            {
                                // NE
                                rot = Quaternion.Euler(0f, 0f, 90f);
                            }
                            else
                            if (CheckWallDir(boundary, (int)Dir.S, level, x, y) &&
                                CheckWallDir(boundary, (int)Dir.W, level, x, y) &&
                                !CheckWallDir(boundary, (int)Dir.SW, level, x, y))
                            {
                                // SW
                                rot = Quaternion.Euler(0f, 0f, 270f);
                            }
                            else
                            if (CheckWallDir(boundary, (int)Dir.S, level, x, y) &&
                                CheckWallDir(boundary, (int)Dir.E, level, x, y) &&
                                !CheckWallDir(boundary, (int)Dir.SE, level, x, y))
                            {
                                // SE
                                rot = Quaternion.Euler(0f, 0f, 0f);
                            }


                        }
                        else if (isJunction(key))
                        {
                            if (!boundary[(int)Dir.S])
                            {
                                rot = Quaternion.Euler(0f, 0f, 180f);
                            }
                            else
                            if (!boundary[(int)Dir.E])
                            {
                                rot = Quaternion.Euler(0f, 0f, 90f);
                            }
                            else
                            if (!boundary[(int)Dir.W])
                            {
                                rot = Quaternion.Euler(0f, 0f, 270f);
                            }
                        }
                    }

                    Instantiate(GetPrefabFromKey(key), new Vector3(startX + x, startY - y, posOffset.z), rot, transform);
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool[] GetBoundaries(int levelWidth, int levelHeight, int x, int y)
    {
        // Returns whether or not each of these neighbours exists
        // E, NE, N, NW, W, SW, S, SE
        bool[] arr = {true, true, true, true, true, true, true, true};

        if (x == levelWidth - 1)
        {
            arr[1] = false;
            arr[0] = false;
            arr[7] = false;
        }
        if (y == 0)
        {
            arr[1] = false;
            arr[2] = false;
            arr[3] = false;
        }
        if (x == 0)
        {
            arr[3] = false;
            arr[4] = false;
            arr[5] = false;
        }
        if (y == levelHeight - 1)
        {
            arr[5] = false;
            arr[6] = false;
            arr[7] = false;
        }

        return arr;
    }

    bool CheckWallDir(bool[] boundary, int dir, int[,] level, int x, int y)
    {
        Debug.Log("(" + x + " , " + y + ") : " + dir + " is " + boundary[dir]);
        if (boundary[dir])
        {
            switch (dir)
            {
                case (int)Dir.E:
                    return isWall(level[x + 1, y]);
                case (int)Dir.NE:
                    return isWall(level[x + 1, y - 1]);
                case (int)Dir.N:
                    return isWall(level[x, y - 1]);
                case (int)Dir.NW:
                    return isWall(level[x - 1, y - 1]);
                case (int)Dir.W:
                    return isWall(level[x - 1, y]);
                case (int)Dir.SW:
                    return isWall(level[x - 1, y + 1]);
                case (int)Dir.S:
                    return isWall(level[x, y + 1]);
                case (int)Dir.SE:
                    return isWall(level[x + 1, y + 1]);
            }
        }

        return false;
    }

    GameObject GetPrefabFromKey(int key)
    {
        switch (key)
        {
            case 1:
                return wallOuterCorner;
            case 2:
                return wallOuterEdge;
            case 3:
                return wallInnerCorner;
            case 4:
                return wallInnerEdge;
            case 5:
                return pellet;
            case 6:
                return powerPellet;
            case 7:
                return wallOuterJunction;
        }
        return null;
    }
    bool isPellet(int i)
    {
        return i == 5 || i == 6;
    }

    bool isInner(int i)
    {
        return i == 3 || i == 4;
    }

    bool isOuter(int i)
    {
        return i == 1 || i == 2;
    }

    bool isCorner(int i)
    {
        return i == 1 || i == 3;
    }

    bool isEdge(int i)
    {
        return i == 2 || i == 4;
    }

    bool isJunction(int i)
    {
        return i == 7;
    }

    bool isWall(int i)
    {
        return isCorner(i) || isEdge(i) || isJunction(i);
    }

}
