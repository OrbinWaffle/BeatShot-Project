
//This script manages the UI.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, ISyncable
{
    [Tooltip("The amount of beats before the counter actaully begins counting.")]
    int initialBeatDelay = 0;
    [Tooltip("A text object displaying the score.")]
    [SerializeField] private Text scoreText;
    [Tooltip("A button to begin the game.")]
    [SerializeField] private GameObject startButton;
    [Tooltip("A screen displayed when the player dies.")]
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject[] stars;
    [SerializeField] private GameObject menuButton;
    public static UIManager mainUIM;
    private int currStar = 0;
    bool playerIsAlive = true;
    void Awake()
    {
        mainUIM = this;
    }
    public void OnSync()
    {
        if(initialBeatDelay > 1)
        {
            initialBeatDelay--;
            return;
        }
        //On each beat, if the player is alive, increment the beats survived and update the counter.
        scoreText.text = "" + RhythmManager.mainRM.beatsSurvived;
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
        menuButton.SetActive(true);
    }
    public void IncrementStars()
    {
        if(currStar > stars.Length)
        {
            return;
        }
        stars[currStar].SetActive(true);
        ++currStar;
    }
}
