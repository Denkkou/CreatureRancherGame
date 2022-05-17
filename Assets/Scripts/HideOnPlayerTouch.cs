using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnPlayerTouch : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("Collided with Destination Marker");
            gameObject.SetActive(false);           
        }
    }
}
