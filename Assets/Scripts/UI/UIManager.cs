using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager;
    public TMP_Text scoreText;
    public TMP_Text gameTimerText;
    public TMP_Text scaredTimerText;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "Score: " + gameManager.GetScore();

        if (gameManager.GetScaredTimer() > 0)
        {
            scaredTimerText.text = Mathf.Ceil(gameManager.GetScaredTimer()).ToString();
        }
        else
        {
            scaredTimerText.text = "";
        }
    }
}
