using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMovement : MonoBehaviour
{

    public List<Vector3> pointList = new List<Vector3>();
    private Animator animator;

    public float moveSpeed;
    private float moveSpeedAnim = 0;

    private AudioSource audioSource;
    [SerializeField] private AudioClip moveSound;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        moveSpeedAnim = 0;

        // Move our lil object if there is a destination for him to go
        if (pointList.Count != 0)
        {
            Vector3 activePoint = pointList[0];
            moveSpeedAnim = moveSpeed;

            // footstep audio checking
            if (moveSound != null && !audioSource.isPlaying)
            {
                audioSource.clip = moveSound;
                audioSource.loop = true;
                audioSource.Play();
            }

            // rotation
            float angle = Mathf.Atan2(activePoint.y - transform.position.y, activePoint.x - transform.position.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            // position
            transform.position = Vector3.MoveTowards(transform.position, activePoint, moveSpeed * Time.deltaTime);

            // muuuuuum are we there yet???
            if (Vector3.Equals(transform.position, activePoint))
            {
                RemovePoint(activePoint);
            }
        }
        else
        {
            audioSource.Stop();
        }

        animator.SetFloat("MoveSpeed", moveSpeedAnim);
    }

    // Functions for adding points to the list
    public void ClearPoints()
    {
        pointList.Clear();
    }

    public void AddPoint(Vector3 point)
    {
        pointList.Add(point);
    }

    public bool RemovePoint(Vector3 point)
    {
        return pointList.Remove(point);
    }

    public bool RemovePointAt(int i)
    {
        if (pointList.Count > i)
        {
            pointList.RemoveAt(i);
            return true;
        }
        return false;
    }
}
