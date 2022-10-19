using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private MazeMovement mazeMover;
    private PointMovement pointMover;

    private AudioSource audioSource;
    [SerializeField] private AudioClip moveSoundPelletless;
    [SerializeField] private AudioClip moveSoundPellet;
    private AudioClip currentFootstepAudio;

    private ParticleSystem partSys;


    // Start is called before the first frame update
    void Start()
    {
        mazeMover = GetComponent<MazeMovement>();
        pointMover = GetComponent<PointMovement>();
        audioSource = GetComponent<AudioSource>();
        partSys = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        PlayFootsteps();
        PlayParticles();
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
            // Determine which footstep audio we should play
            if (mazeMover.CollisionInDirection(mazeMover.currentInput, "Pellet") || mazeMover.CollisionInDirection(Dir.None, "Pellet"))
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
                audioSource.clip = currentFootstepAudio;
                audioSource.loop = false;
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }
    }

    void PlayParticles()
    {
        // footsteps
        if (pointMover.IsMoving())
        {
            if (!partSys.isPlaying)
            {
                Debug.Log("Playing");
                partSys.Play();
            }
        }
        else
        {
            if (partSys.isPlaying)
            {
                Debug.Log("Stopping");
                partSys.Stop();
            }
        }
    }
}
