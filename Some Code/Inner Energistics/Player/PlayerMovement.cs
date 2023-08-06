using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Energistics.Behaviour;

namespace Energistics.Player
{
    [RequireComponent(typeof(CharacterController))]
    [DisallowMultipleComponent]
    public class PlayerMovement : MonoBehaviour
    {
        StateMachine stateMachine;
        Animator animator;

        [Header("Horizontal Movement")]
        [SerializeField] float speed = 0.1f;
        [SerializeField] float sprintMultiplier = 1.5f;
        CharacterController character;

        [Header("Mouse Movement")]
        [SerializeField] bool mouseLookEnabled = true;
        [SerializeField] float sensitivity;
        [SerializeField] float xClamp = 80f;
        Camera cam;

        [Header("Gravity and Surface")]
        [SerializeField] float gravity = 9.81f;
        [SerializeField] float groundCheckRadius = 0.1f;
        [SerializeField] LayerMask playerLayer;

        [Header("Jump")]
        [SerializeField] float jumpHeight = 1f;
        [SerializeField] int jumpCount = 2;

        [Header("Crouch")]
        [SerializeField] bool isCrouching = false;
        [SerializeField] float crouchHeight = 1f;

        [Header("Debug")]
        [SerializeField] bool logState = false;
        [SerializeField] Vector3 verticalVelocity;
        [SerializeField] Vector3 horizontalVelocity;
        [SerializeField] float sprintSpeed;
        [SerializeField] Vector2 rawMovementInput;
        [SerializeField] float mouseX;
        [SerializeField] float clampedY = 0;
        [SerializeField] float mouseY;
        [SerializeField] int jumpMulti;
        [SerializeField] bool isGrounded;
        [SerializeField] bool gravityEnabled;

        bool nullified = false;
        float nullifiedTimer = 0.1f;

