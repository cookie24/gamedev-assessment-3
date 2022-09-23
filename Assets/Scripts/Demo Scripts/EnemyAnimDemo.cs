using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimDemo : MonoBehaviour
{

    private Quaternion defaultRotation;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        defaultRotation = transform.rotation;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("EnemyRegen"))
        {
            transform.Rotate(0f, 0f, -360*Time.deltaTime);
        }
        else
        {
            transform.rotation = defaultRotation;
        }
    }
}
