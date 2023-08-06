using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Energistics.Audio
{
    public class PlayParticleScript : MonoBehaviour
    {

        public delegate void BroadcastParticle();

        public static event BroadcastParticle broadcastParticle;

        ParticleSystem particle;
        private void Awake()
        {
            particle = GetComponent<ParticleSystem>();
        }
        private void OnEnable()
        {
            particle.Play();
            broadcastParticle?.Invoke();
        }
    }
}
