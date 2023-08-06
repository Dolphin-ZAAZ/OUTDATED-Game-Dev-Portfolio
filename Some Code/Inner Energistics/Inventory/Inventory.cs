using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Energistics.Player;

namespace Energistics.Behaviour
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] Dictionary<string, List<Item>> inventoryItems = new Dictionary<string, List<Item>>();
        [SerializeField] SlotManager slotManager;
        [SerializeField] HotbarManager hotbarManager;
        public int itemCount { get { return inventoryItems.Count; } }

        public void AddItem(Item item)
        {
            if (slotManager.AllSlotsFull() == true)
            {
                print("Inventory is full");
            }
            else
            {
                if (inventoryItems.ContainsKey(item.ItemName))
                {
                    inventoryItems[item.ItemName].Add(item);
                    item.transform.parent = transform;
                    item.transform.position = transform.position + transform.right * 5f;
                    item.OnPickedUp();
                    slotManager.AddToSlot(item);
                }
                else
                {
                    inventoryItems.Add(item.ItemName, new List<Item>());
                    item.transform.parent = transform;
                    item.transform.position = transform.position + transform.right * 5f;
                    item.OnPickedUp();
                    slotManager.AddToSlot(item);
                }
            }
        }
        public void RemoveItem(Item item)
        {
            if (inventoryItems.ContainsKey(item.ItemName))
            {
                inventoryItems[item.ItemName].Remove(item);
                if (inventoryItems[item.ItemName].Count <= 0)
                {
                    inventoryItems.Remove(item.ItemName);
                    hotbarManager.UpdateHotbar();
                }
            }
            else
            {
                Debug.Log("This item is not in the inventory.");
            }
        }
        public bool ContainsItem(Item item)
        {
            return inventoryItems.ContainsKey(item.ItemName);
        }
    }
}