using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private float speed = 7f;
    public bool isReflected = false;
    private Vector3 dir = Vector3.zero;
    private Vector2 bounds = new Vector2(15f, 15f);
    private float lifetime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
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
                if (coll.tag == "Wall" || (coll.gameObject != gameObject && coll.tag == "Bullet"))
                {
                    Destroy(coll.gameObject);
                    Destroy();
                }
                else if (coll.tag == "Enemy")
                {
                    if (isReflected)
                    {
                        coll.GetComponent<EnemyController>().EnterStunnedState();
                        Destroy();
                    }
                    else if (lifetime >= 0.25f)
                    {
                        Destroy();
                    }
                }
            }
        }
        if (Mathf.Abs(transform.position.x) >= bounds.x ||
            Mathf.Abs(transform.position.y) >= bounds.y)
        {
            Destroy();
        }
    }

    public void Setup(Vector3 targetPos)
    {
        dir = (targetPos - transform.position).normalized;
    }

    public void Reflect(Vector3 sourcePos)
    {
        dir = (transform.position - sourcePos).normalized;
        speed = 10f;
        isReflected = true;
        GetComponent<TrailRenderer>().endColor = Color.cyan;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
