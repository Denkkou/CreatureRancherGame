using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A variant group stores all variants of a specific species

public enum CreatureType
{
    Eepy,
    UNDEFINED, //used as a flag for mixed storages
    EMPTY //used only for empty creature objects
}

public enum CreatureSize
{
    Small,
    Medium,
    Large,
    UNDEFINED,
    EMPTY
}

[CreateAssetMenu(fileName = "NewVariantGroup", menuName = "Creatures/New Variant Group")]
public class VariantGroup : ScriptableObject
{
    public Variant[] variants;
    public CreatureSize size;
    public CreatureType type;
}
