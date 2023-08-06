using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Energistics.Behaviour
{
    public class BrickTypes
    {
        [SerializeField] float breakForce = 50f;
        [SerializeField] float breakTorque = 50f;
        [SerializeField] float connectionStrength = 1.5f;
        [SerializeField] float massConnectionStrength = 1.5f;
        [SerializeField] float mass = 1f;
        [SerializeField] float brickSize = 0.75f;
        [SerializeField] float drag = 1;
        [SerializeField] float angularDrag = 0.2f;
        [SerializeField] int jointAmount = 0;
        [SerializeField] float deleteTime = 2f;
        [SerializeField] bool isImpossible = false;
    }
}