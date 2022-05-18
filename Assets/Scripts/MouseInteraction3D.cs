using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInteraction3D : MonoBehaviour
{
    public float minInteractDist = 5f;
    public GameObject storageUI;
    public GameObject connectedUI;

    private EventManager _eventManager;

    private Camera _mainCamera;
    private Renderer _renderer;
    private Outline _outline;
    private GameObject _player;

    private Ray _ray;
    private RaycastHit _hit;

    // Start is called before the first frame update
    void Start()
    {
        _eventManager = GameObject.Find("EventSystem").GetComponent<EventManager>();

        _mainCamera = Camera.main;
        _renderer = GetComponent<Renderer>();
        _outline = GetComponent<Outline>();
        _player = GameObject.FindGameObjectWithTag("Player");

        _outline.enabled = false;
    }

    void Update()
    {
        //cast a ray out from the mouse
        _ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        //get object hit by raycast
        if (Physics.Raycast(_ray, out _hit, 1000f))
        {
            //if the object hit is this one
            if (_hit.transform == transform)
            {
                //check to see how far away the player is
                float distToPlayer = Vector3.Distance(_player.transform.position, transform.position);

                //if within the interactable distance, it can be used
                if (distToPlayer < minInteractDist)
                {
                    //enable the outline
                    _outline.enabled = true;

                    //check for clicking
                    if (Input.GetMouseButtonDown(0))
                    {
                        //open associated interfaces
                        if (connectedUI != null)
                        {
                            storageUI.SetActive(true); //always open the storage
                            connectedUI.SetActive(true); //open the pen unique to this

                            //notify of a UI opening event
                            _eventManager.OnUIOpened?.Invoke(this, EventArgs.Empty);
                        }

                        Debug.Log(transform.name + " was clicked.");
                    }
                }
            }
            //if not hovered, remove the outline
            else
                _outline.enabled = false;
        }
    }
}
