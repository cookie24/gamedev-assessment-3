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


    // Start is called before the first frame update
    void Start()
    {
        // Get size of corner template
        int mapHeight = levelMap.GetLength(0);
        int mapWidth = levelMap.GetLength(1);

        // Create larger level out of that
        int[,] level = new int[mapWidth * 2, mapHeight * 2];
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

                // Bottom left
                level[x, levelHeight - y - 1] = levelMap[y, x];

                // Bottom right
                level[levelWidth - x - 1, levelHeight - y - 1] = levelMap[y, x];
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
                int key = level[x, y];
                if (key != 0)
                {
                    Instantiate(GetPrefabFromKey(key), new Vector3(startX + x, startY - y, 0f), Quaternion.identity, transform);
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
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
