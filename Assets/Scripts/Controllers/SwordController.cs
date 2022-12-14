using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    private Animator animator;
    private float swingTimer = 0f;

    private Collider2D[] collArray = new Collider2D[6];
    new private Collider2D collider;
    ContactFilter2D collFilter = new ContactFilter2D();

    private AudioSource audioSource;
    [SerializeField] private AudioClip moveSoundReflect;
    [SerializeField] private AudioClip moveSoundSwing;

    [SerializeField] private GameObject partSysReflectPrefab;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (swingTimer > 0f)
        {
            swingTimer -= Time.deltaTime;
        }
    }

    public void SwingSword()
    {
        if (swingTimer <= 0f)
        {
            // Swing
            animator.SetTrigger("Swing");
            transform.localScale = new Vector3(transform.localScale.x,
                                               -transform.localScale.y,
                                               transform.localScale.z);

            // SFX
            PlayAudioClip(audioSource, moveSoundSwing);

            // Collisions
            Array.Clear(collArray, 0, collArray.Length);
            Physics2D.OverlapCollider(collider, collFilter.NoFilter(), collArray);

            foreach (Collider2D coll in collArray)
            {
                if (coll != null)
                {
                    // Enemy
                    if (coll.tag == "Enemy")
                    {
                        EnemyController ec = coll.GetComponent<EnemyController>();
                        if (ec.GetStunState())
                        {
                            ec.EnterDeadState();
                        }
                    }

                    // Bullet
                    if (coll.tag == "Bullet")
                    {
                        coll.gameObject.GetComponent<BulletController>().Reflect();
                        swingTimer = 0f;
                        Instantiate(partSysReflectPrefab, transform.position, Quaternion.identity);
                        PlayAudioClip(audioSource, moveSoundReflect);
                    }
                }
            }

            // Cooldown
            swingTimer = 0.3f;
        }
    }

    void PlayAudioClip(AudioSource ads, AudioClip ac)
    {
        ads.Stop();
        ads.clip = ac;
        ads.loop = false;
        ads.Play();
    }
}
