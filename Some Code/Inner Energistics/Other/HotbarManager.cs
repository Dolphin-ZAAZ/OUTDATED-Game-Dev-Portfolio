using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Energistics.Player;

namespace Energistics.Behaviour
{
    public class HotbarManager : MonoBehaviour
    {
        int selectedSlotIndex = 0;
        [SerializeField] Transform hotbarTransform;
        List<Slot> hotbarSlots = new List<Slot>();
        Slot selectedHotbarSlot;
        [SerializeField] PlayerInteractor playerInteractor;
        [SerializeField] SlotManager slotManager;
        [SerializeField] PlayerZoom zoomer;

        private void Awake()
        {
            for (int i = 0; i < FindObjectsOfType<Slot>().Length; i++)
            {
                try
                {
                    hotbarSlots.Add(hotbarTransform.GetChild(i).GetComponent<Slot>());
                }
                catch (System.Exception)
                {
                    return;
                }
            }
        }
        private void Start()
        {
            selectedHotbarSlot = hotbarSlots[0];
            selectedSlotIndex = 0;
            UpdateHotbar();
        }
        public bool IsFull()
        {
            bool conditional = false;
            for (int i = 0; i < hotbarSlots.Count; i++)
            {
                if (hotbarSlots[i].IsEmpty == true)
                {
                    conditional = false;
                }
                else
                {
                    conditional = true;
                }
            }
            return conditional;
        }
        public void AddToSlot(Item item)
        {
            bool addedToStack = false;
            for (int i = 0; i < hotbarSlots.Count; i++)
            {
                if (hotbarSlots[i].ItemName == item.ItemName)
                {
                    if (hotbarSlots[i].IsFull == true)
                    {
                        continue;
                    }
                    AddToSlotStack(item);
                    addedToStack = true;
                    break;
                }
            }
            if (addedToStack == false)
            {
                AddToNewSlot(item);
            }
            UpdateHotbar();
        }
        public Slot FindNextSlot(Slot selectedSlot)
        {
            if (selectedSlot.thisSlotType == SlotType.Hotbar)
            {
                return slotManager.FindNextSlot(selectedSlot);
            }
            for (int i = 0; i < hotbarSlots.Count; i++)
            {
                if (hotbarSlots[i].ItemName == selectedSlot.ItemName)
                {
                    if (hotbarSlots[i].IsFull == true)
                    {
                        continue;
                    }
                    return hotbarSlots[i];
                }
                else if (hotbarSlots[i].IsEmpty)
                {
                    return hotbarSlots[i];
                }
            }
            UpdateHotbar();
            return null;
        }
        private void AddToNewSlot(Item item)
        {
            if (hotbarSlots[selectedSlotIndex].IsEmpty)
            {
                hotbarSlots[selectedSlotIndex].AddItem(item);
                return;
            }
            for (int i = 0; i < hotbarSlots.Count; i++)
            {
                if (hotbarSlots[i].IsEmpty)
                {
                    hotbarSlots[i].AddItem(item);
                    break;
                }
            }
            UpdateHotbar();
        }
        private void AddToSlotStack(Item item)
        {
            for (int i = 0; i < hotbarSlots.Count; i++)
            {
                if (hotbarSlots[i].ItemName == item.ItemName)
                {
                    if (hotbarSlots[i].IsFull)
                    {
                        continue;
                    }
                    else
                    {
                        hotbarSlots[i].AddItem(item);
                        break;
                    }
                }
            }
            UpdateHotbar();
        }
        public void HotbarChangeInput(InputAction.CallbackContext action)
        {
            if (action.performed && zoomer.IsZoomed == false)
            {
                if (action.ReadValue<float>() < 0)
                {
                    if (selectedSlotIndex < hotbarSlots.Count - 1)
                    {
                        selectedSlotIndex++;
                    }
                }
                else if (action.ReadValue<float>() > 0)
                {
                    if (selectedSlotIndex > 0)
                    {
                        selectedSlotIndex--;
                    }
                }
                UpdateHotbar();
            }
        }
        public void UpdateHotbar()
        {
            DeselectAllSlots();
            selectedHotbarSlot = hotbarSlots[selectedSlotIndex];
            selectedHotbarSlot.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            if (selectedHotbarSlot.GetTopItem() != null)
            {
                playerInteractor.Equip(selectedHotbarSlot.GetTopItem());
            }
            else
            {
                playerInteractor.Equip(null);
            }
            if (selectedHotbarSlot.IsEmpty)
            {
                playerInteractor.Equip(null);
            }
        }
        public void DeselectAllButEquippedSlots()
        {
            for (int i = 0; i < hotbarSlots.Count; i++)
            {
                if (i == selectedSlotIndex)
                {
                    continue; 
                }
                hotbarSlots[i].transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
        public void DeselectAllSlots()
        {
            for (int i = 0; i < hotbarSlots.Count; i++)
            {
                hotbarSlots[i].transform.localScale = new Vector3(1f, 1f, 1f);
            }
            selectedHotbarSlot = null;
        }
        public void DropHotbarSelectedItem()
        {
            if (hotbarSlots[selectedSlotIndex].GetTopItem() != false)
            {
                playerInteractor.Drop(hotbarSlots[selectedSlotIndex].RemoveItem());
            }
        }
    }
}