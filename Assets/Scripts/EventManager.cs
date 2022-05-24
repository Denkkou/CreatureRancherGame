using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public EventHandler OnRefreshRequested;
    public EventHandler OnUIOpened;
    public EventHandler OnUIClosed;

    public EventHandler<OnCreatureHoveredEventArgs> OnCreatureHovered;
    public class OnCreatureHoveredEventArgs : EventArgs
    {
        public Creature creature;
    }

    public EventHandler OnCreatureHoverExited;

    public EventHandler<OnSwapRequestedEventArgs> OnSwapRequested;
    public class OnSwapRequestedEventArgs : EventArgs
    {
        public Creature creatureA;
        public Creature creatureB;
    }

    private int _lastUsedUID = 0;

    public int LastUsedUID
    {
        get => _lastUsedUID;
        set => _lastUsedUID = value;
    }

    public void UIClosed()
    {
        OnUIClosed?.Invoke(this, EventArgs.Empty);
    }
}
