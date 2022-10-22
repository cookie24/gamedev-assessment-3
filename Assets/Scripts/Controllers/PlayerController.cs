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
    [SerializeField] private AudioClip moveSoundStop;
    private AudioClip currentFootstepAudio;

    [SerializeField] private ParticleSystem partSysMove;
    [SerializeField] private GameObject partSysStopObj;

    private bool stoppedThisFrame = true;


    // Start is called before the first frame update
    void Start()
    {
        mazeMover = GetComponent<MazeMovement>();
        pointMover = GetComponent<PointMovement>();
        audioSource = GetComponent<AudioSource>();
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
            stoppedThisFrame = false;

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
}
