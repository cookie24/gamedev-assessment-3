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
                    ExitDeadState();
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

        float distance;
        Dir input = Dir.None;

        switch (currentMoveType)
        {
            // Type 1: Furthest distance from player (run away)
            case MoveType.Type1:
                distance = 0f;
                foreach (Dir dir in directionList)
                {
                    float checkDist = Vector3.Distance(mazeMover.GetPosInDirection(dir), player.transform.position);
                    if (checkDist >= distance && isValidInput(dir))
                    {
                        input = dir;
                        distance = checkDist;
                    }
                }
                mazeMover.lastInput = input;
                break;

            // Type 2: Closest distance to player (hunter)
            case MoveType.Type2:
                distance = 9999f;
                foreach (Dir dir in directionList)
                {
                    float checkDist = Vector3.Distance(mazeMover.GetPosInDirection(dir), player.transform.position);
                    if (checkDist <= distance && isValidInput(dir))
                    {
                        input = dir;
                        distance = checkDist;
                    }
                }
                mazeMover.lastInput = input;
                break;

            // Type 3: Random valid direction
            case MoveType.Type3:
                List<Dir> validDirs = new List<Dir>();
                foreach (Dir dir in directionList)
                {
                    if (isValidInput(dir))
                    {
                        validDirs.Add(dir);
                    }
                }
                if (validDirs.Count > 0)
                {
                    // Only give an input if there's a valid one. Otherwise, do nothing.
                    mazeMover.lastInput = validDirs[Random.Range(0, validDirs.Count - 1)];
                }
                break;
        }

    }

    private bool isValidInput(Dir dir)
    {
        return Direction.GetOppositeDirection(mazeMover.currentInput) != dir &&
               !mazeMover.CollisionInDirection(dir, "Wall", 0.45f) &&
               !mazeMover.CollisionInDirection(dir, "Ghost Wall", 0.45f);
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
        pointMover.ClearPoints();
        pointMover.AddPoint(spawnPos);
        moveState = MoveState.Dead;
    }

    public void ExitDeadState()
    {
        isDead = false;
        isRecovering = false;
        pointMover.pointList = exitPath;
        moveState = MoveState.InSpawn;
    }

    public bool GetDeadState()
    {
        return isDead;
    }
}
