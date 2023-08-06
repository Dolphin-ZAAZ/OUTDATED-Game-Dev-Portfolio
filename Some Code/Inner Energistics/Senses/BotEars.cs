using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SG;
using Energistics.Behaviour;
using Energistics.Audio;

namespace Energistics.Enemies
{
    public class BotEars : MonoBehaviour
    {
        [SerializeField] LayerMask layerMask;
        [SerializeField] List<Transform> audibleTargets = new List<Transform>();
        [SerializeField] float hearingRange = 5f;
        public List<Transform> AudibleTargets { get { return audibleTargets; } }
        SoundObject[] sounds;
        List<SoundObject> hearableSounds = new List<SoundObject>();

        float timer;
        [SerializeField] float hearingDelay;
        BotBrain brain;
        private void Start()
        {
            brain = GetComponent<BotBrain>();
            UpdateSounds();
        }

        public void UpdateSounds()
        {
            timer = hearingDelay; 
            sounds = FindObjectsOfType<SoundObject>();
            audibleTargets.Clear();
        }

        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0 && brain.CurrentTarget == null)
            {
                RangeTestSounds();
                timer = hearingDelay;
            }
        }
        public void RangeTestSounds()
        {
            foreach (var sound in sounds)
            {
                if (Vector3.Distance(transform.position, sound.transform.position) < hearingRange)
                {
                    if (Vector3.Distance(transform.position, sound.transform.position) < sound.BroadcastRange)
                    {
                        if (hearableSounds.Contains(sound) == false)
                        {
                            hearableSounds.Add(sound);
                        }
                        sound.broadcastSound.AddListener(DetectSound);
                    }
                    else
                    {
                        if (hearableSounds.Contains(sound))
                        {
                            hearableSounds.Remove(sound);
                        }
                        sound.broadcastSound.RemoveListener(DetectSound);
                    }
                }
            }
        }
        public void DetectSound(SoundObject sound)
        {
            if (audibleTargets.Contains(sound.transform.root) == false)
            {
                audibleTargets.Add(sound.transform.root);
            }
        }
    }
}
