using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This StorageManager script will be responsible for all of the data processing and storage.
 * 
 * It will reference little else if possible, and use events to communicate outward. The StorageUI
 * will be a projection of the data stored here, and will refresh when requested by OnRefreshRequested event.
 *
 * Any creature generation code will be handled via a generation script attached to the same game object
 * if appropriate (such as in the breeding pen or for adding to storage
 */

public abstract class StorageManager : MonoBehaviour
{

    public int maxStorageCapacity;
    public Creature[] creatureStorageArray;

    private EventManager _eventManager;

    //permissions for units
    public bool unrestrictedStorage;
    public CreatureSize allowedSize;
    public CreatureType allowedType;

    private bool swapOccuredOnPrevFrame = false;

    private void Start()
    {
        //find the event manager on EventSystem
        _eventManager = GameObject.Find("EventSystem").GetComponent<EventManager>();

        //subscribe to the swap event
        _eventManager.OnSwapRequested += EventManager_OnSwapRequested;

        //null-initialise the storage array
        creatureStorageArray = new Creature[maxStorageCapacity];

        for (int i = 0; i < creatureStorageArray.Length; i++)
            creatureStorageArray[i] = null;

        //refresh any interfaces
        EmitRefreshRequest();
    }

    private void Update()
    {
        //if last frame a swap occured, refresh this frame
        if (swapOccuredOnPrevFrame)
        {
            EmitRefreshRequest();
            swapOccuredOnPrevFrame = false;
        }
    }

    //storage manager functions
    public void AddCreature(Creature creature)
    {
        //find the first free space, a return of -1 is the fail condition
        if (FindFirstEmptyIndex() > -1)
            creatureStorageArray[FindFirstEmptyIndex()] = creature;
        else
            Debug.Log("There is no space available here.");

        EmitRefreshRequest();
    }

    public void RemoveCreature(Creature creature)
    {
        //only remove if there is actually a creature to be removed
        if (creatureStorageArray.Length > 0)
        {
            int indexToRemove = System.Array.IndexOf(creatureStorageArray, creature);
            creatureStorageArray[indexToRemove] = null;
        }
        else
            Debug.Log("There are no creatures stored here.");

        EmitRefreshRequest();
    }

    //receive the two creatures to be swapped
    private void EventManager_OnSwapRequested(object sender, EventManager.OnSwapRequestedEventArgs e)
    {
        Debug.Log("Swapping Creature: " + e.creatureA.creatureID + " and Creature: " + e.creatureB.creatureID);

        //what if the storage manager checks to see which ones are present in its own storage
        //and if one is + one isn't, put the new one in the old one's place
        //if neither are, do nothing
        //if both are, swap positions

        bool isAPresent = false;
        int creatureAPos = -1;
        bool isBPresent = false;
        int creatureBPos = -1;

        //first check is to see if any of the swap candidates are within own storage
        for (int i = 0; i < creatureStorageArray.Length; i++)
        {
            //if present, also store its position in own storage
            if (creatureStorageArray[i] == e.creatureA)
            {
                isAPresent = true;
                creatureAPos = i;
            } 
            else if (creatureStorageArray[i] == e.creatureB)
            {
                isBPresent = true;
                creatureBPos = i;
            }
        }

        //handle the combinations, if neither are present, this storage is not involved
        if (isAPresent == false && isBPresent == false)
            return;

        //if both are in storage, perform an ordinary swap
        if (isAPresent == true && isBPresent == true)
        {
            Creature temp = creatureStorageArray[creatureAPos];
            creatureStorageArray[creatureAPos] = creatureStorageArray[creatureBPos];
            creatureStorageArray[creatureBPos] = temp;
        }

        //if A is in storage, then B is incoming, and needs putting in A's place
        else if (isAPresent == true && isBPresent == false)
        {
            //check if valid here

            creatureStorageArray[creatureAPos] = e.creatureB;
        }

        //if B is in storage, then A is incoming, and needs putting in B's place
        else if (isBPresent == true && isAPresent == false)
        {
            //check if valid here

            creatureStorageArray[creatureBPos] = e.creatureA;
        }

        //state that a swap just occured for the late refresh
        swapOccuredOnPrevFrame = true;
    }

    //helper functions
    private void EmitRefreshRequest()
    {
        _eventManager.OnRefreshRequested?.Invoke(this, EventArgs.Empty);
    }

    private bool IsStorageEmpty()
    {
        for (int i = 0; i < creatureStorageArray.Length; i++)
        {
            if (creatureStorageArray[i] != null)
                return false;
        }

        //if all are null, its empty
        return true;
    }

    private int FindFirstEmptyIndex()
    {
        for (int i = 0; i < creatureStorageArray.Length; i++)
        {
            //so long as one "null" exists, there's space
            if (creatureStorageArray[i] == null)
                return i;
        }

        return -1;
    }

    //dynamic restriction functions
    private void UpdateAcceptedSpecies()
    {
        //if the storage is empty reset the value
        if (IsStorageEmpty() == true)
            allowedType = CreatureType.UNDEFINED;

        //if something is in it, but the accepted isn't set, and also is meant to specify,
        if (unrestrictedStorage == false && IsStorageEmpty() == false && allowedType == CreatureType.UNDEFINED)
        {
            //find the creature added and set the acceptedSpecies to match
            for (int i = 0; i < creatureStorageArray.Length; i++)
            {
                if (creatureStorageArray[i] != null)
                {
                    allowedType = creatureStorageArray[i].type;
                    break;
                }
            }
        }

        //if has an "AcceptedSpecies" text plate, update it
        //UpdateAcceptedSpeciesText();
    }
}
