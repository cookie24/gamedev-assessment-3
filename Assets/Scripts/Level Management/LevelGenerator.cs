using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

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

    [SerializeField] private LevelManager levelManager;
    [SerializeField] private GameManager gameManager;

    public bool customMap = false;


    // Start is called before the first frame update
    void Start()
    {
        // Load custom map (if we so desire)
        if (customMap)
        {
            levelMap = new int[,]
            {
                { 1,2,2,2,2,2,2,2,2,2,2,2,2,2},
                { 2,5,5,5,5,5,5,5,5,5,5,5,5,5},
                { 2,5,3,4,4,4,4,4,4,4,4,3,5,3},
                { 2,5,3,4,4,4,4,4,4,4,4,3,5,4},
                { 2,5,5,5,6,5,5,5,5,5,5,5,5,4},
                { 2,5,3,4,4,3,5,3,4,4,4,4,4,3},
                { 2,5,3,4,4,3,5,3,4,4,4,4,4,4},
                { 2,5,5,5,5,5,5,5,5,5,5,5,5,5},
                { 2,5,3,4,4,3,5,3,3,0,3,4,4,4},
                { 2,5,3,4,3,4,5,4,4,0,4,0,0,0},
                { 2,5,5,5,4,4,5,4,4,0,3,4,4,4},
                { 1,2,1,5,4,4,5,4,4,0,0,0,0,0},
                { 0,0,2,5,4,4,5,4,4,0,3,4,4,0},
                { 2,2,1,5,3,3,5,3,3,0,4,0,0,0},
                { 0,0,0,5,5,5,5,0,0,0,4,0,0,0},
            };
        }

        // Get rid of the manual map. Cast away hard labour in favour of even harder labour.
        Object.Destroy(manualMap);

        // Get size of corner template
        int mapHeight = levelMap.GetLength(0);
        int mapWidth = levelMap.GetLength(1);

        // Convert quarter map into full level.
        // Code is stored in LevelManager for flexibility's sake
        levelManager.CreateLevelArrayFromQuarter(levelMap);
        levelManager.posOffset = this.posOffset;

        // Get LevelManager's variables locally (temporarily)
        int[,] level = levelManager.level;
        int levelWidth = levelManager.levelWidth;
        int levelHeight = levelManager.levelHeight;

        float startX = levelManager.startX;
        float startY = levelManager.startY;

        // ------- NOTE -------
        // Coordinates are now in the correct orientation, (x,y)

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
                    if (!levelManager.IsPellet(key))
                    {
                        // check for edge
                        if (levelManager.IsEdge(key))
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
                        else if (levelManager.IsCorner(key))
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
                        else if (levelManager.IsJunction(key))
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

                    GameObject obj = Instantiate(GetPrefabFromKey(key), new Vector3(startX + x, startY - y, posOffset.z), rot, transform);
                    
                    // Add to pellet list
                    if (levelManager.IsPellet(key))
                    {
                        gameManager.pellets.Add(obj);
                    }
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
                    return levelManager.IsWall(level[x + 1, y]);
                case (int)Dir.NE:
                    return levelManager.IsWall(level[x + 1, y - 1]);
                case (int)Dir.N:
                    return levelManager.IsWall(level[x, y - 1]);
                case (int)Dir.NW:
                    return levelManager.IsWall(level[x - 1, y - 1]);
                case (int)Dir.W:
                    return levelManager.IsWall(level[x - 1, y]);
                case (int)Dir.SW:
                    return levelManager.IsWall(level[x - 1, y + 1]);
                case (int)Dir.S:
                    return levelManager.IsWall(level[x, y + 1]);
                case (int)Dir.SE:
                    return levelManager.IsWall(level[x + 1, y + 1]);
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

}
