using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneSpawner : MonoBehaviour
{
    Spawning spawning;
    [SerializeField] bool hasSpawned = false;
    [SerializeField] public List<GameObject> spawners;
    [SerializeField] public GameObject[] runes;
    // Start is called before the first frame update
    void Start()
    {
        spawners = new List<GameObject>(GameObject.FindGameObjectsWithTag("RuneSpawner"));
        spawning = GameObject.Find("SpawningSystem").GetComponent<Spawning>();
    }

    // Update is called once per frame
    void Update()
    {
        if (spawning.downtimeTimer <= 0f && !hasSpawned && spawning.startCountdown <= 0f)
        {
            int selectedSpawner = (int)(Random.Range(0, spawners.Count));
            Instantiate(runes[0], spawners[selectedSpawner].transform.position, Quaternion.Euler(0,0,90f));
            spawners.Remove(spawners[selectedSpawner]);
            selectedSpawner = (int)(Random.Range(0, spawners.Count));
            Instantiate(runes[1], spawners[selectedSpawner].transform.position, Quaternion.Euler(0, 0, 90f));
            spawners.Remove(spawners[selectedSpawner]);
            selectedSpawner = (int)(Random.Range(0, spawners.Count));
            Instantiate(runes[2], spawners[selectedSpawner].transform.position, Quaternion.Euler(0, 0, 90f));
            spawners.Remove(spawners[selectedSpawner]);
            selectedSpawner = (int)(Random.Range(0, spawners.Count));
            Instantiate(runes[3], spawners[selectedSpawner].transform.position, Quaternion.Euler(0, 0, 90f));
            spawners.Remove(spawners[selectedSpawner]);
            hasSpawned = true;
        }
        if (spawning.downtimeTimer > 0f)
        {
            spawners = new List<GameObject>(GameObject.FindGameObjectsWithTag("RuneSpawner"));
            hasSpawned = false;
        }
    }
}
