using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveLoopDemo : MonoBehaviour
{

    [SerializeField] private PointMovement pointMover;
    [SerializeField] private List<Vector3> pointList = new List<Vector3>();

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip moveSound;

    // Start is called before the first frame update
    void Start()
    {
        pointMover = GetComponent<PointMovement>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pointMover.pointList.Count == 0)
        {
            foreach (Vector3 point in pointList)
            {
                pointMover.AddPoint(point);
            }
        }
    }
}
