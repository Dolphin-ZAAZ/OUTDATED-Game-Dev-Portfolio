using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Energistics.Audio
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] float masterVolume = 100f;
        [SerializeField] float musicVolume = 100f;
        [SerializeField] float environmentVolume = 100f;
        [SerializeField] float weaponVolume = 100f;
        [SerializeField] float npcVolume = 100f;
        [SerializeField] float playerVolume = 100f;
        [SerializeField] float voiceVolume = 100f;

        List<SoundObject> allSounds = new List<SoundObject>();
        List<SoundObject> musicSounds = new List<SoundObject>();
        List<SoundObject> environmentSounds = new List<SoundObject>();
        List<SoundObject> weaponSounds = new List<SoundObject>();
        List<SoundObject> npcSounds = new List<SoundObject>();
        List<SoundObject> playerSounds = new List<SoundObject>();
        List<SoundObject> voiceSounds = new List<SoundObject>();

        private void Awake()
        {
            allSounds.AddRange(FindObjectsOfType<SoundObject>());
            for (int i = 0; i < allSounds.Count; i++)
            {
                switch (allSounds[i].SoundType)
                {
                    case SoundType.Environment:
                        environmentSounds.Add(allSounds[i]);
                        break;
                    case SoundType.Music:
                        musicSounds.Add(allSounds[i]);
                        break;
                    case SoundType.Weapon:
                        weaponSounds.Add(allSounds[i]);
                        break;
                    case SoundType.NPC:
                        npcSounds.Add(allSounds[i]);
                        break;
                    case SoundType.Player:
                        playerSounds.Add(allSounds[i]);
                        break;
                    case SoundType.Voice:
                        voiceSounds.Add(allSounds[i]);
                        break;
                }
            }
        }
        public void ChangeVolume(SoundType soundType, float volume)
        {
            switch (soundType)
            {
                case SoundType.Master:
                    masterVolume = volume;
                    for (int i = 0; i < allSounds.Count; i++)
                    {
                        allSounds[i].SetVolume(masterVolume);
                    }
                    break;
                case SoundType.Environment:
                    environmentVolume = volume;
                    for (int i = 0; i < environmentSounds.Count; i++)
                    {
                        environmentSounds[i].SetVolume(environmentVolume);
                    }
                    break;
                case SoundType.Music:
                    musicVolume = volume;
                    for (int i = 0; i < musicSounds.Count; i++)
                    {
                        musicSounds[i].SetVolume(musicVolume);
                    }
                    break;
                case SoundType.Weapon:
                    weaponVolume = volume;
                    for (int i = 0; i < weaponSounds.Count; i++)
                    {
                        weaponSounds[i].SetVolume(weaponVolume);
                    }
                    break;
                case SoundType.NPC:
                    npcVolume = volume;
                    for (int i = 0; i < npcSounds.Count; i++)
                    {
                        npcSounds[i].SetVolume(npcVolume);
                    }
                    break;
                case SoundType.Player:
                    playerVolume = volume;
                    for (int i = 0; i < playerSounds.Count; i++)
                    {
                        playerSounds[i].SetVolume(playerVolume);
                    }
                    break;
                case SoundType.Voice:
                    voiceVolume = volume;
                    for (int i = 0; i < voiceSounds.Count; i++)
                    {
                        voiceSounds[i].SetVolume(voiceVolume);
                    }
                    break;
            }
        }
        public void MuteSounds(SoundType soundType)
        {
            switch (soundType)
            {
                case SoundType.Master:
                    for (int i = 0; i < allSounds.Count; i++)
                    {
                        allSounds[i].SetVolume(0);
                    }
                    break;
                case SoundType.Environment:
                    for (int i = 0; i < environmentSounds.Count; i++)
                    {
                        environmentSounds[i].SetVolume(0);
                    }
                    break;
                case SoundType.Music:
                    for (int i = 0; i < musicSounds.Count; i++)
                    {
                        musicSounds[i].SetVolume(0);
                    }
                    break;
                case SoundType.Weapon:
                    for (int i = 0; i < weaponSounds.Count; i++)
                    {
                        weaponSounds[i].SetVolume(0);
                    }
                    break;
                case SoundType.NPC:
                    for (int i = 0; i < npcSounds.Count; i++)
                    {
                        npcSounds[i].SetVolume(0);
                    }
                    break;
                case SoundType.Player:
                    for (int i = 0; i < playerSounds.Count; i++)
                    {
                        playerSounds[i].SetVolume(0);
                    }
                    break;
                case SoundType.Voice:
                    for (int i = 0; i < voiceSounds.Count; i++)
                    {
                        voiceSounds[i].SetVolume(0);
                    }
                    break;
            }
        }
        public void UnmuteSounds(SoundType soundType)
        {
            switch (soundType)
            {
                case SoundType.Master:
                    for (int i = 0; i < allSounds.Count; i++)
                    {
                        allSounds[i].SetVolume(masterVolume);
                    }
                    break;
                case SoundType.Environment:
                    for (int i = 0; i < environmentSounds.Count; i++)
                    {
                        environmentSounds[i].SetVolume(environmentVolume);
                    }
                    break;
                case SoundType.Music:
                    for (int i = 0; i < musicSounds.Count; i++)
                    {
                        musicSounds[i].SetVolume(musicVolume);
                    }
                    break;
                case SoundType.Weapon:
                    for (int i = 0; i < weaponSounds.Count; i++)
                    {
                        weaponSounds[i].SetVolume(weaponVolume);
                    }
                    break;
                case SoundType.NPC:
                    for (int i = 0; i < npcSounds.Count; i++)
                    {
                        npcSounds[i].SetVolume(npcVolume);
                    }
                    break;
                case SoundType.Player:
                    for (int i = 0; i < playerSounds.Count; i++)
                    {
                        playerSounds[i].SetVolume(playerVolume);
                    }
                    break;
                case SoundType.Voice:
                    for (int i = 0; i < voiceSounds.Count; i++)
                    {
                        voiceSounds[i].SetVolume(voiceVolume);
                    }
                    break;
            }
        }
    }
}