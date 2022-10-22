using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Paused,
        Normal,
        Scared,
        Recovering,
        Dead
    }

    private float gameTimer = 0f;
    private float scaredTimer = 0f;
    public float deathTimer = 0f;
    private int score;
    private int lives = 3;
    private GameState gameState = GameState.Normal;

    public PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState != GameState.Paused && gameState != GameState.Dead)
        {
            gameTimer += Time.deltaTime;

            scaredTimer -= Time.deltaTime;
            if (scaredTimer <= 0f)
            {
                scaredTimer = 0f;
                gameState = GameState.Normal;
            }
        }
        else
        {
            if (gameState == GameState.Dead)
            {
                deathTimer -= Time.deltaTime;
                if (deathTimer <= 0f)
                {
                    // respawn code
                    player.Respawn();
                    gameState = GameState.Normal;
                }
            }
        }
    }

    public float GetGameTimer()
    {
        return gameTimer;
    }

    public float GetScaredTimer()
    {
        return scaredTimer;
    }

    public void AddScore(int i)
    {
        score += i;
    }

    public int GetScore()
    {
        return score;
    }

    public void StartScaredState()
    {
        gameState = GameState.Scared;
        scaredTimer = 10f;
    }

    public GameState GetGameState()
    {
        return gameState;
    }

    public void PauseState()
    {
        gameState = GameState.Paused;
    }

    public void StartDeathState()
    {
        gameState = GameState.Dead;
        deathTimer = 4f;
        lives -= 1;
    }

    public int GetLives()
    {
        return lives;
    }

}
