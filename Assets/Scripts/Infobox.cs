using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* The infobox will receive a signal containing a currently clicked on creature
 * and use that data to overwrite its textbox with its details.
 * 
 */

public class Infobox : MonoBehaviour
{
    private EventManager _eventManager;

    public GameObject infoboxContainer;

    public GameObject infoText;
    public GameObject speciesText;
    public GameObject icon;

    public GameObject trait1;
    public GameObject trait2;
    public GameObject trait3;

    public GameObject healthInfo;
    public GameObject happyInfo;

    //component references
    private TextMeshProUGUI infoTextTMP;
    private TextMeshProUGUI speciesTextTMP;
    private RawImage iconTexture;

    private TextMeshProUGUI trait1TMP;
    private TextMeshProUGUI trait2TMP;
    private TextMeshProUGUI trait3TMP;

    private TextMeshProUGUI healthInfoText;
    private TextMeshProUGUI happyInfoText;

    void Start()
    {
        //subscribe to the creature clicked event
        _eventManager = GameObject.Find("EventSystem").GetComponent<EventManager>();
        _eventManager.OnCreatureHovered += EventManager_OnCreatureHovered;
        _eventManager.OnCreatureHoverExited += EventManager_OnCreatureHoverExited;

        //get references to all of the components on start
        infoTextTMP = infoText.GetComponent<TextMeshProUGUI>();
        speciesTextTMP = speciesText.GetComponent<TextMeshProUGUI>();
        iconTexture = icon.GetComponent<RawImage>();

        trait1TMP = trait1.GetComponentInChildren<TextMeshProUGUI>();
        trait2TMP = trait2.GetComponentInChildren<TextMeshProUGUI>();
        trait3TMP = trait3.GetComponentInChildren<TextMeshProUGUI>();

        healthInfoText = healthInfo.GetComponentInChildren<TextMeshProUGUI>();
        happyInfoText = happyInfo.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void EventManager_OnCreatureHovered(object sender, EventManager.OnCreatureHoveredEventArgs e)
    {
        //make the info box visible
        infoboxContainer.SetActive(true);

        UpdateInfoBox(e.creature);
    }

    private void EventManager_OnCreatureHoverExited(object sender, EventArgs e)
    {
        //hide the infobox
        infoboxContainer.SetActive(false);

        //potentially add a delay to this, so that the flickering effect
        //when quickly moving between creatures is eliminated
    }

    //update all features of the infobox
    private void UpdateInfoBox(Creature creature)
    {
        if (creature != null)
        {
            //make the gender human legible
            String gender = "";

            if (creature.gender == 0)
                gender = "Female";
            else
                gender = "Male";

            //first format the string for infoTextTMP
            string creatureInfo = "Name: " + creature.nickname +
                "\n" + "Gender: " + gender +
                "\n" + "Age: " + creature.age +
                "\n" + "Variant: " + creature.variant.name;

            //then update it
            infoTextTMP.text = creatureInfo;

            //update the species text
            speciesTextTMP.text = creature.type.ToString();

            //update the icon from the variant info
            iconTexture.texture = creature.variant.variantIcon;

            //if any traits are empty, replace with a stand-in string           
            if (creature.traits[0] != null)
                trait1TMP.text = creature.traits[0].ToString();
            else
                trait1TMP.text = "";

            if (creature.traits[1] != null)
                trait2TMP.text = creature.traits[1].ToString();
            else
                trait2TMP.text = "";

            if (creature.traits[2] != null)
                trait3TMP.text = creature.traits[2].ToString();
            else
                trait3TMP.text = "";

            //update the happiness and health
            healthInfoText.text = creature.healthPercent.ToString();
            happyInfoText.text = creature.happinessPercent.ToString();
        }
    }
}
