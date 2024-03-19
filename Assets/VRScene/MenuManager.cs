using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void Start3DGame()
    {
        SceneManager.LoadScene("TestScene");
    }
    public void Start2DGame()
    {
        SceneManager.LoadScene("Grid Map");
    }
    public void StartControls()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void StartTutorial()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
