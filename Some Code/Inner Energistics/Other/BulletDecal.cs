using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Energistics.Behaviour
{
    public class BulletDecal : MonoBehaviour
    {
        public void PlaceDecal(Vector3 destination, Transform parent, Vector3 rotation)
        {
            transform.SetPositionAndRotation(destination, Quaternion.identity);
            transform.LookAt(destination + rotation);
            transform.parent = parent;
            transform.localPosition += transform.forward * 0.005f;
        }
    }
}