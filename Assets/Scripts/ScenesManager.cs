
//This script manages the different scenes.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void SwitchScene(int sceneChoice)
    {
        SceneManager.LoadScene(sceneChoice);

        /*
        string clickedButtonName = EventSystem.current.currentSelectedGameObject.name;
        // Debug.Log("Name of button: " + clickedButtonName);

        if (clickedButtonName.Equals("MenuButton"))
            SceneManager.LoadScene(0);
        else if (clickedButtonName.Equals("TestButton"))
            SceneManager.LoadScene(1);
        else if (clickedButtonName.Equals("EasyButton"))
            SceneManager.LoadScene(2);
        else if (clickedButtonName.Equals("MediumButton"))
            SceneManager.LoadScene(3);
        else if (clickedButtonName.Equals("HardButton"))
            SceneManager.LoadScene(4);
        */
    }
}
