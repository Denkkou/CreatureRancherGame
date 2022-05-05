using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewVariant", menuName = "Creatures/New Variant")]
public class Variant : ScriptableObject
{
    public string variantName;
    public string variantDescription;

    public Texture2D variantIcon;
    
    //variant's model or prefab
}