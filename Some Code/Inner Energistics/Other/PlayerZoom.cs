using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Energistics.Player
{
    public class PlayerZoom : MonoBehaviour
    {
        Camera camera;
        bool isZoomed = false;
        public bool IsZoomed { get { return isZoomed; } }
        [SerializeField] float normalFOV = 90f;
        [SerializeField] float zoomFOV = 30f;
        private void Awake()
        {
            camera = Camera.main;
        }
        public void ZoomEnabled(bool enabled)
        {
            if (enabled)
            {
                camera.fieldOfView = zoomFOV;
                isZoomed = true;
            }
            else
            {
                camera.fieldOfView = normalFOV;
                isZoomed = false;
            }

        }
        public void ChangeZoomLevel(float amount)
        {
            if (GetComponent<PlayerInteractor>().IsPrimed == false)
            {
                if (isZoomed)
                {
                    camera.fieldOfView += amount;
                }
            }
        }
        public void SetZoomLevel(float amount)
        {
            if (isZoomed & amount > 1 && amount < 180)
            {
                camera.fieldOfView = amount;
            }
        }
        public void ZoomButton(InputAction.CallbackContext action)
        {
            if (action.performed && isZoomed == false)
            {
                ZoomEnabled(true);
            }
            if (action.canceled)
            {
                ZoomEnabled(false);
            }
        }
        public void ZoomIncrease(InputAction.CallbackContext action)
        {
            if (action.ReadValue<float>() > 0f)
            {
                ChangeZoomLevel(-1f);
            }
            if (action.ReadValue<float>() < 0f)
            {
                ChangeZoomLevel(1f);
            }
        }
    }
}