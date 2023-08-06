using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using Fractal.NPC;
using UnityEngine.UI;

namespace Fractal
{
    public class OverlayHud : MonoBehaviour
    {
        Spawner spawner;

        TMP_Text cellCount;
        TMP_Text speciesCount;
        TMP_Text[] ranks;
        Image[] images;
        Dictionary<int,int> top8;

        private void Awake()
        {
            spawner = FindObjectOfType<Spawner>();
        }
        private void Start()
        {
            cellCount = GameObject.Find("CellCount").GetComponent<TMP_Text>();
            speciesCount = GameObject.Find("SpeciesCount").GetComponent<TMP_Text>();
            ranks = transform.Find("Scoreboard").GetComponentsInChildren<TMP_Text>();
            images = transform.Find("Scoreboard").GetComponentsInChildren<Image>();
            UpdateCellCount();
            UpdateSpeciesCount();
            top8 = spawner.speciesRank.OrderByDescending(pair => pair.Value).Take(10).ToDictionary(pair => pair.Key, pair => pair.Value);
            UpdateSpeciesRank();
        }
        public void UpdateCellCount()
        {
            cellCount.text = "Cell Count: " + spawner.npcAmount;
        }
        public void UpdateSpeciesCount()
        {
            speciesCount.text = "Species Count: " + spawner.speciesRank.Count;
        }
        public void UpdateSpeciesRank()
        {
            top8 = spawner.speciesRank.OrderByDescending(pair => pair.Value).Take(8).ToDictionary(pair => pair.Key, pair => pair.Value);
            List<int> keys = new List<int>();
            List<int> values = new List<int>();
            foreach (KeyValuePair<int,int> rank in top8)
            {
                keys.Add(rank.Key);
                values.Add(rank.Value);
            }
            for (int i = 0; i < keys.Count; i++)
            {
                if (spawner.speciesRank.Count <= 8)
                {
                    if (values[i] <= 1)
                    {
                        values[i] = 0;
                    }
                }
                ranks[i].text = i+1+": ID-" + keys[i] + " with " + values[i] + " cells";
                images[i + 1].color = spawner.npcs[keys[i] - 1].colorID;
                
            }
        }
    }
}

