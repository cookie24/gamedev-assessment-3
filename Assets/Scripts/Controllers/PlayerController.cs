using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private MazeMovement mazeMover;
    private PointMovement pointMover;
    public GameManager gameManager;

    private AudioSource audioSource;
    [SerializeField] private AudioClip moveSoundPelletless;
    [SerializeField] private AudioClip moveSoundPellet;
    [SerializeField] private AudioClip moveSoundStop;
    [SerializeField] private AudioClip moveSoundDeath;
    private AudioClip currentFootstepAudio;

    private Animator animator;

    [SerializeField] private ParticleSystem partSysMove;
    [SerializeField] private ParticleSystem partSysDeath;
    [SerializeField] private GameObject partSysStopObj;

    private bool stoppedThisFrame = true;
    private Collider2D[] collArray = new Collider2D[2];

    private Vector3 spawnPoint;

    [SerializeField] private bool isInnovation = false;
    [SerializeField] private SwordController swordController;

    private int dashCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        mazeMover = GetComponent<MazeMovement>();
        pointMover = GetComponent<PointMovement>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        spawnPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.GetGameState() != GameManager.GameState.Paused &&
            gameManager.GetGameState() != GameManager.GameState.Dead)
        {
            GetInput();
            PlayFootsteps();
            PlayParticles();
            CheckCollisions();
        }
    }

    void GetInput()
    {
        
        if (Input.GetKeyDown(KeyCode.D))
            {
                mazeMover.lastInput = Dir.E;
            }
            else
        if (Input.GetKeyDown(KeyCode.W))
            {
                mazeMover.lastInput = Dir.N;
            }
            else
        if (Input.GetKeyDown(KeyCode.A))
            {
                mazeMover.lastInput = Dir.W;
            }
            else
        if (Input.GetKeyDown(KeyCode.S))
            {
                mazeMover.lastInput = Dir.S;
            }

        // Swinging
        if (isInnovation && Input.GetKeyDown(KeyCode.Space))
        {
            if (gameManager.GetGameState() == GameManager.GameState.Scared)
            {
                // Dash
                Dash(mazeMover.currentInput);
            }
            else
            {
                // Swing
                swordController.SwingSword();
            }
        }
    }

    void Dash(Dir dir)
    {
        if (dir != Dir.None && pointMover.IsMoving())
        {
            // Snap player to destination tile position if they are still moving
            pointMover.SnapToCurrentPoint();
            pointMover.ClearPoints();

            CheckCollisions();

            // Move along every tile in direction, as long as there is no wall
            while (!mazeMover.CollisionInDirection(mazeMover.currentInput, "Wall", 0.45f))
            {
                transform.position = mazeMover.GetPosInDirection(dir);
                CheckCollisions();
            }

            // Check if we should leave scared state
            dashCount -= 1;
            if (dashCount <= 0)
            {
                gameManager.EndScaredState();
            }

        }
    }

    public void RefillDashes()
    {
        dashCount = 3;
    }

    void PlayFootsteps()
    {
        // footstep audio checking
        if (pointMover.IsMoving())
        {
            stoppedThisFrame = false;

            // Determine which footstep audio we should play
            if (mazeMover.CollisionInDirection(mazeMover.currentInput, "Pellet", 0.6f) ||
                mazeMover.CollisionInDirection(Dir.None, "Pellet", 0.5f) )
            {
                currentFootstepAudio = moveSoundPellet;
            }
            else
            {
                currentFootstepAudio = moveSoundPelletless;
            }

            // play the audio (if it isn't already)
            if (!audioSource.isPlaying && currentFootstepAudio != null)
            {
                PlayAudioClip(currentFootstepAudio);
            }
        }
        else
        {
            if (!stoppedThisFrame)
            {
                Debug.Log("BOOF");
                audioSource.Stop();
                PlayAudioClip(moveSoundStop);
            }
        }
    }

    void PlayAudioClip(AudioClip ac)
    {
        audioSource.clip = ac;
        audioSource.loop = false;
        audioSource.Play();
    }

    void PlayParticles()
    {
        // footsteps
        if (pointMover.IsMoving())
        {
            if (!partSysMove.isPlaying)
            {
                partSysMove.Play();
            }
        }
        else
        {
            if (partSysMove.isPlaying)
            {
                partSysMove.Stop();
            }
            if (!stoppedThisFrame)
            {
                Instantiate(partSysStopObj, transform.position + Direction.GetDirectionVector3(mazeMover.currentInput), Quaternion.identity)
                    .GetComponent<ParticleSystem>().Play();
                stoppedThisFrame = true;
            }
        }
    }

    void CheckCollisions()
    {
        // Get collisions
        Array.Clear(collArray, 0, collArray.Length);
        collArray = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        Collider2D coll;

        // Teleporters
        coll = CheckCollisionTag("Teleporter");
        if (coll != null)
        {

            // Clear the movement queue, keep inputs tho
            pointMover.ClearPoints();

            // teleport
            Vector3 targetPos = coll.GetComponent<Teleporter>().targetPos;
            transform.position = targetPos;

            Debug.Log("TP to " + targetPos);
        }

        // Pellets
        coll = CheckCollisionTag("Pellet");
        if (coll != null)
        {
            gameManager.pellets.Remove(coll.gameObject);
            Destroy(coll.gameObject);
            gameManager.AddScore(10);
        }

        // Power Pellet
        coll = CheckCollisionTag("Power Pellet");
        if (coll != null)
        {
            gameManager.pellets.Remove(coll.gameObject);
            Destroy(coll.gameObject);
            gameManager.StartScaredState();
        }

        // Bonus Cherry
        coll = CheckCollisionTag("Cherry");
        if (coll != null)
        {
            Destroy(coll.gameObject);
            gameManager.AddScore(100);
        }

        // Ghosts
        coll = CheckCollisionTag("Enemy");
        if (coll != null)
        {
            EnemyController ec = coll.GetComponent<EnemyController>();
            if (gameManager.GetGameState() == GameManager.GameState.Scared)
            {
                // Scared enemy code
                if (!ec.GetDeadState())
                {
                    ec.EnterDeadState();
                    gameManager.AddScore(300);
                }
            }
            else if (!ec.GetDeadState())
            {
                Die();
            }
        }

        // Bullets
        coll = CheckCollisionTag("Bullet");
        if (coll != null)
        {
            Die();
        }
    }

    Collider2D CheckCollisionTag(string tag)
    {
        foreach (Collider2D coll in collArray)
        {
            if (coll != null && coll.tag == tag)
            {
                return coll;
            }
        }
        return null;
    }

    public void Respawn()
    {
        transform.position = spawnPoint;
        pointMover.ClearPoints();
        mazeMover.ClearInput();
        animator.SetTrigger("Respawn");
    }

    public void Die()
    {
        // Death code

        Debug.Log("Player Death");

        gameManager.StartDeathState();
        animator.SetTrigger("Dead");

        partSysMove.Stop();
        partSysDeath.Play();

        audioSource.Stop();
        PlayAudioClip(moveSoundDeath);

        // despawn bullets
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Bullet"))
        {
            Destroy(gameObject);
        }
    }
}
