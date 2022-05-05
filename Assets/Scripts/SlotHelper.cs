using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The Slot Helper object is to help render the dragged icon on top of the others
// It is a little inelegant but it can communicate the data we need while preserving
// the functionality of the inventory's grid layout.

public class SlotHelper : MonoBehaviour
{
    //reference to the original parent of the icon temporarily childed
    public GameObject originalParent;
}
