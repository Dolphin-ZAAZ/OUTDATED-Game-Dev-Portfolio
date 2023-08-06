using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Energistics.Player;

namespace Energistics.Behaviour
{
    public class CollisionDetector : MonoBehaviour
    {
        [SerializeField] PlayerMovement mover;
        [SerializeField] LayerMask brickLayer;
        [SerializeField] float delay;
        [SerializeField] float force;
        private void OnTriggerEnter(Collider other)
        {
            mover.NullifyMovement(delay, force);
        }
        private void OnTriggerStay(Collider other)
        {
            mover.NullifyMovement(delay, force);
        }
    }
}