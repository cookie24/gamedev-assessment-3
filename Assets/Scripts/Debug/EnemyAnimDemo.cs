using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimDemo : MonoBehaviour
{

    public float defaultRotation = 0;
    [SerializeField] private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("EnemyRegen"))
        {
            transform.Rotate(0f, 0f, 360*Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, defaultRotation);
        }
    }
}
