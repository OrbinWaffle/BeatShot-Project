using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, ISyncable
{
    int initialBeatDelay = 0;
    int beatsSurvived = 0;
    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject gameOver;
    bool playerIsAlive = true;
    public void OnSync()
    {
        if(initialBeatDelay > 1)
        {
            initialBeatDelay--;
            return;
        }
        if(playerIsAlive == true)
        {
            beatsSurvived++;
            scoreText.text = "" + beatsSurvived;
        }
    }
    public void BeginPlaying()
    {
        startButton.SetActive(false);
    }
    public void PlayerDied()
    {
        playerIsAlive = false;
        gameOver.SetActive(true);
    }
}
