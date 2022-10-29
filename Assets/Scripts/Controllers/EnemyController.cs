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
         Shooting,
         Stunned
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
    [SerializeField] private List<Vector3> clockwisePath = new List<Vector3>();
    [SerializeField] private List<Vector3> currentPath = new List<Vector3>();
    private bool isOnPath = false;

    private AudioSource audioSource;
    [SerializeField] private AudioClip moveSoundShoot;
    [SerializeField] private AudioClip moveSoundKill;
    [SerializeField] private AudioClip moveSoundStun;

    [SerializeField] private bool isInnovation = false;
    [SerializeField] private GameObject bulletPrefab;
    private float shootTimer = 0f;
    private float stunTimer = 0f;
    [SerializeField] private GameObject stunEffectObj;

    // Start is called before the first frame update
    void Start()
    {
        mazeMover = GetComponent<MazeMovement>();
        pointMover = GetComponent<PointMovement>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        currentMoveType = prefMoveType;
        OverwritePointList(exitPath);
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
                if (!pointMover.IsMoving())
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

                    // Design Innovation shooting mechanic
                    if (isInnovation && !isScared)
                    {
                        CheckToShoot();
                    }
                }
                else
                {
                    animator.speed = 0f;
                }
                break;
            case MoveState.Shooting:
                // Will only run in Level 2
                if (isScared)
                {
                    moveState = MoveState.Moving;
                }

                shootTimer -= Time.deltaTime;
                if (!pointMover.IsMoving())
                {
                    if (shootTimer <= 0f)
                    {
                        // Check if we should move instead of shooting
                        if (!CheckToMove() && gameManager.GetGameState() != GameManager.GameState.Dead)
                        {
                            // Face the player
                            Vector3 playerPos = player.transform.position;
                            float angle = Mathf.Atan2(playerPos.y - transform.position.y, playerPos.x - transform.position.x) * Mathf.Rad2Deg;
                            transform.rotation = Quaternion.Euler(0f, 0f, angle);

                            // Shoot
                            Instantiate(bulletPrefab,
                                        transform.position,
                                        transform.rotation)
                                .GetComponent<BulletController>()
                                .Setup(playerPos);

                            // Start shooting/moving cooldown
                            shootTimer = 0.8f;

                            // SFX
                            PlayAudioClip(audioSource, moveSoundShoot);
                        }
                    }
                }
                break;
            case MoveState.Stunned:
                if (gameManager.GetGameState() != GameManager.GameState.Paused &&
                    gameManager.GetGameState() != GameManager.GameState.Dead)
                {
                    stunTimer -= Time.deltaTime;
                    if (stunTimer <= 0f)
                    {
                        ExitStunnedState();
                    }
                }

                break;
        }
    }
    void PlayAudioClip(AudioSource ads, AudioClip ac)
    {
        ads.Stop();
        ads.clip = ac;
        ads.loop = false;
        ads.Play();
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
                InputTowardsPoint(player.transform.position);
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
            
                // Type 4: Move clockwise around map
            case MoveType.Type4:
                // reset the path if it is empty
                if (currentPath.Count <= 0)
                {
                    currentPath = new List<Vector3>(clockwisePath);
                    Vector3 point = GetClosestPoint(currentPath);
                    while (currentPath[0] != point && currentPath.Count > 0)
                    {
                        currentPath.RemoveAt(0);
                    }
                }

                // check if we are on path + delete points
                if (transform.position == currentPath[0])
                {
                    isOnPath = true;
                    currentPath.RemoveAt(0);
                }

                // reset the path if it is empty
                // yes this is here a second time to avoid a NullReferenceException
                if (currentPath.Count <= 0)
                {
                    currentPath = new List<Vector3>(clockwisePath);
                }

                if (!isOnPath)
                {
                    InputTowardsPoint(currentPath[0]);

                }
                else
                {
                    Vector3 targetPoint = currentPath[0];
                    InputTowardsPoint(targetPoint);
                }

                break;
        }

    }

    private void InputTowardsPoint(Vector3 point)
    {
        float distance = 9999f;
        Dir input = Dir.None;
        foreach (Dir dir in directionList)
        {
            float checkDist = Vector3.Distance(mazeMover.GetPosInDirection(dir), point);
            if (checkDist <= distance && isValidInput(dir))
            {
                input = dir;
                distance = checkDist;
            }
        }
        mazeMover.lastInput = input;
    }

    private bool isValidInput(Dir dir)
    {
        return Direction.GetOppositeDirection(mazeMover.currentInput) != dir &&
               !mazeMover.CollisionInDirection(dir, "Wall", 0.45f) &&
               !mazeMover.CollisionInDirection(dir, "Ghost Wall", 0.45f);
    }

    public void EnterScaredState()
    {
        isScared = true;
        isRecovering = false;
        isOnPath = false;
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
        isOnPath = false;

        stunTimer = 0f;
        stunEffectObj.GetComponent<SpriteRenderer>().enabled = false;
        PlayAudioClip(audioSource, moveSoundKill);
    }

    public void ExitDeadState()
    {
        isDead = false;
        isRecovering = false;
        mazeMover.ClearInput();
        OverwritePointList(exitPath);
        moveState = MoveState.InSpawn;
    }

    public bool GetDeadState()
    {
        return isDead;
    }

    public void OverwritePointList(List<Vector3> list)
    {
        pointMover.pointList = new List<Vector3>(exitPath);
    }

    private Vector3 GetClosestPoint(List<Vector3> list)
    {
        Vector3 picked = list[0];
        float dist = 9999f;
        float newDist;
        foreach (Vector3 point in list)
        {
            newDist = Vector3.Distance(transform.position, point);
            if (newDist <= dist)
            {
                picked = point;
                dist = newDist;
            }
        }
        return picked;
    }

    private bool CanShoot()
    {
        if (transform.position.x == player.transform.position.x ||
            transform.position.y == player.transform.position.y)
        {

            if (Physics2D.Linecast(transform.position, player.transform.position, LayerMask.GetMask("Wall")))
            {
                return false;
            }
            return true;
        }

        return false;
    }

    private void CheckToShoot()
    {
        if (CanShoot())
        {
            // enter shooting state
            mazeMover.ClearInput();
            moveState = MoveState.Shooting;
        }
    }

    private bool CheckToMove()
    {
        if (!CanShoot())
        {
            // Return to normal state
            moveState = MoveState.Moving;
            return true;
        }
        return false;
    }

    private Vector3 GetDirectionToPlayer()
    {
        return (transform.position - player.transform.position).normalized;
    }

    public void EnterStunnedState()
    {
        moveState = MoveState.Stunned;
        stunTimer = 3f;
        stunEffectObj.GetComponent<SpriteRenderer>().enabled = true;
        gameManager.AddScore(100);
        PlayAudioClip(audioSource, moveSoundStun);
    }

    public void ExitStunnedState()
    {
        moveState = MoveState.Moving;
        stunEffectObj.GetComponent<SpriteRenderer>().enabled = false;
    }

    public bool GetStunState()
    {
        return stunTimer > 0f;
    }
}
