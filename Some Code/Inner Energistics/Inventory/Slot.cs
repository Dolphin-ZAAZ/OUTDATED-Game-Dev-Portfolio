using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Energistics.Behaviour
{
    public enum SlotType
    {
        Hotbar,
        Inventory
    }
    public class Slot : MonoBehaviour
    {
        [SerializeField] Inventory inventory;
        [SerializeField] SlotManager slotManager;
        [SerializeField] HotbarManager hotbarManager;
        [SerializeField] bool isFull = false;
        [SerializeField] bool isEmpty = true;
        [SerializeField] SlotType slotType = SlotType.Inventory;
        public SlotType thisSlotType { get { return slotType; } }
        public bool IsFull { get { return isFull; } }
        public bool IsEmpty { get { return isEmpty; } }

        [SerializeField] List<Item> itemsInStack = new List<Item>();
        [SerializeField] Image slotImage;
        [SerializeField] Image stackAmountImage;
        [SerializeField] TMP_Text stackAmount;
        [SerializeField] string itemName;
        public string ItemName { get { return itemName; } }
        public int StackCount { get { return itemsInStack.Count; } }

        Sprite defaultSlotImage;
        Sprite defaultAmountImage;
        private void Awake()
        {
            slotImage = transform.Find("Icon").GetComponent<Image>();
            stackAmount = transform.Find("Number Background").GetComponentInChildren<TMP_Text>();
            stackAmountImage = transform.Find("Number Background").GetComponent<Image>();
            stackAmount.text = "";

            defaultSlotImage = slotImage.sprite;
            defaultAmountImage = stackAmountImage.sprite;

            ClearSlot(); // watch this
            isEmpty = true;
        }
        public void AddItem(Item item)
        {
            if (isEmpty)
            {
                itemsInStack.Add(item);
                slotImage.sprite = item.Image;
                stackAmountImage.enabled = true;
                stackAmountImage.sprite = defaultAmountImage;
                itemName = item.ItemName;
                isEmpty = false;
            }
            else if (isFull)
            {
                Debug.Log(name + ": Slot is full");
            }
            else
            {
                itemsInStack.Add(item);
            }
            if (itemsInStack.Count >= item.MaxItemStack)
            {
                isFull = true;
            }
            stackAmount.text = StackCount.ToString();
        }
        public Item RemoveItem()
        {
            if (itemsInStack.Count > 0)
            {
                Item topSlotItem = itemsInStack[0];
                itemsInStack.RemoveAt(0); // not gonna work
                isFull = false;
                stackAmount.text = StackCount.ToString();
                if (itemsInStack.Count <= 0)
                {
                    ClearSlot();
                }
                return topSlotItem;
            }
            else
            {
                stackAmount.text = "";
                ClearSlot();
                return null;
            }
        }
        public Item UseItem()
        {
            Item usedItem = RemoveItem();
            if (StackCount <= 0)
            {
                stackAmount.text = "";
                ClearSlot();
                return null;
            }
            return usedItem;
        }
        public void ClearSlot()
        {
            isEmpty = true;
            isFull = false;
            itemName = null;
            slotImage.sprite = defaultSlotImage; 
            stackAmountImage.enabled = false;
            stackAmount.text = "";
        }
        public Item GetTopItem()
        {
            if (itemsInStack.Count > 0)
            {
                return itemsInStack[0];
            }
            else
            {
                return null;
            }
        }
    }
}