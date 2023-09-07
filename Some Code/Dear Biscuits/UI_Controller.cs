using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;

public class UI_Controller : MonoBehaviour
{
    AudioSource audiosource;
    AudioSource levelSelect;
    [SerializeField] AudioClip[] clips;
    [SerializeField] GameObject[] slides;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject paused;
    [SerializeField] GameObject inGameUI;
    [SerializeField] GameObject inGameAudio;

    [SerializeField] GameObject optionsMenu;

    public bool isPaused;

    float soundVolume = 0.5f;
    float musicVolume = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Music").GetComponent<Slider>().value = PlayerPrefs.GetFloat("Music Volume", 0.5f);
        GameObject.Find("Sound").GetComponent<Slider>().value = PlayerPrefs.GetFloat("Sound FX Volume", 0.5f);
        GameObject[] soundFX = GameObject.FindGameObjectsWithTag("SoundFX");
        foreach (GameObject sound in soundFX)
        {
            sound.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Sound FX Volume", 0.5f);
        }
        GameObject[] music = GameObject.FindGameObjectsWithTag("Music");
        foreach (GameObject source in music)
        {
            source.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Music Volume", 0.5f);
        }

        audiosource = this.GetComponent<AudioSource>();
        pauseMenu = GameObject.Find("PauseMenu");
        optionsMenu = GameObject.Find("OptionsMenu");
        optionsMenu.SetActive(false);
        inGameUI = GameObject.Find("InGameUI");
        inGameAudio = GameObject.Find("Audio");
        isPaused = false;
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            pauseMenu.SetActive(false);
        }
        else
        {
            levelSelect = GameObject.Find("Level Selected").GetComponent<AudioSource>();
        }
        paused = GameObject.Find("Paused");
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            paused.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && !GameObject.Find("WinTrigger").GetComponent<WinTrigger>().hasWon)
        {
            Pause();
        }
        if (Input.GetButtonDown("Reset") && isPaused)
        {
            Retry();
        }
    }
    IEnumerator Loadwait(int levelIndex)
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(levelIndex);
    }
    IEnumerator Quitwait()
    {
        yield return new WaitForSeconds(1f);
        Application.Quit();
    }
    public void PlayLevel(int levelIndex)
    {
        StartCoroutine(Loadwait(levelIndex)); 
        levelSelect.Play();
    }

    public void Back()
    {
        if (optionsMenu.activeSelf)
        {
            optionsMenu.SetActive(false);
            GameObject activeSlide = slides[0];
            audiosource.clip = clips[2];
            audiosource.Play();
            activeSlide.SetActive(true);
        }
        else
        {
            GameObject activeSlide = slides[0];
            for (int i = 0; i < slides.Length; i++)
            {
                if (slides[i].activeSelf == false)
                {
                    activeSlide = slides[i];
                }
                else
                {
                    slides[i].SetActive(false);
                    break;
                }
            }
            audiosource.clip = clips[2];
            audiosource.Play();
            activeSlide.SetActive(true);
        }
    }
    public void Next()
    {
        GameObject activeSlide = slides[0];
        for (int i = 0; i < slides.Length; i++)
        {
            if (slides[i].activeSelf == true)
            {
                activeSlide = slides[i + 1];
                slides[i].SetActive(false);
            }
            else
            {
                slides[i].SetActive(false);
            }
        }
        audiosource.clip = clips[1];
        audiosource.Play();
        activeSlide.SetActive(true);
    }
    public void QuitGame()
    {
        audiosource.clip = clips[2];
        audiosource.Play();
        Time.timeScale = 1;
        StartCoroutine(Quitwait());
    }

    public void Pause()
    {
        if (isPaused)
        {
            paused.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            pauseMenu.SetActive(false);
            inGameUI.SetActive(true);
            inGameAudio.SetActive(true);
            isPaused = false;
            Time.timeScale = 1;
            audiosource.clip = clips[2];
            audiosource.Play();
            GameObject[] soundFX = GameObject.FindGameObjectsWithTag("SoundFX");
            foreach (GameObject sound in soundFX)
            {
                sound.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Sound FX Volume");
            }
            GameObject[] music = GameObject.FindGameObjectsWithTag("Music");
            foreach (GameObject source in music)
            {
                source.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Music Volume");
            }
        }
        else
        {
            paused.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            pauseMenu.SetActive(true);
            inGameUI.SetActive(false);
            inGameAudio.SetActive(false);
            isPaused = true;
            Time.timeScale = 0;
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        audiosource.clip = clips[1];
        audiosource.Play();
        Time.timeScale = 1;
    }

    public void MainMenu()
    {
        audiosource.clip = clips[2];
        audiosource.Play();
        Time.timeScale = 1;
        StartCoroutine(Loadwait(0));
    }

    public void Options()
    {
        optionsMenu.SetActive(true);
        audiosource.clip = clips[1];
        audiosource.Play();
        slides[0].SetActive(false);
    }

    public void MusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("Music Volume", volume);
        GameObject[] music = GameObject.FindGameObjectsWithTag("Music");
        foreach (GameObject source in music)
        {
            source.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Music Volume");
        }
    }
    public void SoundFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("Sound FX Volume", volume);
        GameObject[] soundFX = GameObject.FindGameObjectsWithTag("SoundFX");
        foreach (GameObject sound in soundFX)
        {
            sound.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Sound FX Volume");
        }
    }

    public void hoverSelectPlay()
    {
        audiosource.clip = clips[3];
        audiosource.Play();
    }
}
