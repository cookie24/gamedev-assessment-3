using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherryController : MonoBehaviour
{
    private GameManager gameManager;
    private float nextSpawnTime = 0f;
    private float spawnInterval = 10f;

    [SerializeField] private GameObject cherryPrefab;
    [SerializeField] private List<Vector3> cherrySpawnPoints;
    [SerializeField] private List<Vector3> cherryTargetPoints;
    private int cherryDataIndex = 0;


    private GameObject cherryObject;
    public float cherrySpeed;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GetComponent<GameManager>();
        nextSpawnTime += spawnInterval;
    }

    // Update is called once per frame
    void Update()
    {

        // spawn cherry
        if (gameManager.gameTimer >= nextSpawnTime)
        {
            // generate new cherry data index
            int i = (int)Random.Range(0, cherrySpawnPoints.Count - 1);
            if (i == cherryDataIndex)
            {
                if (i == 0)
                {
                    i++;
                }
                else
                {
                    i--;
                }
            }
            cherryDataIndex = i;


            // spawn
            cherryObject = Instantiate(cherryPrefab, cherrySpawnPoints[cherryDataIndex], Quaternion.identity);


            // set next spawn time
            nextSpawnTime += spawnInterval;
        }

        // move cherry
        if (cherryObject != null)
        {
            cherryObject.transform.position = Vector3.MoveTowards( cherryObject.transform.position,
                                                                    cherryTargetPoints[cherryDataIndex],
                                                                    cherrySpeed * Time.deltaTime);

            if (Vector3.Equals(cherryObject.transform.position, cherryTargetPoints[cherryDataIndex]))
            {
                Destroy(cherryObject);
            }
        }
    }
}
