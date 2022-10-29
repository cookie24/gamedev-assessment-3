using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private float speed = 10f;
    public bool isReflected = false;
    private Vector3 dir = Vector3.zero;
    private Vector2 bounds = new Vector2(15f, 15f);
    private float lifetime = 0f;

    [SerializeField] private GameObject particleShoot;
    [SerializeField] private GameObject particleStop;

    // Start is called before the first frame update
    void Start()
    {
        transform.position += transform.right * 0.4f;
        Instantiate(particleShoot, transform.position, transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        lifetime += Time.deltaTime;

        // move
        transform.position = Vector3.MoveTowards(transform.position,
                                                 transform.position + dir,
                                                 speed * Time.deltaTime) ;

        

        // check for collision with wall
        Vector2 checkPos = transform.position;
        Collider2D[] collArray = Physics2D.OverlapCircleAll(checkPos, 0.2f);
        foreach (Collider2D coll in collArray)
        {
            if (coll != null)
            {
                if (coll.tag == "Wall")
                {
                    Die();
                }
                else if (coll.tag == "Bullet" && coll.gameObject != gameObject &&
                    !isReflected && !coll.GetComponent<BulletController>().isReflected)
                {
                    Destroy(coll.gameObject);
                    Die();
                }
                else if (coll.tag == "Enemy" && !coll.GetComponent<EnemyController>().GetDeadState())
                {
                    if (isReflected)
                    {
                        coll.GetComponent<EnemyController>().EnterStunnedState();
                        Die();
                    }
                    else if (lifetime >= 0.25f)
                    {
                        Die();
                    }
                }
            }
        }
        if (Mathf.Abs(transform.position.x) >= bounds.x ||
            Mathf.Abs(transform.position.y) >= bounds.y)
        {
            Die();
        }
    }

    private void Die()
    {
        Instantiate(particleStop, transform.position, transform.rotation);
        Destroy();
    }

    public void Setup(Vector3 targetPos)
    {
        dir = (targetPos - transform.position).normalized;
    }

    public void Reflect()
    {
        dir = -dir;
        speed = 15f;
        isReflected = true;
        GetComponent<TrailRenderer>().endColor = Color.cyan;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
