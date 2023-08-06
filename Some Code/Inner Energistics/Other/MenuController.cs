using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Energistics.Behaviour;
using System;
using Energistics.Player;

namespace Energistics.UI
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] CanvasGroup pauseMenu;
        [SerializeField] CanvasGroup deathMenu;
        [SerializeField] CanvasGroup soundMenu;

        [SerializeField] PlayerStats stats;

        public bool IsPaused { get { return Time.timeScale == 0F; } }

        bool escape = false;
        bool goBack = false;
        bool goSoundMenu = false;

        InventoryUI inventoryUI;
        StateMachine stateMachine;
        private void Awake()
        {
            inventoryUI = FindObjectOfType<InventoryUI>();
            stateMachine = new StateMachine();

            var paused = new IsPaused(this);
            var dead = new IsDead(this);
            var onSoundMenu = new OnSoundMenu(this);
            var menuInactive = new MenuInactive(this, inventoryUI);

            stateMachine.AddTransition(menuInactive, paused, Escape());
            stateMachine.AddTransition(paused, onSoundMenu, SoundMenuActive());
            stateMachine.AddTransition(onSoundMenu, paused, Escape());
            stateMachine.AddTransition(paused, menuInactive, Escape());
            stateMachine.AddTransition(menuInactive, dead, Dead());

            Func<bool> Escape() => () => escape || goBack;
            Func<bool> SoundMenuActive() => () => goSoundMenu;
            Func<bool> Dead() => () => stats.IsDead;

            stateMachine.SetState(menuInactive);

            pauseMenu.gameObject.SetActive(false);
            deathMenu.gameObject.SetActive(false);
            soundMenu.gameObject.SetActive(false);
        }
        private void Update()
        {
            stateMachine.Tick();
        }
        public void SoundMenuActive(bool enabled)
        {
            if (enabled)
            {
                soundMenu.gameObject.SetActive(true);
                soundMenu.alpha = 1;
                soundMenu.blocksRaycasts = true;
            }
            else
            {
                soundMenu.gameObject.SetActive(false);
                soundMenu.alpha = 0;
                soundMenu.blocksRaycasts = false;
            }
        }
        public void DeathMenuActive(bool enabled)
        {
            if (enabled)
            {
                deathMenu.gameObject.SetActive(true);
                deathMenu.alpha = 1;
                deathMenu.blocksRaycasts = true;
            }
            else
            {
                deathMenu.gameObject.SetActive(false);
                deathMenu.alpha = 0;
                deathMenu.blocksRaycasts = false;
            }
        }
        public void PauseMenuActive(bool enabled)
        {
            if (enabled)
            {
                pauseMenu.gameObject.SetActive(true);
                pauseMenu.alpha = 1;
                pauseMenu.blocksRaycasts = true;
            }
            else
            {
                pauseMenu.gameObject.SetActive(false);
                pauseMenu.alpha = 0;
                pauseMenu.blocksRaycasts = false;
            }
        }
        public void EscapeInput(InputAction.CallbackContext action)
        {
            if (action.performed)
            {
                 escape = true;
            }
        }
        public void ResetEscapeInput()
        {
            escape = false;
        }
        public void BackButton()
        {
            goBack = true;
        }
        public void WentBack()
        {
            goBack = false;
        }
        public void SoundMenuButton()
        {
            goSoundMenu = true;
        }
        public void WentSoundMenu()
        {
            goSoundMenu = false;
        }
    }
    public class IsPaused : IState
    {
        MenuController controller;
        public IsPaused(MenuController _controller)
        {
            controller = _controller;
        }
        public void OnEnter()
        {
            Time.timeScale = 0f;
            controller.ResetEscapeInput();
            controller.WentBack();
            controller.PauseMenuActive(true);
            controller.GetComponent<PlayerMovement>().MouseLookEnabled(false);
        }

        public void OnExit()
        {
            controller.ResetEscapeInput();
            controller.PauseMenuActive(false);
        }

        public void Tick()
        {
            return;
        }
    }
    public class IsDead : IState
    {
        MenuController controller;
        public IsDead(MenuController _controller)
        {
            controller = _controller;
        }
        public void OnEnter()
        {
            controller.ResetEscapeInput();
            controller.DeathMenuActive(true);
            controller.GetComponent<PlayerMovement>().MouseLookEnabled(false);
        }

        public void OnExit()
        {
            controller.DeathMenuActive(false);
        }

        public void Tick()
        {
            return;
        }
    }
    public class OnSoundMenu : IState
    {
        MenuController controller;
        public OnSoundMenu(MenuController _controller)
        {
            controller = _controller;
        }
        public void OnEnter()
        {
            controller.ResetEscapeInput();
            controller.SoundMenuActive(true);
            controller.WentSoundMenu();
            controller.GetComponent<PlayerMovement>().MouseLookEnabled(false);
        }

        public void OnExit()
        {
            controller.SoundMenuActive(false);
        }

        public void Tick()
        {
            return;
        }
    }
    public class MenuInactive : IState
    {
        MenuController controller;
        InventoryUI inventoryUI;
        public MenuInactive(MenuController _controller, InventoryUI _inventoryUI)
        {
            controller = _controller;
            inventoryUI = _inventoryUI;
        }
        public void OnEnter()
        {
            Time.timeScale = 1f;
            controller.ResetEscapeInput();
            controller.PauseMenuActive(false);
            controller.DeathMenuActive(false);
            controller.SoundMenuActive(false);
            controller.WentBack();
            controller.WentSoundMenu();
            if (inventoryUI.Open == false)
            {
                controller.GetComponent<PlayerMovement>().MouseLookEnabled(true);
            }
        }

        public void OnExit()
        {
            return;
        }

        public void Tick()
        {
            return;
        }
    }
}