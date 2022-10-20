
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

    public void SwitchScene()
    {
        string clickedButtonName = EventSystem.current.currentSelectedGameObject.name;
        // Debug.Log("Name of button: " + clickedButtonName);

        if (clickedButtonName.Equals("Easy"))
            SceneManager.LoadScene(1);

    }
}
