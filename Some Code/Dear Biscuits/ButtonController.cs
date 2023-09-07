using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonController: MonoBehaviour, IPointerEnterHandler, ISelectHandler, IPointerExitHandler
{
    Animator animator;
    bool hasPlayed;
    private void Start()
    {
        animator = this.GetComponent<Animator>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetBool("Highlighted", true);
        if (!hasPlayed)
        {
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                GameObject.Find("Canvas").GetComponent<UI_Controller>().hoverSelectPlay();
            }
            else
            {
                GameObject.Find("UI_Controller").GetComponent<UI_Controller>().hoverSelectPlay();
            }
            hasPlayed = true;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetBool("Highlighted", false);
        hasPlayed = false;
    }
    public void OnSelect(BaseEventData eventData)
    {
        //do your stuff when selected
    }
}
