using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private EventManager _eventManager;

    private Transform _cameraTransform;
    private Transform _pivotTransform;

    private Vector3 _localRotation;
    private float _cameraDistance = 10f;

    private bool _uiIsOpen = false;

    public Transform cameraTarget;

    //modifiable properties
    public float mouseSensitivity = 4f;
    public float scrollSensitivity = 2f;

    public float orbitDamp = 10f;
    public float scrollDamp = 6f;

    public float closestZoom = 1.5f;
    public float furthestZoom = 25f;

    public float lowestPitch = 0f;
    public float highestPitch = 90f;

    void Start()
    {
        //subscribe to UI related events
        _eventManager = GameObject.Find("EventSystem").GetComponent<EventManager>();
        _eventManager.OnUIClosed += EventManager_OnUIClosed;
        _eventManager.OnUIOpened += EventManager_OnUIOpened;

        _cameraTransform = transform;
        _pivotTransform = transform.parent;
    }

    void LateUpdate()
    {
        //only allow camera interactions when not in UI
        if (_uiIsOpen == false)
        {
            //if middle mouse is held down
            if (Input.GetMouseButton(2))
            {
                //allow camera movement
                if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
                {
                    _localRotation.x += Input.GetAxis("Mouse X") * mouseSensitivity;
                    _localRotation.y -= Input.GetAxis("Mouse Y") * mouseSensitivity;

                    //clamp y between horizon and perpendicular to floor
                    _localRotation.y = Mathf.Clamp(_localRotation.y, lowestPitch, highestPitch);
                }
            }

            //zoom the camera via scrolling
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                float scrollAmount = Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;

                //scroll out faster the further away we are from it and visa versa
                scrollAmount *= (_cameraDistance * 0.3f);

                //invert the distance to get intended behaviour
                _cameraDistance += scrollAmount * -1f;

                //clamp the zoom distance
                _cameraDistance = Mathf.Clamp(_cameraDistance, closestZoom, furthestZoom);
            }

            //set the camera transformations
            Quaternion QT = Quaternion.Euler(_localRotation.y, _localRotation.x, 0);
            _pivotTransform.rotation = Quaternion.Lerp(_pivotTransform.rotation, QT, Time.deltaTime * orbitDamp);

            //if these values are equal we don't want to update them
            if (_cameraTransform.localPosition.z != _cameraDistance * -1f)
            {
                //lerp towards target destination
                _cameraTransform.localPosition = new Vector3(0f, 0f, Mathf.Lerp(_cameraTransform.localPosition.z, _cameraDistance * -1f, Time.deltaTime * scrollDamp));
            }

            _pivotTransform.position = cameraTarget.transform.position;
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
