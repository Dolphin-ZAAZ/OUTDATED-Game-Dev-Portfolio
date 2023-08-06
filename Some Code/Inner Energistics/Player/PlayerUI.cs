using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Energistics.Behaviour;
using Energistics.Player;

namespace Energistics.UI
{
    public class PlayerUI : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] PlayerInteractor interactor;
        [SerializeField] LayerMask ignoreInRaycast;
        [SerializeField] float scanDistance = 5000f;
        [SerializeField] bool showAnyName = false;

        [Header("UI Elements")]
        [SerializeField] TMP_Text nameText;
        [SerializeField] TMP_Text pickUp;
        [SerializeField] TMP_Text interact;
        [SerializeField] CanvasGroup hud;

        string objectInViewName;
        Camera cam;
        Vector3 nameTextOrigin;
        private void Awake()
        {
            TryGetComponent<PlayerInteractor>(out interactor);
            if (interactor != null)
            {
                scanDistance = interactor.GetPickupDistance;
            }
            else
            {
                scanDistance = 10f;
            }
            nameTextOrigin = nameText.rectTransform.localPosition;
            cam = GetComponentInChildren<Camera>();
            if (cam.tag != "MainCamera")
            {
                print("Main Camera not being used");
            }
        }

        private void Update()
        {
            CheckInFront();
        }
        public void CheckInFront()
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, scanDistance, ~ignoreInRaycast))
            {
                objectInViewName = hit.transform.name;
                if (hit.transform.GetComponent<IInteractable>() != null)
                {
                    if (hit.transform.GetComponent<Item>())
                    {
                        pickUp.alpha = 1;
                        interact.alpha = 0; 
                        nameText.rectTransform.SetLocalPositionAndRotation(nameTextOrigin, Quaternion.identity);
                        nameText.alpha = 1;
                    }
                    else
                    {
                        pickUp.alpha = 0;
                        interact.alpha = 1;
                        nameText.rectTransform.SetLocalPositionAndRotation(nameTextOrigin, Quaternion.identity);
                        nameText.alpha = 1;
                    }
                }
                else
                {
                    pickUp.alpha = 0;
                    interact.alpha = 0;
                    nameText.rectTransform.SetLocalPositionAndRotation(new Vector3(nameTextOrigin.x, nameTextOrigin.y + interact.fontSize, 
                        nameTextOrigin.z), Quaternion.identity);
                    if (showAnyName)
                    {
                        nameText.alpha = 1;
                    }
                    else
                    {
                        nameText.alpha = 0;
                    }
                }
                nameText.text = objectInViewName;

            }
            else
            {
                nameText.alpha = 0;
                pickUp.alpha = 0;
                interact.alpha = 0;
            }
        }
    }
}