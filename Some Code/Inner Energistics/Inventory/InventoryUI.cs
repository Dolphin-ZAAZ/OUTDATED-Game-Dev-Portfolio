using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Energistics.Player;
using Energistics.UI;

namespace Energistics.Behaviour
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] PlayerMovement playerMovement;
        [SerializeField] CanvasGroup optionsMenu;
        [SerializeField] CanvasGroup inventorySlotsVisuals;
        MenuController menuController;
        bool inventoryOpen;
        bool inventoryLocked;
        public bool Open { get { return inventoryOpen; } }
        public bool Locked { get { return inventoryLocked; } }
        public bool optionOpen { get { return optionsMenu.alpha == 1; } }
        private void Awake()
        {
            menuController = FindObjectOfType<MenuController>();
        }
        public void OpenInventory()
        {
            if (menuController.IsPaused == true)
            {
                return;
            }
            inventorySlotsVisuals.gameObject.SetActive(true);
            inventorySlotsVisuals.alpha = 1;
            inventoryOpen = true;
        }
        public void CloseInventory()
        {
            if (menuController.IsPaused == true)
            {
                return;
            }
            inventorySlotsVisuals.gameObject.SetActive(false);
            inventorySlotsVisuals.alpha = 0;
            inventoryOpen = false;
            inventoryLocked = false;
            CloseOptionsMenu();
            playerMovement.MouseLookEnabled(true);
        }
        public void LockInventory()
        {
            if (menuController.IsPaused == true)
            {
                return;
            }
            inventoryLocked = true;
            playerMovement.MouseLookEnabled(false);
        }
        public void OpenOptionsMenu()
        {
            if (menuController.IsPaused == true)
            {
                return;
            }
            optionsMenu.gameObject.SetActive(true);
            optionsMenu.alpha = 1;
            optionsMenu.blocksRaycasts = true;
            Vector3 selectionLocation = Input.mousePosition;
            optionsMenu.transform.SetPositionAndRotation(new Vector3(
                selectionLocation.x - optionsMenu.GetComponent<RectTransform>().rect.xMin,
                selectionLocation.y + optionsMenu.GetComponent<RectTransform>().rect.yMin,
                selectionLocation.z), Quaternion.identity);
        }
        public void CloseOptionsMenu()
        {
            if (menuController.IsPaused == true)
            {
                return;
            }
            optionsMenu.gameObject.SetActive(false);
            optionsMenu.alpha = 0;
            optionsMenu.blocksRaycasts = false;
        }
    }
}