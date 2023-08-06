using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Energistics.Behaviour
{
    public class BulletPool : MonoBehaviour
    {
        [Header("Bullet Trail")]
        [SerializeField] int bulletAmount = 500;
        [SerializeField] GameObject bullet;
        int bulletIndex = 0;
        float timeCounter = 0.01f;
        List<GameObject> bullets = new List<GameObject>();

        [Header("Bullet Decal")]
        [SerializeField] int decalAmount = 500;
        [SerializeField] GameObject decal;
        List<GameObject> decals = new List<GameObject>();
        int decalIndex = 0;
        public void SetBullet(GameObject _bullet)
        {
            if (_bullet != null)
            {
                bullet = _bullet;
                SpawnBullets();
                if (bullet.GetComponent<Bullet>().Decal != null)
                {
                    decal = bullet.GetComponent<Bullet>().Decal.gameObject;
                    SpawnDecals();
                    transform.position = new Vector3(30000f, 30000f, 30000f);
                }
            }
        }
        private void SpawnDecals()
        {
            for (int i = 0; i < decalAmount; i++)
            {
                GameObject newDecal = Instantiate(decal);
                decals.Add(newDecal);
                newDecal.transform.parent = transform;
                newDecal.SetActive(false);
                if (i == decalAmount - 1)
                {
                    newDecal.SetActive(true);
                }
            }
        }
        private void SpawnBullets()
        {
            for (int i = 0; i < bulletAmount; i++)
            {
                GameObject newBullet = Instantiate(bullet);
                bullets.Add(newBullet);
                newBullet.transform.parent = transform;
                newBullet.SetActive(false);
                if (i == bulletAmount - 1)
                {
                    newBullet.SetActive(true);
                }
            }
        }

        public GameObject SpawnNextBullet(Vector3 start, Vector3 destination, float timer, float damage)
        {
            for (int i = 0; i < bulletAmount; i++)
            {
                if (bullets[i].activeSelf == true)
                {
                    bulletIndex++;
                    continue;
                }
                else
                {
                    bulletIndex++;
                    if (bulletIndex > bulletAmount - 1)
                    {
                        bulletIndex = 0;
                    }
                    break;
                }
            }
            timeCounter = 0.001f; 
            bullets[bulletIndex].SetActive(true);
            bullets[bulletIndex].GetComponent<Bullet>().MoveBullet(start, destination, timeCounter, timer, damage);
            return bullets[bulletIndex];
        }
        public GameObject SpawnNextDecal(Vector3 destination, Transform parent, Vector3 rotation)
        {
            for (int i = 0; i < decalAmount; i++)
            {
                decalIndex++;
                if (decalIndex > decalAmount - 1)
                {
                    decalIndex = 0;
                }
                break;
            }
            decals[decalIndex].SetActive(true);
            decals[decalIndex].GetComponent<BulletDecal>().PlaceDecal(destination, parent, rotation);
            return decals[decalIndex];
        }
    }
}