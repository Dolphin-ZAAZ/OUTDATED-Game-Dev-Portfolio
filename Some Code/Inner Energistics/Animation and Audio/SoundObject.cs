using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Energistics.Audio
{
    public enum SoundType
    {
        Master,
        Music,
        Environment,
        Weapon,
        NPC,
        Player,
        Voice
    }
    [System.Serializable]
    public class SoundObject : MonoBehaviour
    {

        public delegate void BroadcastSound();

        public static event BroadcastSound _broadcastSound;

        public UnityEvent<SoundObject> broadcastSound;

        AudioSource source;

        public float BroadcastRange { get { return broadcastRange; } }
        [SerializeField] SoundType soundType = SoundType.Master;
        public SoundType SoundType { get { return soundType; } }
        [SerializeField] bool onEnablePlay = true;
        [SerializeField] bool enableBroadcast = true;
        [SerializeField] float broadcastRange = 100f;
        [SerializeField] SoundPool pool;
        float volume = 1;
        float defaultVolume = 1f;
        private void Awake()
        {
            source = GetComponent<AudioSource>();
            defaultVolume = source.volume;
        }
        private void OnEnable()
        {
            if (onEnablePlay)
            { 
                if (pool != null)
                {
                    pool.GetSource();
                }
                else
                {
                    PlaySound();
                }
            }
        }
        public void SetVolume(float volume)
        {
            if (volume <= 100 && volume >= 0)
            {
                if (volume == 0)
                {
                    source.volume = 0;
                    return;
                }
                if (volume >= 100)
                {
                    source.volume = 100;
                    return;
                }
                source.volume = defaultVolume*volume/100;
            }
        }
        public void PlayFromCode()
        {
            if (pool != null)
            {
                pool.GetSource();
            }
            else
            {
                PlaySound();
            }
        }
        public void PlaySound()
        {
            source.Play();
            if (enableBroadcast)
            { 
                broadcastSound?.Invoke(this);
            }
        }
    }
}