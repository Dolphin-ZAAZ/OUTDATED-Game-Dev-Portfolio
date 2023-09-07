using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinTrigger : MonoBehaviour
{
    GameObject player;
    GameObject winText;
    public bool hasWon = false;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        winText = GameObject.Find("YouWon");
        winText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator CompleteWait()
    {
        yield return new WaitForSeconds(6f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !hasWon)
        {
            GameObject.Find("Truck").GetComponent<PlayerMovement>().winSoundPlayed = false;
            winText.SetActive(true);
            hasWon = true;
            if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
            {
                StartCoroutine(CompleteWait());
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene(0);
            }
        }
    }
}
