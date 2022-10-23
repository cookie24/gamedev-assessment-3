using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    enum MoveState
    {
         Moving,
         InSpawn,
         Dead,
    }

    enum MoveType
    {
        Type1,
        Type2,
        Type3,
        Type4
    }

    private static List<Dir> directionList = new List<Dir>()
        { 
            Dir.E,
            Dir.N,
            Dir.W,
            Dir.S
        };

    private MazeMovement mazeMover;
    private PointMovement pointMover;
    public GameManager gameManager;
    public GameObject player;
    private Animator animator;

    private bool isScared = false;
    private bool isDead = false;
    private bool isRecovering = false;

    [SerializeField] private MoveState moveState = MoveState.InSpawn;
    [SerializeField] private MoveType prefMoveType = MoveType.Type1;
    private MoveType currentMoveType;

    [SerializeField] private Vector3 spawnPos;
    [SerializeField] private List<Vector3> exitPath = new List<Vector3>();


    // Start is called before the first frame update
    void Start()
    {
        mazeMover = GetComponent<MazeMovement>();
        pointMover = GetComponent<PointMovement>();
        animator = GetComponent<Animator>();

        currentMoveType = prefMoveType;
        pointMover.pointList = exitPath;
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
            
        }
        else if (isScared && gameManager.GetScaredTimer() <= 3f)
        {
            isRecovering = true;
        }

        switch (moveState)
        {
            case MoveState.Dead:
                if (!pointMover.IsMoving())
                {
                    moveState = MoveState.InSpawn;
                }
                break;
            case MoveState.InSpawn:
                if (pointMover.pointList.Count <= 0)
                {
                    moveState = MoveState.Moving;
                }
                break;
            case MoveState.Moving:
                if (gameManager.GetGameState() != GameManager.GameState.Paused &&
                    gameManager.GetGameState() != GameManager.GameState.Dead)
                {
                    GetInput();
                    animator.speed = 1f;
                }
                else
                {
                    animator.speed = 0f;
                }
                break;
        }
    }

    void GetInput()
    {
        // Choose movement type based on scared state
        if (isScared)
        {
            currentMoveType = MoveType.Type1;
        }
        else
        {
            currentMoveType = prefMoveType;
        }

        switch (currentMoveType)
        {
            // Type 1: Furthest distance from player (run away)
            case MoveType.Type1:
                float distance = 0f;
                Dir input = Dir.None;
                foreach (Dir dir in directionList)
                {
                    float checkDist = Vector3.Distance(mazeMover.GetPosInDirection(dir), player.transform.position);
                    if (checkDist > distance &&
                        Direction.GetOppositeDirection(mazeMover.currentInput) != dir &&
                        !mazeMover.CollisionInDirection(dir, "Wall", 0.45f) &&
                        !mazeMover.CollisionInDirection(dir, "Ghost Wall", 0.45f))
                    {
                        input = dir;
                        distance = checkDist;
                    }
                }
                mazeMover.lastInput = input;
                break;
        }

    }

    private void Respawn()
    {
        isDead = false;
        isRecovering = false;
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
