using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataWriter : MonoBehaviour
{
    private const string path = "Assets/Resources/scores.txt";

    public static void writeScore(string levelName, int value)
    {
        PlayerPrefs.SetInt(levelName, value);
    }
    public static int readScore(string levelName)
    {
        return PlayerPrefs.GetInt(levelName);
    }
    public static void resetValues()
    {
        PlayerPrefs.SetInt("Box", 0);
        PlayerPrefs.SetInt("Smiley", 0);
        PlayerPrefs.SetInt("Corridors", 0);
    }
}
