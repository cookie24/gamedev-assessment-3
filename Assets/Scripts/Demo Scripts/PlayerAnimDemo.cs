using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimDemo : MonoBehaviour
{

    public enum AnimType {Idle, Walking, Dead};
    public AnimType animType;
    private float moveSpeed;

    [SerializeField] private Animator animator;

    private float timer = 0;
    private int counter = 1;

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = 0f;
        switch (animType)
        {
            case AnimType.Walking:
                moveSpeed = 1f;
                break;
            case AnimType.Dead:
                animator.SetTrigger("Dead");
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("MoveSpeed", moveSpeed);
        timer += Time.deltaTime;
        if (timer > counter)
        {
            if (animType == AnimType.Walking)
            {
                transform.Rotate(0f, 0f, 90f);
            }
            counter++;
        }
    }
}
