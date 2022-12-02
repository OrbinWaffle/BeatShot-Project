using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Text boxScore;
    [SerializeField] Text smileScore;
    [SerializeField] Text corScore;

    void Awake()
    {
        UpdateScores();
    }
    public void ResetScores()
    {
        DataWriter.resetValues();
        UpdateScores();
    }
    void UpdateScores()
    {
        boxScore.text = "High Score: " + DataWriter.readScore("Box");
        smileScore.text = "High Score: " + DataWriter.readScore("Smiley");
        corScore.text = "High Score: " + DataWriter.readScore("Corridors");
    }
}
