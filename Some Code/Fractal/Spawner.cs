using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fractal.NPC;

namespace Fractal
{
    public class Spawner : MonoBehaviour
    {
        [Header("Spawnables")]
        public GameObject blobObject;
        public GameObject npc;

        [Header("Spawn Info")]
        public int blobSpawnAmount = 1000;
        public float playFieldSize = 1000f;
        public int npcSpawnAmount;
        public int currentBlobValue = 1;

        public int npcAmount = 0;
        int blobAmount = 0;
        public Dictionary<int, int> speciesRank = new Dictionary<int, int>();
        public KdTree<NPCStats> npcs = new KdTree<NPCStats>();
        private void Awake()
        {
            npcs.Clear();
            SpawnBlobWave(blobSpawnAmount);
            SpawnNPCWave(npcSpawnAmount);
        }
        private void Update()
        {
            if (blobAmount < blobSpawnAmount)
            {
                Vector2 blobPosition = Random.insideUnitCircle * playFieldSize / 2;
                Instantiate(blobObject, new Vector3(blobPosition.x, blobPosition.y, 1), Quaternion.identity);
                blobAmount++;
            }
        }
        public void SpawnBlobWave(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Vector2 blobPosition = Random.insideUnitCircle * playFieldSize / 2;
                Blob currentBlob = Instantiate(blobObject, new Vector3(blobPosition.x, blobPosition.y, 1), Quaternion.identity).GetComponent<Blob>();
                blobAmount++;
            }
        }
        public void SpawnNPCWave(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Vector2 npcPosition = Random.insideUnitCircle * playFieldSize / 2;
                npcAmount++;
                NPCStats currentNPC = Instantiate(npc, new Vector3(npcPosition.x, npcPosition.y, 1), Quaternion.identity).GetComponent<NPCStats>();
                currentNPC.npcID = npcAmount;
                speciesRank.Add(npcAmount, 1);
                npcs.Add(currentNPC);
                currentNPC.isChild = false;
            }
        }
    }
}

