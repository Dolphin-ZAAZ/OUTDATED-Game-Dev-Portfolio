using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Energistics.Audio
{
    [DisallowMultipleComponent]
    public class SoundPool : MonoBehaviour
    {
        [SerializeField] SoundObject[] sources;
        int currentSourcesIndex = 0;

        private void Start()
        {
            currentSourcesIndex = 0;
        }
        public void GetSource()
        {
            sources[currentSourcesIndex].PlaySound();
            if (currentSourcesIndex < sources.Length - 1)
            {
                currentSourcesIndex++;
            }
            else
            {
                currentSourcesIndex = 1;
            }
        }
    }
}