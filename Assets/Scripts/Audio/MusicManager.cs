using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] private AudioClip introMusic;
    [SerializeField] private AudioClip normalMusic;
    [SerializeField] private AudioClip deadMusic;
    [SerializeField] private AudioClip scaredMusic;

    private AudioSource audioSource;
    private bool isIntro = true;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gameManager = GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Start the music at the beginning of the game
        if (isIntro && gameManager.GetGameState() != GameManager.GameState.Paused)
        {
            SetClip(normalMusic, true);
            isIntro = false;
        }
        else
        {
            // Change music based on game state
            if (gameManager.GetGameState() == GameManager.GameState.Scared)
            {
                SwapClip(scaredMusic, true);
            }
            else if (gameManager.GetRegenState())
            {
                SwapClip(deadMusic, true);
            }
            else if (audioSource.clip == scaredMusic || audioSource.clip == deadMusic)
            {
                SwapClip(normalMusic, true);
            }
        }


    }

    // Changes the current audio clip, AND sets it to the same time.
    void SwapClip(AudioClip clip, bool isLoop) {
        if (audioSource.clip != clip)
        {
            float time = audioSource.time;
            SetClip(clip, isLoop);
            if (clip.length >= time)
            {
                audioSource.time = time;
            }
        }
    }

    void SetClip(AudioClip clip, bool isLoop)
    {
        audioSource.clip = clip;
        audioSource.Play();
        audioSource.loop = isLoop;
    }
}
