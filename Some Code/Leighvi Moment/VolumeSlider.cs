using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   

public class VolumeSlider : MonoBehaviour
{
    // Start is called before the first frame update
    Slider slider;
    Slider musicSlider;

    AudioSource music;
    void Start()
    {
        slider = GameObject.Find("MasterVolume").GetComponent<Slider>();
        musicSlider = GameObject.Find("MusicVolume").GetComponent<Slider>();
        music = GameObject.Find("BackgroundMusic").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        AudioListener.volume = slider.value;
        music.volume = musicSlider.value / 10;
    }
}
