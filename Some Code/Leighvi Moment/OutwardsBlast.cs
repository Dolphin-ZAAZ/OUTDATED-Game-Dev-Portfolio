using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutwardsBlast : MonoBehaviour
{
    [Header("Logic")]
    [SerializeField] PointEffector2D blast;
    [SerializeField] float forceMag;
    [SerializeField] float forceApply;
    [SerializeField] float forceApplyTime;
    [SerializeField] bool apply;
    [SerializeField] float cooldownTimer;
    [SerializeField] float cooldown;

    [Header("Effects")]
    [SerializeField] ParticleSystem explosion;
    [SerializeField] GameObject light;
    [SerializeField] AudioSource sound;
    bool played;
    float timer = 0.25f;

    Text blastText;
    Image blastScalar;
    Image blastGrey;
    Image blastControlGrey;

    // Start is called before the first frame update
    void Start()
    {
        blast = GetComponent<PointEffector2D>();
        explosion = GameObject.Find("OutwardsParticles").GetComponent<ParticleSystem>();
        light = GameObject.Find("OutwardsLight");
        sound = GameObject.Find("OutwardsSound").GetComponent<AudioSource>();
        light.SetActive(false);

        blastText = GameObject.Find("BlastText").GetComponent<Text>();
        blastGrey = GameObject.Find("BlastGrey").GetComponent<Image>();
        blastScalar = GameObject.Find("BlastScalar").GetComponent<Image>();
        blastControlGrey = GameObject.Find("BlastControlGrey").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        cooldownTimer -= Time.deltaTime;
        forceApply -= Time.deltaTime;
        timer -= Time.deltaTime;
        if (Input.GetButtonDown("Right Click Ability") && cooldownTimer <= 0 && !GameObject.Find("Canvas").GetComponent<UserInterface>().paused)
        {
            apply = true;
            cooldownTimer = cooldown;
            forceApply = forceApplyTime;
            timer = 0.27f;
            sound.Play();
        }
        else if (Input.GetButtonDown("Right Click Ability") && !GameObject.Find("Canvas").GetComponent<UserInterface>().paused)
        {
            GameObject.Find("ErrorSound").GetComponent<AudioSource>().Play();
        }
        if (cooldownTimer > 0)
        {
            GameObject.Find("SpawningSystem").GetComponent<Spawning>().globalRightActivate = true;
            blastText.gameObject.SetActive(true);
            blastText.text = "" + Mathf.FloorToInt(cooldownTimer + 1);
            blastText.color = new Color(1f, 1f, 1f, 1f);

            blastGrey.gameObject.SetActive(true);
            blastControlGrey.gameObject.SetActive(true);
            blastScalar.gameObject.SetActive(true);
            blastScalar.fillAmount = cooldownTimer / cooldown;
        }
        else
        {
            GameObject.Find("SpawningSystem").GetComponent<Spawning>().globalRightActivate = false;
            blastText.gameObject.SetActive(false);
            blastGrey.gameObject.SetActive(false);
            blastControlGrey.gameObject.SetActive(false);
            blastScalar.gameObject.SetActive(false);
        }
        if (apply && forceApply > 0)
        {
            blast.forceMagnitude = forceMag;
            if (!played)
            {
                explosion.Play();
                played = true;
            }
            if (timer > 0)
            {
                light.SetActive(true);
            }
            else
            {
                light.SetActive(false);
            }

        }
        if (forceApply <= 0 && apply)
        {
            apply = false;
            blast.forceMagnitude = 0f;
            played = false;
        }
        
    }
}
