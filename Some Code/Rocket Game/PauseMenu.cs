using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update
    bool isPaused = true;
    GameObject pauseMenu;
    AudioSource inGameMusic;
    void Start()
    {
        inGameMusic = GameObject.Find("In Game Music").gameObject.GetComponent<AudioSource>();
        pauseMenu = GameObject.Find("Pause Menu").gameObject;
        pauseMenu.SetActive(false);
        isPaused = false;
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            PauseAndResume();
        }
    }
    public void PauseAndResume()
    {
        if (isPaused == true)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
            inGameMusic.Play();
            isPaused = false;
        }
        else
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            inGameMusic.Pause();
            isPaused = true;
        }
    }
    public void QuitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void QuitToDesktop()
    {
        Application.Quit();
    }
}
