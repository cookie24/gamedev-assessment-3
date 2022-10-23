using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private MazeMovement mazeMover;
    private PointMovement pointMover;
    public GameManager gameManager;
    private Animator animator;

    private bool isScared = false;
    private bool isDead = false;
    private bool isRecovering = false;

    private float deadTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        mazeMover = GetComponent<MazeMovement>();
        pointMover = GetComponent<PointMovement>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // animator variables
        animator.SetBool("Scared", isScared);
        animator.SetBool("Dead", isDead);
        animator.SetBool("Recovering", isRecovering);

        // dead timer
        if (isDead)
        {
            deadTimer -= Time.deltaTime;
            if (deadTimer <= 0)
            {
                // respawn

                isDead = false;
                isRecovering = false;
            }
        }
        else if (isScared && gameManager.GetScaredTimer() <= 3f)
        {
            isRecovering = true;
        }
    }

    public void EnterScaredState()
    {
        isScared = true;
        isRecovering = false;
    }

    public void ExitScaredState()
    {
        isScared = false;
        isRecovering = false;
    }

    public void EnterDeadState()
    {
        isDead = true;
        deadTimer = 5f;
        isRecovering = false;
    }

    public void ExitDeadState()
    {
        isDead = false;
        isRecovering = false;
    }

    public bool GetDeadState()
    {
        return isDead;
    }
}
