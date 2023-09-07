using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    AudioSource click;
    GameObject slide1;
    GameObject slide2;
    GameObject next;
    GameObject back;
    // Start is called before the first frame update
    void Start()
    {
        click = GameObject.Find("Audio Source").GetComponent<AudioSource>();
        slide1 = GameObject.Find("Slide1");
        slide2 = GameObject.Find("Slide2");
        next = GameObject.Find("Next");
        back = GameObject.Find("Back");

        slide1.SetActive(false);
        slide2.SetActive(false);
        next.SetActive(false);
        back.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayLevel1()
    {
        SceneManager.LoadScene("MainGameplay");
        click.Play();
    }
    public void MainMenuQuitToDesktop()
    {
        click.Play();
        Application.Quit();
    }
    public void HowToPlay()
    {
        slide1.SetActive(true);
        next.SetActive(true);
        back.SetActive(true);
    }
    public void Next()
    {
        slide1.SetActive(false);
        slide2.SetActive(true);
        next.SetActive(false);
        back.SetActive(true);
    }

    public void Back()
    {
        if (slide1.activeSelf == true)
        {
            slide1.SetActive(false);
            slide2.SetActive(false);
            next.SetActive(false);
            back.SetActive(false);
        }
        else
        {
            slide1.SetActive(true);
            slide2.SetActive(false);
            next.SetActive(true);
            back.SetActive(true);
        }
    }
}
