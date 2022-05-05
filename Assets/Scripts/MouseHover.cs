using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //reference to event system
    private EventManager _eventManager;

    //hover data
    public bool isHovered;
    private Vector3 _ogScale;
    private float _minMaxScale = 0.05f;
    private float _oscSpeed = 25f;
    private int _oscCounter = 0;

    private void Start()
    {
        //find the event manager on EventSystem
        _eventManager = GameObject.Find("EventSystem").GetComponent<EventManager>();

        //cache original scale
        _ogScale = transform.localScale;
    }

    private void Update()
    {
        if (isHovered)
            WobbleEffect();
        else
            transform.localScale = _ogScale;
    }

    private void WobbleEffect()
    {
        Vector3 tempScale = transform.localScale;

        //use sin function to oscillate
        tempScale.x = _ogScale.x + ((_minMaxScale * Mathf.Sin(_oscCounter / _oscSpeed)));
        tempScale.y = _ogScale.y + ((_minMaxScale * Mathf.Sin(_oscCounter / _oscSpeed)));

        transform.localScale = tempScale;
        _oscCounter++;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        _oscCounter = 0;
    }
}
