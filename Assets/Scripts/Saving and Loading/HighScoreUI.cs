using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScoreUI : MonoBehaviour
{
    public TMP_Text level1Score;
    public TMP_Text level1Time;
    public TMP_Text level2Score;
    public TMP_Text level2Time;

    // Start is called before the first frame update
    void Start()
    {
        level1Score.text = "High Score: " + PlayerPrefs.GetInt("Level1Score", 0);
        level1Time.text = TimeFormatter.FormatTime(PlayerPrefs.GetFloat("Level1Time", 0));

        level2Score.text = "High Score: " + PlayerPrefs.GetInt("Level2Score", 0);
        level2Time.text = TimeFormatter.FormatTime(PlayerPrefs.GetFloat("Level2Time", 0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
