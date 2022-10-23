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

    private float introTimer = 4f;
    private float gameTimer = 0f;
    private float scaredTimer = 0f;
    public float deathTimer = 0f;
    private int score;
    private int lives = 3;
    private GameState gameState = GameState.Paused;

    public PlayerController player;
    public List<EnemyController> enemies;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (introTimer > 0f)
        {
            introTimer -= Time.deltaTime;
            if (introTimer <= 0f)
            {
                // Game Start
                gameState = GameState.Normal;
            }
        }
        if (gameState != GameState.Paused && gameState != GameState.Dead)
        {
            gameTimer += Time.deltaTime;

            scaredTimer -= Time.deltaTime;
            if (scaredTimer <= 0f)
            {
                // End of scared state

                scaredTimer = 0f;
                gameState = GameState.Normal;
                EndScaredState();
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

    public float GetIntroTimer()
    {
        return introTimer;
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
        foreach (EnemyController e in enemies)
        {
            e.EnterScaredState();
        }
    }

    public void EndScaredState()
    {
        foreach (EnemyController e in enemies)
        {
            e.ExitScaredState();
        }
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

    public bool GetRegenState()
    {
        foreach (EnemyController e in enemies)
        {
            if (e.GetDeadState())
            {
                return true;
            }
        }
        return false;
    }

}
