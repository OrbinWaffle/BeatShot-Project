
//This script manages the UI.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, ISyncable
{
    [Tooltip("The amount of beats before the counter actaully begins counting.")]
    int initialBeatDelay = 0;
    [Tooltip("How many beats have passed since the game started.")]
    int beatsSurvived = 0;
    [Tooltip("A text object displaying the score.")]
    [SerializeField] private Text scoreText;
    [Tooltip("A button to begin the game.")]
    [SerializeField] private GameObject startButton;
    [Tooltip("A screen displayed when the player dies.")]
    [SerializeField] private GameObject gameOver;
    bool playerIsAlive = true;
    public void OnSync()
    {
        if(initialBeatDelay > 1)
        {
            initialBeatDelay--;
            return;
        }
        //On each beat, if the player is alive, increment the beats survived and update the counter.
        if(playerIsAlive == true)
        {
            beatsSurvived++;
            scoreText.text = "" + beatsSurvived;
        }
    }
    //Disable the start button when the game begins.
    public void BeginPlaying()
    {
        startButton.SetActive(false);
    }
    //Display the game over screen when the player dies.
    public void PlayerDied()
    {
        playerIsAlive = false;
        gameOver.SetActive(true);
    }
}
