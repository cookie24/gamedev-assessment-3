using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLabelFreezer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, transform.parent.rotation.z * -1.0f);
    }
}
