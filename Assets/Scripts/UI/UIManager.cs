using System;
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
    public TMP_Text centreText;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Score
        scoreText.text = "Score: " + gameManager.GetScore();

        // Game Timer
        gameTimerText.text = TimeFormatter.FormatTime(gameManager.GetGameTimer());

        // Scared Timer
        if (gameManager.GetScaredTimer() > 0)
        {
            scaredTimerText.text = Mathf.Ceil(gameManager.GetScaredTimer()).ToString();
        }
        else
        {
            scaredTimerText.text = "";
        }

        // Intro Timer
        float introTimer = gameManager.GetIntroTimer();
        if (introTimer > 4f)
        {
            centreText.text = "";
        }
        else if (introTimer > 3f)
        {
            centreText.text = "3";
        }
        else if (introTimer > 2f)
        {
            centreText.text = "2";
        }
        else if (introTimer > 1f)
        {
            centreText.text = "1";
        }
        else if (introTimer > 0f)
        {
            centreText.text = "GO!";
        }
        else
        {
            centreText.text = "";
        }

        // Game Over
        if (gameManager.GetEndTimer() > 0f)
        {
            centreText.text = "Game Over";
        }
    }

    

}
