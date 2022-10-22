using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    enum GameState
    {
        Paused,
        Normal,
        Scared,
        Dead
    }

    private float gameTimer = 0f;
    private int score;
    private GameState gameState = GameState.Paused;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameTimer += Time.deltaTime;
    }

    public float getGameTimer()
    {
        return gameTimer;
    }

    public void AddScore(int i)
    {
        score += i;
    }

    public int GetScore()
    {
        return score;
    }

}
