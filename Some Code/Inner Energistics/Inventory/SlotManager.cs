using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Energistics.Player;

namespace Energistics.Behaviour
{
    public class SlotManager : MonoBehaviour
    {
        [SerializeField] PlayerInteractor playerInteractor;
        [SerializeField] List<Slot> inventorySlots = new List<Slot>();
        [SerializeField] InventoryUI inventoryUI;
        [SerializeField] Transform inventoryTransform;
        [SerializeField] Slot selectedSlot;
        [SerializeField] HotbarManager hotbar;
        public bool SlotSelected { get { return selectedSlot != null; } }
        private void Start()
        {
            for (int i = 0; i < FindObjectsOfType<Slot>().Length; i++)
            {
                try
                {
                    inventorySlots.Add(inventoryTransform.GetChild(i).GetComponent<Slot>());
                }
                catch (System.Exception)
                {
                    return;
                }
            }
        }
        public bool AllSlotsFull()
        {
            int fullSlotsAmount = 0;
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (inventorySlots[i].IsFull)
                {
                    fullSlotsAmount++;
                    continue;
                }
            }
            if (fullSlotsAmount == inventorySlots.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public Slot FindNextSlot(Slot slot)
        {
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (inventorySlots[i].ItemName == selectedSlot.ItemName)
                {
                    if (inventorySlots[i].IsFull == true)
                    {
                        continue;
                    }
                    return inventorySlots[i];
                }
                else if (inventorySlots[i].IsEmpty)
                {
                    return inventorySlots[i];
                }
            }
            hotbar.UpdateHotbar();
            return null;
        }
        public void AddToSlot(Item item)
        {
            bool addedToStack = false;
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (inventorySlots[i].ItemName == item.ItemName)
                {
                    if (inventorySlots[i].IsFull == true)
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
            hotbar.UpdateHotbar();
        }
        private void AddToNewSlot(Item item)
        {
            if (hotbar.IsFull() == false)
            {
                if (item.MaxItemStack <= 1)
                {
                    hotbar.AddToSlot(item);
                    return;
                }
            }
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (inventorySlots[i].IsEmpty)
                {
                    inventorySlots[i].AddItem(item);
                    break;
                }
            }
            hotbar.UpdateHotbar();
        }
        private void AddToSlotStack(Item item)
        {
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (inventorySlots[i].ItemName == item.ItemName)
                {
                    if(inventorySlots[i].IsFull)
                    {
                        continue;
                    }
                    else
                    {
                        inventorySlots[i].AddItem(item);
                        break;
                    }
                }
            }
            hotbar.UpdateHotbar();
        }
        public void RemoveSlotItem(Item item)
        {
            selectedSlot.RemoveItem(); 
            hotbar.UpdateHotbar();
        }
        public void DropSlotItem()
        {
            if (selectedSlot != null)
            {
                playerInteractor.Drop(selectedSlot.RemoveItem());
                if (selectedSlot.IsEmpty)
                {
                    DeselectAllSlots();
                    inventoryUI.CloseOptionsMenu();
                }
            }
            hotbar.UpdateHotbar();
        }
        public void DropAllSlotItems()
        {
            if (selectedSlot != null)
            {
                int tempStackCount = selectedSlot.StackCount;
                for (int i = 0; i < tempStackCount; i++)
                {
                    playerInteractor.Drop(selectedSlot.RemoveItem());
                    if (selectedSlot.IsEmpty)
                    {
                        DeselectAllSlots();
                        inventoryUI.CloseOptionsMenu();
                        break;
                    }
                }
            }
            hotbar.UpdateHotbar();
        }
        public void SelectSlot(Slot slot)
        {
            DeselectAllSlots();
            if (slot != null)
            {
                selectedSlot = slot;
                selectedSlot.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            }
            hotbar.UpdateHotbar();
        }
        public void DeselectAllSlots()
        {
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                inventorySlots[i].transform.localScale = new Vector3(1f, 1f, 1f);
            }
            selectedSlot = null; 
            hotbar.UpdateHotbar();
        }
        public void MoveToSlot(Slot targetSlot)
        {
            if (selectedSlot != null && targetSlot != null && selectedSlot != targetSlot)
            {
                if (targetSlot.IsEmpty)
                {
                    int tempStackCount = selectedSlot.StackCount;
                    for (int i = 0; i < tempStackCount; i++)
                    {
                        targetSlot.AddItem(selectedSlot.RemoveItem());
                    }
                    DeselectAllSlots();
                }
                else
                {
                    
                    List<Item> oldSelectedSlotItems = new List<Item>(); 
                    List<Item> oldTargetSlotItems = new List<Item>();
                    int tempSelectedStackCount = selectedSlot.StackCount; 
                    int tempTargetStackCount = targetSlot.StackCount;
                    //Merge Items
                    if (selectedSlot.IsFull == false && targetSlot.IsFull == false && selectedSlot.ItemName == targetSlot.ItemName)
                    {
                        for (int i = 0; i < tempSelectedStackCount; i++)
                        {
                            targetSlot.AddItem(selectedSlot.RemoveItem());
                            if (targetSlot.IsFull == true)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        //Swap items
                        for (int i = 0; i < tempSelectedStackCount; i++)
                        {
                            oldSelectedSlotItems.Add(selectedSlot.RemoveItem());
                        }
                        for (int i = 0; i < tempTargetStackCount; i++)
                        {
                            oldTargetSlotItems.Add(targetSlot.RemoveItem());
                        }
                        for (int i = 0; i < oldSelectedSlotItems.Count; i++)
                        {
                            targetSlot.AddItem(oldSelectedSlotItems[i]);
                        }
                        for (int i = 0; i < oldTargetSlotItems.Count; i++)
                        {
                            selectedSlot.AddItem(oldTargetSlotItems[i]);
                        }
                    }
                    DeselectAllSlots();
                }
            }
            hotbar.UpdateHotbar();
        }
        public void SplitSlot()
        {
            int tempStackCount = selectedSlot.StackCount;
            if (tempStackCount == 2)
            {
                AddToNewSlot(selectedSlot.RemoveItem());
            }
            else
            {
                for (int i = tempStackCount - (tempStackCount % 2); i < tempStackCount; i++)
                {
                    AddToNewSlot(selectedSlot.RemoveItem());
                }
            }
            hotbar.UpdateHotbar();
        }
        public void UseSlotItem()
        {
            selectedSlot.UseItem();
            hotbar.UpdateHotbar();
        }
    }
}