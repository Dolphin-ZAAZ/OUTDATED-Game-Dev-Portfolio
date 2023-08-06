using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Energistics.Behaviour
{
    [DisallowMultipleComponent]
    public class Item : MonoBehaviour, IInteractable
    {
        [Header("States")]
        protected bool isDropped;
        protected bool isPickedUp;
        protected bool isEquipped;
        protected bool isFocused;
        protected bool isUsing;
        protected bool isFocusedAndUsing;

        [Header("Item Details")]
        [SerializeField] protected string itemName = "Empty Item";
        [SerializeField] protected Sprite image;
        [SerializeField] protected GameObject mesh;
        [SerializeField] protected bool isConsumable = true;
        [SerializeField] protected int maxItemStack = 36;
        protected bool completedItem;
        public bool IsConsumable { get { return isConsumable; } }
        public int MaxItemStack { get { return maxItemStack; } }
        public string ItemName { get { return itemName; } }

        [Header("Arm Position")]
        [SerializeField] Vector3 armsPosition;
        [SerializeField] Vector3 focusPosition;
        [SerializeField] Vector3 armsRotation;
        [SerializeField] Vector3 focusRotation;
        [SerializeField] float primeSpeed = 2.5f;
        [SerializeField] float zoomAmount = 0;
        public float PrimeSpeed { get { return primeSpeed; } }
        public float ZoomAmount { get { return zoomAmount; } }
        
        [Header("Composition")]
        protected Rigidbody thisRigidbody;
        protected StateMachine stateMachine;
        [SerializeField] protected Animator animator;

        public Vector3 FocusPosition { get { return focusPosition; } }
        public Vector3 ArmsPosition { get { return armsPosition; } }
        public Vector3 FocusRotation { get { return focusRotation; } }
        public Vector3 ArmsRotation { get { return armsRotation; } }
        public GameObject Mesh { get { if (mesh != null) return mesh; else return gameObject; } }
        public Sprite Image { get { return image; } }
        public string Name { get { return itemName; } }
        public Transform Transform { get { return transform; } }
        private void Awake()
        {
            InitializeItem();
        }

        protected void InitializeItem()
        {
            stateMachine = new StateMachine();
            thisRigidbody = GetComponent<Rigidbody>();

            var pickedUp = new IsPickedUp(this);
            var equipped = new IsEquipped(this);
            var focused = new IsFocused(this);
            var inUse = new IsUsing(this);
            var inFocusAndUse = new IsFocusedAndUsing(this);
            var dropped = new IsDropped(this);

            stateMachine.AddTransition(dropped, pickedUp, PickedUp());
            stateMachine.AddTransition(dropped, equipped, Equipped());

            stateMachine.AddTransition(pickedUp, dropped, Dropped());
            stateMachine.AddTransition(pickedUp, equipped, Equipped());

            stateMachine.AddTransition(equipped, pickedUp, PickedUp());
            stateMachine.AddTransition(equipped, dropped, Dropped());
            stateMachine.AddTransition(equipped, inUse, UsingItem());
            stateMachine.AddTransition(equipped, focused, Focused());

            stateMachine.AddTransition(focused, inUse, UsingItem());
            stateMachine.AddTransition(focused, inFocusAndUse, FocusedAndUsing());
            stateMachine.AddTransition(focused, pickedUp, PickedUp());
            stateMachine.AddTransition(focused, dropped, Dropped());
            stateMachine.AddTransition(focused, equipped, Equipped());

            stateMachine.AddTransition(inUse, equipped, Equipped());
            stateMachine.AddTransition(inUse, focused, Focused());
            stateMachine.AddTransition(inUse, pickedUp, PickedUp());
            stateMachine.AddTransition(inUse, dropped, Dropped());
            stateMachine.AddTransition(inUse, inFocusAndUse, FocusedAndUsing());

            stateMachine.AddTransition(inFocusAndUse, focused, Focused());
            stateMachine.AddTransition(inFocusAndUse, inUse, UsingItem());
            stateMachine.AddTransition(inFocusAndUse, equipped, Equipped());
            stateMachine.AddTransition(inFocusAndUse, pickedUp, PickedUp());
            stateMachine.AddTransition(inFocusAndUse, dropped, Dropped());

            Func<bool> PickedUp() => () => isPickedUp;
            Func<bool> Equipped() => () => isEquipped;
            Func<bool> Focused() => () => isFocused;
            Func<bool> UsingItem() => () => isUsing;
            Func<bool> FocusedAndUsing() => () => isFocusedAndUsing;
            Func<bool> Dropped() => () => isDropped;

            stateMachine.SetState(dropped);
            isDropped = true;

            if (mesh == null || image == null || itemName == null || animator == null)
            {
                completedItem = false;
            }
            else
            {
                completedItem = true;
            }
        }

        private void Update()
        {
            stateMachine.Tick();
        }
        public void ResetTriggers()
        {
            isUsing = false;
            isFocusedAndUsing = false;
            isDropped = false;
            isEquipped = false;
            isFocused = false;
            isPickedUp = false;
        }

        public void ItemEnabled(bool value)
        {
            mesh.SetActive(value);
            thisRigidbody.detectCollisions = value;
            thisRigidbody.useGravity = value;
        }
        public void ItemHeld(bool value)
        {
            mesh.SetActive(value);
            thisRigidbody.velocity = Vector3.zero;
            thisRigidbody.freezeRotation = true;
            thisRigidbody.detectCollisions = !value;
            thisRigidbody.useGravity = !value;
        }

        public virtual void OnPickedUp() { ResetTriggers(); isPickedUp = true;  }
        public virtual void OnEquiped() { ResetTriggers(); isEquipped = true; }
        public virtual void OnUse() { ResetTriggers(); isUsing = true;  }
        public virtual void OnFocus() { ResetTriggers(); isFocused = true;  }
        public virtual void OnFocusedAndUsing() { ResetTriggers(); isFocusedAndUsing = true;  }
        public virtual void OnDrop() { ResetTriggers(); isDropped = true;  }
        public virtual void PickedUp() { return; }
        public virtual void Equiped() { return; }
        public virtual void Use() { return; }
        public virtual void Focus() { return; }
        public virtual void Drop() { return; }

        public void OnInteracted(IActor actor)
        {
            return;
        }
    }
    public class IsDropped : IState
    {
        private readonly Item item;
        public IsDropped(Item _item)
        {
            item = _item;
        }
        public void OnEnter()
        {
            Debug.Log("changed dropped");
            item.ItemEnabled(true);
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
    public class IsEquipped : IState
    {
        private readonly Item item;
        public IsEquipped(Item _item)
        {
            item = _item;
        }
        public void OnEnter()
        {
            item.ItemHeld(true);
            item.Equiped(); 
            Debug.Log("changed equipped");
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
    public class IsFocused : IState
    {
        private readonly Item item;
        public IsFocused(Item _item)
        {
            item = _item;
        }
        public void OnEnter()
        {
            item.Focus(); 
            Debug.Log("changed to focus");
        }

        public void OnExit()
        {
            item.ResetTriggers();
        }

        public void Tick()
        {
            return;
        }
    }
    public class IsPickedUp : IState
    {
        private readonly Item item;
        public IsPickedUp(Item _item)
        {
            item = _item;
        }
        public void OnEnter()
        {
            item.ItemEnabled(false);
            Debug.Log("changed picked up");
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
    public class IsUsing : IState
    {
        private readonly Item item;
        public IsUsing(Item _item)
        {
            item = _item;
        }
        public void OnEnter()
        {
            if (item.GetComponent<FullyAutomaticGun>())
            {
                item.GetComponent<FullyAutomaticGun>().ResetAttackDelta();
            }
            else if (item.GetComponent<Gun>())
            {
                item.GetComponent<Gun>().OnUse();
            } 
            Debug.Log("changed to use");
        }

        public void OnExit()
        {
            if (item.GetComponent<FullyAutomaticGun>())
            {
                item.GetComponent<FullyAutomaticGun>().ResetAttackDelta();
            }
        }

        public void Tick()
        {
            if (item.GetComponent<FullyAutomaticGun>())
            {
                item.GetComponent<FullyAutomaticGun>().AutomaticFire();
            }
        }
    }
    public class IsFocusedAndUsing : IState
    {
        private readonly Item item;
        public IsFocusedAndUsing(Item _item)
        {
            item = _item;
        }
        public void OnEnter()
        {
            Debug.Log("changed to focus and using");
            item.Focus();
            if (item.GetComponent<FullyAutomaticGun>())
            {
                item.GetComponent<FullyAutomaticGun>().ResetAttackDelta();
            }
            else if (item.GetComponent<Gun>())
            {
                item.GetComponent<Gun>().OnUse();
            }
        }

        public void OnExit()
        {
            if (item.GetComponent<FullyAutomaticGun>())
            {
                item.GetComponent<FullyAutomaticGun>().ResetAttackDelta();
            }
            item.ResetTriggers();
        }

        public void Tick()
        {
            if (item.GetComponent<FullyAutomaticGun>())
            {
                item.GetComponent<FullyAutomaticGun>().AutomaticFire();
            }
        }
    }
}
