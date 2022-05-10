using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* This StorageUI will be the front-end to the StorageManager.
 * 
 * StorageManager will contain all of the values required here to generate the appropriate UI.
 * This script will reference StorageManager one-way as I want to avoid co-dependency. The storage
 * size will be set in the StorageManager.
 * 
 * This script wont even store the slots, it will only read the information contained in StorageManager
 * and dynamically update to fit. As such, there needs to be a Refresh UI function to ensure that
 * whenever a refresh signal is emitted, the UI updates.
 * 
 * EventManager is a script located on EventSystem in the hierarchy
 */

public class StorageUI : MonoBehaviour
{
    public GameObject connectedStorage;
    public GameObject slotsPanel;
    public GameObject slotPrefab;

    private EventManager _eventManager;

    public GameObject[] slotsArray;
    private int storageCapacity;

    private void Start()
    {
        //subscribe to the OnRefreshRequested event
        _eventManager = GameObject.Find("EventSystem").GetComponent<EventManager>();
        _eventManager.OnRefreshRequested += EventManager_OnRefreshRequested;

        //get the storage capacity from manager
        storageCapacity = connectedStorage.GetComponent<StorageManager>().maxStorageCapacity;

        //initialise slots array size
        slotsArray = new GameObject[storageCapacity];

        //generate the slots
        GenerateSlots();
    }

    private void GenerateSlots()
    {
        //generate slots based on the storage size
        for (int i = 0; i < storageCapacity; i++)
        {
            var createdSlot = Instantiate(slotPrefab);
            createdSlot.transform.SetParent(slotsPanel.transform, false);
            slotsArray[i] = createdSlot;
        }

        _eventManager.OnRefreshRequested?.Invoke(this, EventArgs.Empty);
    }

    //at some point, the bottom part of the interface will be the data window - if a slot is moused over,
    //the window will populate with the creature's info. this might require the slots communicating
    //with this script, maybe by sending themselves via an event, to tell this UI to update the info box

    private string RetrieveSlotInfo(GameObject slot)
    {
        //retrieve creature information from this slot, if any
        int posInArray = System.Array.IndexOf(slotsArray, slot);
        Creature targetCreature = connectedStorage.GetComponent<StorageManager>().creatureStorageArray[posInArray];

        if (targetCreature != null)
            return targetCreature.infoText;
        else
            return "No Creature in this Slot.";
    }

    private void EventManager_OnRefreshRequested(object sender, EventArgs e)
    {
        //refresh the UI here
        //Debug.Log("Refresh requested from " + sender);

        for (int i = 0; i < slotsArray.Length; i++)
        {
            try
            {
                //set the icon
                slotsArray[i].transform.GetChild(0).GetComponent<RawImage>().enabled = true;
                slotsArray[i].transform.GetChild(0).GetComponent<RawImage>().texture = connectedStorage.GetComponent<StorageManager>().creatureStorageArray[i].variant.variantIcon;
            }
            catch
            {
                slotsArray[i].transform.GetChild(0).GetComponent<RawImage>().enabled = false;
                slotsArray[i].transform.GetChild(0).GetComponent<RawImage>().texture = null;
            }


        }
    }

}
