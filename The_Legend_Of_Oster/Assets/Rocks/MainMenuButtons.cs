using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenuButtons : MonoBehaviour
{
     
    public void PlayGame() {
        SceneManager.LoadScene(1); //hub world
    }

    public void QuitGame() {
        Application.Quit();
    }


}
