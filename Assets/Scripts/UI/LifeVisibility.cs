using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeVisibility : MonoBehaviour
{
    public int maxLives = 1;
    public GameManager gameManager;
    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.GetLives() < maxLives)
        {
            image.enabled = false;
        }
    }
}
