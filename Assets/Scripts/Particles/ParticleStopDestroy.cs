using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleStopDestroy : MonoBehaviour
{
    private ParticleSystem partSys;

    // Start is called before the first frame update
    void Start()
    {
        partSys = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!partSys.isPlaying)
        {
            Destroy(this.gameObject);
        }
    }
}
