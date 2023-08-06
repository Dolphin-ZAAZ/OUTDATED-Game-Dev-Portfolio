using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Energistics.Behaviour;
using UnityEngine.Events;
using Energistics.Audio;
using Energistics.UI;

namespace Energistics.Player
{
    [DisallowMultipleComponent]
    public class PlayerInteractor : MonoBehaviour, IActor
    {
        [Header("Sound")]
        [SerializeField] SoundObject click1;
        [SerializeField] SoundObject click2;

        [Header("Config")]
        [SerializeField] LayerMask ignoreInRaycast;
        [SerializeField] float pickupDistance = 5f;
        [SerializeField] float lowerItemSpeed = 1.5f;
        public float GetPickupDistance { get { return pickupDistance; } }

        [Header("Composition")]
        private Camera cam;
        [SerializeField] Transform arms;
        [SerializeField] Vector3 armsPosition;
        [SerializeField] Vector3 focusPosition;
        [SerializeField] Vector3 focusRotation;
        public Transform Arms { get { return arms; } }
        private Item equippedItem;
        bool usingEquipedItem = false;
        bool isPrimed = false;
        float primeProgress = 0;
        public bool IsPrimed { get { return isPrimed; } }

        [SerializeField] Animator animator;

        MenuController menuController;
        Inventory inventory;
        InventoryUI inventoryUI;
        PlayerZoom zoomer;

        bool selfLimiterTrigger = false;

