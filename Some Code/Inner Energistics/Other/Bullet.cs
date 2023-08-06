using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Energistics.Player;

namespace Energistics.Behaviour
{
    public class Bullet : MonoBehaviour
    {
        List<GameObject> hits = new List<GameObject>();
        [SerializeField] BulletDecal decal;
        [SerializeField] int hardness = 3;
        [SerializeField] float damageFalloff = 2f;
        float damage;
        Gun currentParentGun;
        public BulletDecal Decal { get { return decal; } }
        bool mustMove = false;
        float bulletSpeedInverse;
        Vector3 start;
        Vector3 destination;
        float movementProgess;
        TrailRenderer trail;
        private void Awake()
        {
            trail = GetComponent<TrailRenderer>();
        }
        void Update()
        {
            if (mustMove)
            {
                movementProgess += Time.deltaTime / bulletSpeedInverse;
                Vector3 tempVector = destination - start;
                transform.position = Vector3.Lerp(start, tempVector.normalized * 1000, movementProgess);
                trail.enabled = true;
            }
        }
        public void MoveBullet(Vector3 _start, Vector3 _destination, float _timer, float _bulletSpeed, float _damage)
        {
            damage = _damage;
            start = _start;
            destination = _destination;
            movementProgess = _timer;
            bulletSpeedInverse = _bulletSpeed;
            trail.enabled = false;
            mustMove = true;
        }
        private void OnDisable()
        {
            mustMove = false;
            hits.Clear();
        }
        private void OnTriggerEnter(Collider other)
        {
            hits.Add(other.gameObject);
            int collateralCount = 0;
            IDamageable damageable;
            if (hits[collateralCount].TryGetComponent<IDamageable>(out damageable))
            {
                collateralCount++;
                hardness -= damageable.Hardness;
                if (hits.Count > 1)
                {
                    damageable.Damage(damage*(hardness/(damageFalloff *10)), this.gameObject);
                }
            }
            if (hardness <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}