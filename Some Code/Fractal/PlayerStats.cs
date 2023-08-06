using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Fractal.Player
{
    public class PlayerStats : MonoBehaviour
    {
        [Header("Player Stats")]
        public int health = 10;
        public int size = 10;
        public int level = 1;

        public int healthIncrement = 1;
        public int sizeIncrement = 1;
        public int levelIncrement = 100;
        public TMP_Text textHealth;
        public TMP_Text textSize;

        [HideInInspector]
        BoxCollider2D hitBox;

        private void Awake()
        {
            hitBox = GetComponent<BoxCollider2D>();
        }
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.GetComponent<Blob>())
            {
                Blob currentBlob = collider.gameObject.GetComponent<Blob>();
                if (health == size)
                {
                    size += sizeIncrement * currentBlob.blobValue;
                    health += healthIncrement * currentBlob.blobValue;
                }
                else
                {
                    health += healthIncrement * currentBlob.blobValue;
                }
                textSize.text = size.ToString();
                textHealth.text = health.ToString();
                transform.localScale = new Vector3(size/10f, size/10f, 1f);
                Destroy(currentBlob.gameObject);
            }
        }
    }
}

