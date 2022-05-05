using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A variant group stores all variants of a specific species

public enum CreatureSize
{
    Small,
    Medium,
    Large,
    UNDEFINED
}

public enum CreatureType
{
    Eepy,
    UNDEFINED
}

[CreateAssetMenu(fileName = "NewVariantGroup", menuName = "Creatures/New Variant Group")]
public class VariantGroup : ScriptableObject
{
    public Variant[] variants;
    public CreatureSize size;
    public CreatureType type;
}
