using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerDashCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text counterText;
    [SerializeField] private PlayerController player;
    [SerializeField] private GameManager gameManager;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.dashCount > 0 && gameManager.GetGameState() == GameManager.GameState.Scared)
        {
            counterText.text = player.dashCount.ToString();
        }
        else
        {
            counterText.text = "";
        }
    }
}
