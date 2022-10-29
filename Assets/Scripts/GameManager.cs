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
        Dead,
    }

    public string levelName = "Level1";
    private float introTimer = 4f;
    private float gameTimer = 0f;
    private float scaredTimer = 0f;
    private float deathTimer = 0f;
    private float endTimer = 0f;
    private int score;
    private int lives = 3;
    private GameState gameState = GameState.Paused;

    private SceneLoader sceneLoader;

    public PlayerController player;
    public List<EnemyController> enemies;
    public List<GameObject> pellets = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        sceneLoader = GetComponent<SceneLoader>();
    }

    // Update is called once per frame
    void Update()
    {
        // Game Timer
        if (gameState != GameState.Paused)
        {
            gameTimer += Time.deltaTime;
        }

        // Intro
        if (introTimer > 0f)
        {
            introTimer -= Time.deltaTime;
            if (introTimer <= 0f)
            {
                // Game Start
                gameState = GameState.Normal;
            }
        }

        // Game over
        if (endTimer > 0f)
        {
            endTimer -= Time.deltaTime;
            if (endTimer <= 0f)
            {
                // Save and leave
                if (score > PlayerPrefs.GetInt(levelName + "Score", 0) ||
                    (score == PlayerPrefs.GetInt(levelName + "Score", 0) &&
                    gameTimer > PlayerPrefs.GetFloat(levelName + "Time", 0)))
                {
                    PlayerPrefs.SetInt(levelName + "Score", score);
                    PlayerPrefs.SetFloat(levelName + "Time", gameTimer);
                }
                sceneLoader.LoadScene("StartScene");
            }
        }

        // Normal gameplay
        if (gameState != GameState.Paused && gameState != GameState.Dead)
        {

            scaredTimer -= Time.deltaTime;
            if (scaredTimer <= 0f)
            {
                // End of scared state

                scaredTimer = 0f;
                gameState = GameState.Normal;
                EndScaredState();
            }

            if (pellets.Count <= 0)
            {
                TriggerGameOver();
            }

        }
        else
        {
            // Dead state
            if (gameState == GameState.Dead)
            {
                deathTimer -= Time.deltaTime;
                if (deathTimer <= 0f)
                {
                    if (lives <= 0)
                    {
                        // Game over
                        TriggerGameOver();
                    }
                    else
                    {
                        // respawn
                        player.Respawn();
                        gameState = GameState.Normal;
                    }
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

    public float GetEndTimer()
    {
        return endTimer;
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
        scaredTimer = 0f;
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

    public void TriggerGameOver()
    {
        endTimer = 3f;
        gameState = GameState.Paused;
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