        [Header("Mouse Mover")]
        [SerializeField] float moverSpeed = 50;
        bool mustMove = false;
        float verticalMoverCounter = 0;
        float horizontalMoverCounter = 0;
        float moverVertical = 0;
        float moverHorizontal = 0;
        public IState GetState()
        {
            return stateMachine.GetState();
        }
        private void Awake()
        {
            mouseLookEnabled = true;
            stateMachine = new StateMachine();
            stateMachine.logState = logState;
            animator = GetComponent<Animator>();
            character = GetComponent<CharacterController>();
            cam = GetComponentInChildren<Camera>();
            jumpMulti = jumpCount;
            sprintSpeed = speed * sprintMultiplier;

            var idle = new IsIdle(animator);
            var walking = new IsWalking(this, animator);
            var sprinting = new IsSprinting(this, animator);
            var inAir = new InAir(this, animator);

            stateMachine.AddTransition(idle, walking, IsWalking());
            stateMachine.AddTransition(idle, sprinting, IsSprinting());
            stateMachine.AddTransition(idle, inAir, IsJumping());

            stateMachine.AddTransition(walking, sprinting, IsSprinting());
            stateMachine.AddTransition(walking, inAir, IsJumping());
            stateMachine.AddTransition(walking, inAir, IsFalling());
            stateMachine.AddTransition(walking, idle, IsStationary());

            stateMachine.AddTransition(sprinting, walking, IsWalking());
            stateMachine.AddTransition(sprinting, inAir, IsJumping());
            stateMachine.AddTransition(sprinting, inAir, IsFalling());
            stateMachine.AddTransition(sprinting, idle, IsStationary());

            stateMachine.AddTransition(inAir, idle, IsStationary());
            stateMachine.AddTransition(inAir, walking, IsWalking());
            stateMachine.AddTransition(inAir, sprinting, IsSprinting());

            Func<bool> IsStationary() => () => horizontalVelocity == Vector3.zero && isGrounded;
            Func<bool> IsWalking() => () => speed != sprintSpeed && horizontalVelocity != Vector3.zero && isGrounded;
            Func<bool> IsSprinting() => () => speed == sprintSpeed && horizontalVelocity != Vector3.zero && isGrounded;
            Func<bool> IsJumping() => () => verticalVelocity.y > 0f;
            Func<bool> IsFalling() => () => !isGrounded && verticalVelocity.y <= 0;

            stateMachine.SetState(idle);

            isGrounded = true;
        }
        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            mustMove = false;
        }
        private void Update()
        {
            character.Move(verticalVelocity * Time.deltaTime);
            stateMachine.Tick();
            if (gravityEnabled)
            {
                ApplyGravity();
            }
            else
            {
                verticalVelocity.y = -1f;
            }
            CheckGrounded();
            if (nullified == false)
            {
                HorizontalMovement();
            }
            else
            {
                character.Move(horizontalVelocity * speed * Time.deltaTime);
            }
            nullifiedTimer -= Time.fixedDeltaTime;
            if (nullifiedTimer < 0)
            {
                nullified = false;
            }
            if (mouseLookEnabled == true)
            {
                MouseLook();
            }
            if (mustMove)
            {
                MouseMover();
            }
        }
        public bool CheckGrounded()
        {
            isGrounded = Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - character.height / 2, transform.position.z), groundCheckRadius, ~playerLayer);
            return isGrounded;
        }

        public Vector3 GetVerticalVelocity()
        {
            return verticalVelocity;
        }
        public void NullifyMovement(float delay, float force)
        {
            if (nullified == false)
            {
                horizontalVelocity = -horizontalVelocity - (horizontalVelocity * force);
                nullified = true;
                nullifiedTimer = Time.fixedDeltaTime * delay;
            }
        }
        
        public void HorizontalMovement()
        {
            if (nullified == false)
            {
                horizontalVelocity = transform.right * rawMovementInput.x + transform.forward * rawMovementInput.y;
                character.Move(horizontalVelocity * speed * Time.deltaTime);
            }
        }

        public void ResetJump()
        {
            jumpMulti = jumpCount;
        }
        public void EnableGravity()
        {
            gravityEnabled = true;
        }
        public void DisableGravity()
        {
            gravityEnabled = false;
        }
        public void ApplyGravity()
        {
            verticalVelocity.y -= gravity * Time.deltaTime;
        }
        public float GetGravity()
        {
            return gravity;
        }

        public void MouseLookEnabled(bool enabled)
        {
            mouseLookEnabled = enabled;
            if (enabled)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        public void MouseLook()
        {
            transform.Rotate(new Vector3(0, mouseX * Time.deltaTime * sensitivity, 0), Space.World);
            clampedY -= mouseY * Time.deltaTime * sensitivity;
            clampedY = Mathf.Clamp(clampedY, -xClamp, xClamp);
            cam.transform.eulerAngles = new Vector3(clampedY, transform.eulerAngles.y, 0);
        }
        public void MoveMouse(float _vertical, float _horizontal)
        {
            mustMove = true;
            moverVertical = _vertical;
            moverHorizontal = _horizontal;
        }

        private void MouseMover()
        {
            clampedY -= Time.deltaTime * moverSpeed;
            verticalMoverCounter -= Time.deltaTime * moverSpeed;
            if (moverHorizontal > 0f)
            {
                horizontalMoverCounter += Time.deltaTime * (moverSpeed / 5);
                if (horizontalMoverCounter >= moverHorizontal)
                {
                    moverHorizontal = 0f;
                    horizontalMoverCounter = 0f;
                }
            }
            else if (moverHorizontal < 0f)
            {
                horizontalMoverCounter -= Time.deltaTime * (moverSpeed / 5);
                if (horizontalMoverCounter <= moverHorizontal)
                {
                    moverHorizontal = 0f;
                    horizontalMoverCounter = 0f;
                }
            }
            if (verticalMoverCounter <= moverVertical)
            {
                mustMove = false;
                verticalMoverCounter = 0f;
            }
            transform.Rotate(new Vector3(0, moverHorizontal * (Time.deltaTime * (moverSpeed / 5)), 0));
        }

        public void OnMovement(InputAction.CallbackContext value)
        {
            rawMovementInput = value.ReadValue<Vector2>();
        }
        public void OnMouseLookX(InputAction.CallbackContext value)
        {
            mouseX = value.ReadValue<float>();
        }
        public void OnMouseLookY(InputAction.CallbackContext value)
        {
            mouseY = value.ReadValue<float>();
        }
        public void OnJump(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                if (jumpMulti >= 1)
                {
                    if (verticalVelocity.y < 0f)
                    {
                        verticalVelocity.y += -verticalVelocity.y + Mathf.Sqrt(2f * jumpHeight * gravity);
                    }
                    else
                    {
                        verticalVelocity.y += Mathf.Sqrt(2f * jumpHeight * gravity);
                    }
                    jumpMulti--;
                    animator.SetTrigger("switchJump");
                    if (jumpMulti%2 == 0)
                    {
                        animator.ResetTrigger("switchJump");
                    }
                }
            }
        }
        public void OnSprint(InputAction.CallbackContext value)
        {
            speed = sprintSpeed;
            if (value.canceled)
            {
                speed = sprintSpeed / sprintMultiplier;
            }
        }

        public void OnCrouch(InputAction.CallbackContext value)
        {
            isCrouching = true;

            if (value.canceled)
            {
                isCrouching = false;
            }
        }
    }
}

