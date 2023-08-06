using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Energistics.Behaviour;

namespace Energistics.Enemies
{
    public class Spawner : MonoBehaviour
    {
        bool playerInRange = false;
        float spawnTimer = 0;
        [SerializeField] int maxSpawnCount = 20;
        [SerializeField] int waveCountMin = 5;
        [SerializeField] int waveCountMax = 10;
        [SerializeField] float maxSpawnRange = 10f;
        [SerializeField] float minSpawnRange = 5f;
        [SerializeField] GameObject enemyObject;
        [SerializeField] float spawnCountdownLength = 10f;
        [SerializeField] LayerMask floorLayer;
        List<GameObject> enemies = new List<GameObject>();
        List<GameObject> activeEnemies = new List<GameObject>();
        int enemyIndex = 0;

        private void Awake()
        {
            for (int i = 0; i < maxSpawnCount; i++)
            {
                GameObject tempEnemy = Instantiate(enemyObject, transform.position, Quaternion.identity, transform);
                enemies.Add(tempEnemy);
                tempEnemy.transform.parent = null;
                tempEnemy.SetActive(false);
            }
            activeEnemies.Clear();
        }
        public void RemoveActiveEnemy(GameObject enemy)
        {
            activeEnemies.Remove(enemy);
        }
        private GameObject NextEnemy()
        {
            enemyIndex++;
            if (enemyIndex > enemies.Count - 1)
            {
                enemyIndex = 0;
            }
            return enemies[enemyIndex];
        }
        private void Update()
        {
            spawnTimer -= Time.deltaTime;
            if (playerInRange)
            {
                if (spawnTimer <= 0)
                {
                    if (activeEnemies.Count < maxSpawnCount)
                    {
                        int waveSize = Mathf.RoundToInt(Random.Range(waveCountMin, waveCountMax));
                        RaycastHit hit;
                        Physics.Raycast(transform.position, Vector3.down, out hit, 10000, floorLayer);
                        for (int i = 0; i < waveSize; i++)
                        {
                            Vector3 randomPoint = Random.onUnitSphere;
                            randomPoint = new Vector3(randomPoint.x + transform.position.x, hit.point.y + 0.5f, randomPoint.z + transform.position.z);
                            randomPoint = randomPoint.normalized * Random.Range(minSpawnRange, maxSpawnRange);
                            Vector3 spawnPoint = randomPoint;
                            GameObject nextEnemy = NextEnemy();
                            nextEnemy.transform.position = spawnPoint;
                            nextEnemy.SetActive(true);
                            activeEnemies.Add(nextEnemy);
                        }
                        spawnTimer = spawnCountdownLength;
                    }
                }
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = true;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = false;
            }
        }
    }
}