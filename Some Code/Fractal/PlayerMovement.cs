using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fractal.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public float speed = 5f;
        Rigidbody2D body;
        Controls controls;
        private void Awake()
        {
            controls = new Controls();
            body = GetComponent<Rigidbody2D>();
        }
        private void OnEnable()
        {
            controls.Enable();
        }
        private void OnDisable()
        {
            controls.Disable();
        }
        void Update()
        {
            float step = speed * Time.deltaTime;
            Vector3 direction = controls.Keyboard.Move.ReadValue<Vector2>();
            direction = controls.Touch.Move.ReadValue<Vector2>();
            Vector3 destination = this.transform.position + direction;
            this.transform.position = Vector3.MoveTowards(transform.position, destination, step);
        }
    }
}

