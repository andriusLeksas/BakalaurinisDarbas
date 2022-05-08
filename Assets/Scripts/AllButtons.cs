using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class AllButtons : MonoBehaviour
{
    public Slider musicSlider;
    GameObject gController;
    private void Awake()
    {
        gController = GameObject.Find("GameController");
        if (gController == null) return;
        AudioSource music = gController.GetComponent<AudioSource>();
        music.volume = musicSlider.value;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void ExitScene()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("GameLevel");
    }

    public void ChangeVolume(float vol)
    {
        if (gController == null) return;
        AudioSource music = gController.GetComponent<AudioSource>();
        music.volume = vol;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
