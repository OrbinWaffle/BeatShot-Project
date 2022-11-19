using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataWriter : MonoBehaviour
{
    private const string path = "Assets/Resources/test.txt";

    public static void writeScore(string levelName, string value)
    {
        int index = 0;
        StreamReader reader = new StreamReader(path);
        string text = reader.ReadLine();
        while (text != null)
        {
            string[] splitString = text.Split(" ");
            string fileLevelName = splitString[0];
            if (fileLevelName.Equals(levelName)) 
            {
                break;
            }
            text = reader.ReadLine();
            index++;
        }
        reader.Close();
        if(text == null)
        {
            Debug.LogWarning("Could not find level.");
            return;
        }
        lineChanger(levelName + " " + value, path, index);
    }

    public static string readScore(string levelName)
    {
        StreamReader reader = new StreamReader(path);
        string text = reader.ReadLine();
        while (text != null)
        {
            string[] splitString = text.Split(" ");
            string fileLevelName = splitString[0];
            if (fileLevelName.Equals(levelName)) 
            {
                reader.Close();
                return splitString[1];
            }
            text = reader.ReadLine();
        }
        reader.Close();
        return null;
    }

    static void lineChanger(string newText, string fileName, int lineToEdit)
    {
     string[] arrLine = File.ReadAllLines(fileName);
     for (int i = 0; i < arrLine.Length; i++) {
        Debug.Log(arrLine[i]);
     }
     Debug.Log(lineToEdit);
     arrLine[lineToEdit] = newText;
     File.WriteAllLines(fileName, arrLine);
    }
    public static void resetValues()
    {
        StreamWriter writer = new StreamWriter(path, false);
        writer.Write("easy 0\nmedium 0\nhard 0");
        writer.Close();
    }
}
