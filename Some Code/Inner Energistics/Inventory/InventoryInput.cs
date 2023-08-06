using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace Energistics.Behaviour
{
    public class InventoryInput : MonoBehaviour
    {
        [SerializeField] GraphicRaycaster raycaster;
        [SerializeField] EventSystem eventSystem;
        [SerializeField] PointerEventData pointerData;
        [SerializeField] InventoryUI inventoryUI;
        [SerializeField] SlotManager slotManager;
        [SerializeField] HotbarManager hotbarManager;
        [SerializeField] bool extreme = false;
        private void Awake()
        {
            eventSystem = FindObjectOfType<EventSystem>();
            raycaster = transform.parent.GetComponentInChildren<GraphicRaycaster>();
        }
        public List<RaycastResult> MousePointerTarget()
        {
            List<RaycastResult> pointerResults = new List<RaycastResult>();
            pointerData = new PointerEventData(eventSystem);
            pointerData.position = Input.mousePosition;
            if (pointerData != null)
            {
                raycaster.Raycast(pointerData, pointerResults);
            }
            return pointerResults;
        }
        public Slot SlotUnderMouse()
        {
            List<RaycastResult> targets = MousePointerTarget();
            Slot slotUnderMouse = null;
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i].gameObject.TryGetComponent(out slotUnderMouse))
                {
                    break;
                }
            }
            return slotUnderMouse;
        }
        public void OptionsButton(InputAction.CallbackContext action)
        {
            if (action.performed)
            {
                if (inventoryUI.Open && inventoryUI.Locked == false)
                {
                    inventoryUI.LockInventory();
                }
                Slot slotUnderMouse = SlotUnderMouse();
                if (slotUnderMouse != null)
                {
                    if (slotUnderMouse.IsEmpty == false)
                    {
                        slotManager.SelectSlot(slotUnderMouse);
                        inventoryUI.OpenOptionsMenu();
                    }
                    else
                    {
                        slotManager.SelectSlot(null);
                        inventoryUI.CloseOptionsMenu();
                    }
                }
                else
                {
                    slotManager.SelectSlot(null);
                    inventoryUI.CloseOptionsMenu();
                }
            }
        }
        public void OpenInventoryButton(InputAction.CallbackContext action)
        {
            if (action.performed)
            {
                if (inventoryUI.Locked)
                {
                    inventoryUI.CloseInventory();
                }
                else
                {
                    inventoryUI.OpenInventory();
                }
            }
            else
            {
                if (inventoryUI.Locked == false)
                {
                    inventoryUI.CloseInventory();
                }
            }
        }
        public void SelectButton(InputAction.CallbackContext action)
        {
            if (action.performed && inventoryUI.optionOpen == false)
            {
                Slot tempSlot = SlotUnderMouse();
                if (slotManager.SlotSelected == false)
                {
                    if (tempSlot != null)
                    {
                        if (tempSlot.IsEmpty == false)
                        {
                            if (extreme == true)
                            {
                                slotManager.SelectSlot(tempSlot);
                                if (hotbarManager.FindNextSlot(tempSlot) != null)
                                {
                                    slotManager.MoveToSlot(hotbarManager.FindNextSlot(tempSlot));
                                    hotbarManager.UpdateHotbar();
                                }
                                else
                                {
                                    Debug.Log("Nowhere to move to.");
                                }
                            }
                            else
                            {
                                slotManager.SelectSlot(tempSlot);
                            }
                        }
                        else
                        {
                            slotManager.SelectSlot(null);
                        }
                    }
                    else
                    {
                        slotManager.SelectSlot(null);
                    }
                }
                else
                {
                    slotManager.MoveToSlot(tempSlot);
                }
            }
        }
        public void CloseInventoryButton(InputAction.CallbackContext action)
        {
            inventoryUI.CloseInventory();
        }
        public void DropInput(InputAction.CallbackContext action)
        {
            if (action.performed)
            {
                if (inventoryUI.Locked == false)
                {
                    hotbarManager.DropHotbarSelectedItem();
                    return;
                }
                slotManager.SelectSlot(SlotUnderMouse());
                if (extreme == true)
                {
                    slotManager.DropAllSlotItems();
                }
                else
                {
                    slotManager.DropSlotItem();
                }
            }
        }
        public void ExtremeButton(InputAction.CallbackContext action)
        {
            if (action.performed)
            {
                extreme = true;
            }
            if (action.canceled)
            {
                extreme = false;
            }
        }
    }
}
