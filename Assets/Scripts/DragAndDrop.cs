using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/* This drag and drop script was adapted from https://stackoverflow.com/questions/44515498/drag-and-drop-in-scrollrect-scrollview-in-unity3d
 * 
 * It's currently cosmetic; the object we are dragging around is the icon from each slot. When it comes to calling the swap function,
 * I might have to refer to the icon's _ogParent reference to get the slot it came from (important to note that it's parented to something else
 * until released. I don't know what order the draggedIn code and the reparent code will execute, so lets just assume the wrong order. To fix this
 * I've added a script to the temporary sibling/parent to store a reference to the previous parent, aka, this slot.
 * 
 * The way this class works, I think, is to restrict the scrollRect's functionality to whenever we aren't dragging. 
 * 
 * Cosmetically, both the origin slot and the icon go semi-transparent when dragging. Additionally, both the original slot
 * and the currently hovered over slot (while still dragging) both wobble. This should give some visual feedback for
 * what slots are potentially being swapped.
 * 
 * There is currently a coupled hack fix for the lingering wobble when you release at line 117 so try and find a way around this
 * (might be an issue with MouseHover.cs?)
 */

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    private bool draggingSlot;
    private ScrollRect scrollRect;

    //object references
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;

    private Vector2 _transformBeforeDrag;
    private RawImage _image;

    //sibling and parent info
    private int _siblingIndex;
    private GameObject _slotHelper;

    private EventManager _eventManager;

    public GameObject _ogParent;

    //this flag is specifically for determining when something has been
    //clicked and dragged the intended way, as to avoid bypassing it
    private bool _isPickedUp;

    private void Start()
    {
        //find the event manager on EventSystem
        _eventManager = GameObject.Find("EventSystem").GetComponent<EventManager>();

        //get the canvas components
        _canvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();

        //set the icon that will be dragged
        _image = transform.GetChild(0).GetComponent<RawImage>();

        //store index of this object, as to be able to return to it
        _siblingIndex = transform.GetSiblingIndex();

        //get the last sibling (the helper object that will store the icon to render on top)
        _slotHelper = transform.parent.gameObject.transform.GetChild(transform.parent.gameObject.transform.childCount - 1).gameObject;

        //store reference to the original parent (this) for both referencing later & reparenting
        _ogParent = transform.gameObject;

        //reference to the scrollRect
        scrollRect = GameObject.Find("Scroll").GetComponent<ScrollRect>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //set the original position of the icon when clicked
        _transformBeforeDrag = _image.rectTransform.anchoredPosition;

        StartCoroutine(StartTimer());
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        StopAllCoroutines();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_isPickedUp == false)
            eventData.pointerDrag = null;

        ExecuteEvents.Execute(scrollRect.gameObject, eventData, ExecuteEvents.endDragHandler);

        StopAllCoroutines();
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(0.3f);
        draggingSlot = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {   
        ExecuteEvents.Execute(scrollRect.gameObject, eventData, ExecuteEvents.beginDragHandler);
    }

    public void OnDrag(PointerEventData eventData)
    {

        if (draggingSlot)
        {
            //Do the dragging effects if being dragged
            DragEffects(eventData);
            
        } else
        {
            //otherwise, let the scrollRect do its normal behaviour
            ExecuteEvents.Execute(scrollRect.gameObject, eventData, ExecuteEvents.dragHandler);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ExecuteEvents.Execute(scrollRect.gameObject, eventData, ExecuteEvents.endDragHandler);

        if (draggingSlot)
        {
            //undo all of the drag effects when dragging stops
            RevertDragEffects();

            //try to fix the lingering hover wobble (hack fix, dont couple like this)
            transform.GetComponent<MouseHover>().isHovered = false;
        }
    }

    //some tidier functions for OnDrag and OnDragEnd
    private void DragEffects(PointerEventData eventData)
    {
        _isPickedUp = true;

        //follow mouse position
        _image.rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;

        //set the slot cosmetics
        _canvasGroup.alpha = 0.6f;
        _canvasGroup.blocksRaycasts = false;

        //set the icon cosmetics
        _image.maskable = false;
        _image.raycastTarget = false;

        //make icon semi-transparent
        var tempColour = _image.color;
        tempColour.a = 0.6f;
        _image.color = tempColour;

        //parent to the helper temporarily
        _image.transform.SetParent(_slotHelper.transform, true);

        //tell the helper the original parent
        _slotHelper.GetComponent<SlotHelper>().originalParent = _ogParent;
    }

    private void RevertDragEffects()
    {
        draggingSlot = false;
        _isPickedUp = false;

        //reset the cosmetics to normal
        _canvasGroup.alpha = 1.0f;
        _canvasGroup.blocksRaycasts = true;

        _image.maskable = true;
        _image.raycastTarget = true;

        //make icon non-transparent
        var tempColour = _image.color;
        tempColour.a = 1f;
        _image.color = tempColour;

        //return to initial position
        _image.rectTransform.anchoredPosition = _transformBeforeDrag;
        _image.rectTransform.localScale = Vector3.one;

        //reparent and revert organisation to normal
        _image.transform.SetParent(_ogParent.transform, false);
        _image.transform.SetSiblingIndex(_siblingIndex);

        //finally clear the helper's parent ref for hygiene
        _slotHelper.GetComponent<SlotHelper>().originalParent = null;
    }

    //this is the code for when an object is dragged onto this one
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            //get references to both candidates
            GameObject swapCandidateA = eventData.pointerDrag.gameObject;
            GameObject swapCandidateB = transform.gameObject;

            //we need to get each of the slots' storage UI parents as they have a ref to their home storage
            StorageManager homeStorageA = FindParentWithTag(swapCandidateA, "StorageUI").GetComponent<StorageUI>().connectedStorage.GetComponent<StorageManager>();
            StorageManager homeStorageB = FindParentWithTag(swapCandidateB, "StorageUI").GetComponent<StorageUI>().connectedStorage.GetComponent<StorageManager>();

            //we then want to find the creatures associated with these slots via their positions in the slots array that contains them
            int slotAPosInArray = System.Array.IndexOf(FindParentWithTag(swapCandidateA, "StorageUI").GetComponent<StorageUI>().slotsArray, swapCandidateA);
            int slotBPosInArray = System.Array.IndexOf(FindParentWithTag(swapCandidateB, "StorageUI").GetComponent<StorageUI>().slotsArray, swapCandidateB);

            Creature creatureA = null;
            Creature creatureB = null;

            //if both indices are in bounds, request the swap, otherwise degrade gracefully
            if ((slotAPosInArray >= 0 && slotAPosInArray < homeStorageA.creatureStorageArray.Length)
                && (slotBPosInArray >= 0 && slotBPosInArray < homeStorageB.creatureStorageArray.Length))
            {
                creatureA = homeStorageA.creatureStorageArray[slotAPosInArray];
                creatureB = homeStorageB.creatureStorageArray[slotBPosInArray];

                //emit event containing the two creatures if all is well
                _eventManager.OnSwapRequested?.Invoke(this, new EventManager.OnSwapRequestedEventArgs { creatureA = creatureA, creatureB = creatureB });
            }
            else
                Debug.Log("Swap aborted: Out of Bounds Error");


            //there is room for improvement here - swapping to null slots breaks, so maybe have an alternative signal for potential null entries?
            //also, re-enable the image component on the slot prefab if this works
        }
    }

    public static GameObject FindParentWithTag(GameObject childObject, string tag)
    {
        Transform t = childObject.transform;
        while (t.parent != null)
        {
            if (t.parent.tag == tag)
                return t.parent.gameObject;

            t = t.parent.transform;
        }
        return null; // Could not find a parent with given tag.
    }
}