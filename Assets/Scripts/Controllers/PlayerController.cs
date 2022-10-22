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
    private AudioClip currentFootstepAudio;

    [SerializeField] private ParticleSystem partSysMove;
    [SerializeField] private GameObject partSysStopObj;

    private bool stoppedThisFrame = true;
    private Collider2D[] collArray = new Collider2D[2];
    new private Collider2D collider;
    ContactFilter2D collFilter = new ContactFilter2D();


    // Start is called before the first frame update
    void Start()
    {
        mazeMover = GetComponent<MazeMovement>();
        pointMover = GetComponent<PointMovement>();
        audioSource = GetComponent<AudioSource>();
        collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        PlayFootsteps();
        PlayParticles();
        CheckCollisions();
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
    }

    void PlayFootsteps()
    {
        // footstep audio checking
        if (pointMover.IsMoving())
        {
            stoppedThisFrame = false;

            // Determine which footstep audio we should play
            if (mazeMover.CollisionInDirection(mazeMover.currentInput, "Pellet", 0.8f) ||
                mazeMover.CollisionInDirection(Dir.None, "Pellet", 0.8f) )
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
        Physics2D.OverlapCollider(collider, collFilter.NoFilter(), collArray);
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
            Destroy(coll.gameObject);
            gameManager.AddScore(10);
        }

        // Bonus Cherry
        coll = CheckCollisionTag("Cherry");
        if (coll != null)
        {
            Destroy(coll.gameObject);
            gameManager.AddScore(100);
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
}
