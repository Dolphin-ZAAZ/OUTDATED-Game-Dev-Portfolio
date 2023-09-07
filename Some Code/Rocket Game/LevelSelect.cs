using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] GameObject levelSelectScreen;
    [SerializeField] GameObject firstSelectedLevel;
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject back;
    [SerializeField] GameObject mainMenu;
    EventSystem eventSystem;
    void Start()
    {
        Time.timeScale = 1;
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }
    void Update()
    {
        if (Input.GetButtonDown("Cancel") == true)
        {
            if(levelSelectScreen.activeSelf == true)
            {
                LevelSelectScreen();
            }
        }
    }
    public void LevelSelectScreen()
    {
        if (levelSelectScreen.activeSelf == false)
        {
            levelSelectScreen.SetActive(true);
            back.SetActive(true);
            mainMenu.SetActive(false);
            eventSystem.SetSelectedGameObject(firstSelectedLevel, null);
        }
        else if(levelSelectScreen.activeSelf == true)
        {
            levelSelectScreen.SetActive(false);
            back.SetActive(false);
            mainMenu.SetActive(true);
            eventSystem.SetSelectedGameObject(playButton, null);
        }
    }
    public void LoadThisLevel(int level)
    {
        SceneManager.LoadScene(level);
    }
    public void QuitToDesktop()
    {
        Application.Quit();
    }
}
