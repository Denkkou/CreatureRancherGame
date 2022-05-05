using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature
{
    //a unique ID for each creature
    public int creatureID;

    public string nickname;
    public float age;
    public int gender;

    public CreatureType type;
    public CreatureSize size;

    public Variant variant;

    public string[] traits = { null, null, null };

    public int healthPercent;
    public int happinessPercent;

    public string infoText;

    //full initialise, size, type & variant are passed by VariantGroup's data (eg. all 'Eepy' are type: 'Eepy' and size: 'Small')
    public void Init(string _nickname, float _age, int _gender, CreatureType _type, CreatureSize _size, Variant _variant, string[] _traits)
    {
        //set the unique ID
        SetUID();

        //attributes
        nickname = _nickname;
        age = _age;
        gender = _gender;
        type = _type;
        size = _size;
        variant = _variant;
        traits = _traits;

        healthPercent = 0;
        happinessPercent = 0;

        //set info text for easy debug / logging
        infoText = GenerateInfoText();
    }

    private string GenerateInfoText()
    {
        string infoString = "Species: " + type +
            "\n" + "Nickname: " + nickname +
            "\n" + "Age: " + age +
            "\n" + "Gender: " + gender +
            "\n" + "Variant: " + variant.variantName +
            "\n" + "Size: " + size +
            "\n" + "Traits: " + traits[0] + " " + traits[1] + " " + traits[2] +
            "\n" + "Health: " + healthPercent + "/100" +
            "\n" + "Happiness: " + happinessPercent + "/100" +
            "\n" + "UID: " + creatureID;

        return infoString;
    }
    
    private void SetUID()
    {
        //take the next UID value from the eventManager
        EventManager evMan = GameObject.Find("EventSystem").GetComponent<EventManager>();
        creatureID = evMan.LastUsedUID += 1;

        //and update the last used for the next creature
        evMan.LastUsedUID = creatureID;
    }
}
