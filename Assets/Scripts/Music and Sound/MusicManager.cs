using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    [SerializeField] private AudioClip introMusic;
    [SerializeField] private AudioClip normalMusic;
    [SerializeField] private AudioClip deadMusic;
    [SerializeField] private AudioClip scaredMusic;

    private AudioSource audioSource;
    private bool isIntro = true;
    private float timer = 0;
    private int intTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Start the music at the beginning of the game
        if (isIntro && !audioSource.isPlaying)
        {
            SetClip(normalMusic, true);
        }
        else
        {
            timer += Time.deltaTime;

            // Change music in certain intervals (debug)
            if (timer >= intTimer)
            {
                if (intTimer == 10)
                {
                    SwapClip(deadMusic, true);
                }
                else if (intTimer == 15)
                {
                    SwapClip(scaredMusic, true);
                }

                intTimer += 1;
            }
        }


    }

    // Changes the current audio clip, AND sets it to the same time.
    void SwapClip(AudioClip clip, bool isLoop) {
        float time = audioSource.time;
        SetClip(clip, isLoop);
        if (clip.length >= time) {
            audioSource.time = time;
        }
    }

    void SetClip(AudioClip clip, bool isLoop)
    {
        audioSource.clip = clip;
        audioSource.Play();
        audioSource.loop = isLoop;
    }
}