        private void Awake()
        {
            selfLimiterTrigger = false;
            primeProgress = -2f;
            inventory = FindObjectOfType<Inventory>();
            inventoryUI = FindObjectOfType<InventoryUI>();
            cam = FindObjectOfType<Camera>();
            zoomer = FindObjectOfType<PlayerZoom>();
            menuController = FindObjectOfType<MenuController>();
            if (arms == null)
            {
                if (transform.Find("Arms"))
                {
                    arms = transform.Find("Arms");
                }
                else
                {
                    arms = transform.Find("Player Camera").transform.Find("Arms");
                }
            }
        }
        private void Update()
        {
            if (equippedItem != null )
            {
                if (isPrimed && primeProgress > -1f)
                {
                    primeProgress += Time.deltaTime * equippedItem.PrimeSpeed;
                    if (inventory.ContainsItem(equippedItem))
                    {
                        zoomer.ZoomEnabled(true);
                        zoomer.SetZoomLevel(equippedItem.ZoomAmount);
                        animator.SetBool("isFocusing", true);
                        equippedItem.Transform.SetLocalPositionAndRotation(Vector3.Lerp(equippedItem.ArmsPosition, equippedItem.FocusPosition, primeProgress), Quaternion.Euler(Vector3.Lerp(equippedItem.ArmsRotation, equippedItem.FocusRotation, primeProgress)));
                    }
                    if (primeProgress > 1f)
                    {
                        primeProgress = -2f;
                        if (inventory.ContainsItem(equippedItem))
                        {
                            if (usingEquipedItem)
                            {
                                Focus(equippedItem);
                                equippedItem.OnFocusedAndUsing();
                            }
                            else
                            {
                                Focus(equippedItem);
                            }
                        }
                    }
                }
                else if (isPrimed == false && primeProgress > -1f)
                {
                    if (inventory.ContainsItem(equippedItem))
                    {
                        equippedItem.OnEquiped();
                    }
                    zoomer.ZoomEnabled(false);
                    primeProgress += Time.deltaTime * equippedItem.PrimeSpeed * lowerItemSpeed;
                    if (inventory.ContainsItem(equippedItem))
                    {
                        equippedItem.Transform.SetLocalPositionAndRotation(Vector3.Lerp(equippedItem.FocusPosition, equippedItem.ArmsPosition, primeProgress), Quaternion.Euler(Vector3.Lerp(equippedItem.FocusRotation, equippedItem.ArmsRotation, primeProgress)));
                    }
                    if (primeProgress > 1f)
                    {
                        primeProgress = -2f;
                        if (inventory.ContainsItem(equippedItem))
                        {
                            if (usingEquipedItem)
                            {
                                usingEquipedItem = false;
                                equippedItem.OnEquiped();
                                Equip(equippedItem);
                                Use(equippedItem);
                                usingEquipedItem = true;
                            }
                            else
                            {
                                Equip(equippedItem);
                            }
                        }
                    }
                }
            }
            else
            {
                zoomer.ZoomEnabled(false);
            }
        }
        public void StoreItem(Item item)
        {
            inventory.AddItem(item);
        }
        public void Interact()
        {
            Item item = null;
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, pickupDistance, ~ignoreInRaycast))
            {
                hit.transform.gameObject.TryGetComponent(out item);
                if (item != null)
                {
                    StoreItem(item);
                    return;
                }
            }
            IInteractable interactable = null;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, pickupDistance, ~ignoreInRaycast))
            {
                hit.transform.gameObject.TryGetComponent(out interactable);
                if (interactable != null)
                {
                    interactable.OnInteracted(this);
                }
            }
        }
        public void Equip(Item item)
        {
            if (usingEquipedItem == true)
            {
                return;
            }
            if (equippedItem != null)
            {
                if (inventory.ContainsItem(equippedItem) == false)
                {
                    equippedItem = null;
                    return;
                }
                equippedItem.OnPickedUp();
            }
            if (item == null)
            {
                if (equippedItem != null)
                {
                    equippedItem.OnPickedUp();
                    equippedItem = null;
                }
            }
            else
            {
                if (inventory.ContainsItem(item))
                {
                    equippedItem = item;
                    item.OnEquiped();
                    item.Transform.parent = arms;
                    item.Transform.localPosition = item.ArmsPosition; //arms position is set in inspector
                    item.Transform.SetLocalPositionAndRotation(item.ArmsPosition, Quaternion.Euler(item.ArmsRotation));
                    zoomer.ZoomEnabled(false);
                    animator.SetBool("isFocusing", false);
                }
            }
        }

        public void Drop(Item item)
        {
            if (item != null)
            {
                RaycastHit hit;
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, pickupDistance, ~ignoreInRaycast))
                {
                    item.transform.parent = null;
                    Vector3 eulerAngles = item.transform.localRotation.eulerAngles;
                    if (eulerAngles.y != 0)
                    {
                        eulerAngles = hit.normal * 90;
                    }
                    if (hit.normal.z != 0)
                    {
                        eulerAngles.y += 90;
                    }
                    item.Mesh.SetActive(true);
                    item.transform.SetLocalPositionAndRotation(hit.point + (hit.normal * item.GetComponentInChildren<MeshRenderer>().transform.localScale.y), Quaternion.Euler(0, eulerAngles.y, eulerAngles.z));
                    item.OnDrop();
                }
                else
                {
                    item.transform.parent = null;
                    Vector3 eulerAngles = item.transform.localRotation.eulerAngles;
                    eulerAngles.y += 90;
                    item.transform.SetLocalPositionAndRotation(transform.position + transform.forward * pickupDistance, Quaternion.Euler(0, eulerAngles.y, eulerAngles.z));
                    item.OnDrop();
                }
                inventory.RemoveItem(item);
                item.transform.parent = null;
                zoomer.ZoomEnabled(false);
            }
        }

        public void Focus(Item item)
        {
            if (inventory.ContainsItem(item))
            {
                item.Transform.parent = arms;
                item.OnFocus();
            }
        }

        public void Use(Item item)
        {
            item.OnUse();
        }

        #region 
        public void CancelInventory(InputAction.CallbackContext value)
        {
            if (menuController.IsPaused == true)
            {
                return;
            }
            if (equippedItem != null && value.canceled && inventoryUI.Locked)
            {
                if (inventory.ContainsItem(equippedItem))
                {
                    Equip(equippedItem);
                }
            }
        }
        public void PushPrime(InputAction.CallbackContext value)
        {
            if (menuController.IsPaused == true)
            {
                return;
            }
            if (equippedItem != null && value.performed && inventoryUI.Locked == false)
            {
                isPrimed = true;
                if (primeProgress <= -2f)
                {
                    primeProgress = 0f;
                }
                else
                {
                    primeProgress = 1 - primeProgress;
                }
            }
            if (equippedItem != null && value.canceled && inventoryUI.Locked == false)
            {
                isPrimed = false;
                if (primeProgress <= -2f)
                {
                    primeProgress = 0f;
                }
                else
                {
                    primeProgress = 1 - primeProgress;
                }
            }
        }
        public void PushInteract(InputAction.CallbackContext value)
        {
            if (menuController.IsPaused == true)
            {
                return;
            }
            if (value.performed)
            {
                Interact();
            }
        }
        public void PushUse(InputAction.CallbackContext value)
        {
            if (menuController.IsPaused == true)
            {
                return;
            }
            if (equippedItem != null && value.performed && inventoryUI.Locked == false)
            {
                usingEquipedItem = true;
                if (isPrimed)
                {
                    equippedItem.OnFocusedAndUsing();
                }
                else
                {
                    Use(equippedItem);
                }
            }
            if (equippedItem != null && value.canceled && inventoryUI.Locked == false)
            {
                usingEquipedItem = false;
                if (isPrimed)
                {
                    Focus(equippedItem);
                }
                else
                {
                    if (inventory.ContainsItem(equippedItem))
                    {
                        equippedItem.OnEquiped();
                        Equip(equippedItem);
                    }
                }
            }
        }
        #endregion
    }
}
