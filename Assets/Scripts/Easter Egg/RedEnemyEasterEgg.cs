using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedEnemyEasterEgg : MonoBehaviour
{
    private int count = 0;

    public void TriggerEasterEgg()
    {
        count++;
        if (count == 3)
        {
            GetComponent<AudioSource>().Play();
        }
    }
}
