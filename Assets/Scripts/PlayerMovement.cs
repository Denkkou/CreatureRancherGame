using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    public GameObject destinationMarker;

    private EventManager _eventManager;
    private bool _uiIsOpen = false;

    NavMeshAgent agent;
    
    void Start()
    {
        _eventManager = GameObject.Find("EventSystem").GetComponent<EventManager>();

        //subscribe to UI open and close events
        _eventManager.OnUIClosed += EventManager_OnUIClosed;
        _eventManager.OnUIOpened += EventManager_OnUIOpened;

        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {   
        //only allow movement and world interaction when not in UI
        if (_uiIsOpen == false)
        {
            if (Input.GetMouseButtonDown(0))
            {

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                //when clicked, cast a ray out from the screen
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if (hit.transform.tag == "Walkable")
                    {
                        //set destination for the agent
                        agent.SetDestination(hit.point);

                        //move marker & unhide
                        destinationMarker.transform.position = hit.point;
                        destinationMarker.SetActive(true);
                    } else
                        Debug.Log("Player clicked invalid destination");
                }
            }
        }
    }

    private void EventManager_OnUIClosed(object sender, EventArgs e)
    {
        _uiIsOpen = false;
    }

    private void EventManager_OnUIOpened(object sender, EventArgs e)
    {
        _uiIsOpen = true;
    }
}
