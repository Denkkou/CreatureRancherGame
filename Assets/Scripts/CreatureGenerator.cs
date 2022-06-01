using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script is responsible for generating new creatures, be it randomly or through breeding.
 * 
 * For now, a debug function is used by a UI button to create and send off a generic creature. As such,
 * the random creature function is not feature complete and just generates the same thing every time.
 * 
 * To remedy this, a list of variant groups needs populating, and for a random variant from a random group
 * to be picked. Now that variant groups contain species data, there is no need to edit or reorganise any
 * lists or conditionals.
 */

public class CreatureGenerator : MonoBehaviour
{
    public VariantGroup EepyVariantGroup;

    public Creature RandomCreature()
    {
        //right now just do a random Eepy
        Creature creature = new Creature();

        string[] tempTraits = { "Sleepy", null, null };
        int randIndex = Random.Range(0, EepyVariantGroup.variants.Length);
        int randGender = Random.Range(0, 2);
        creature = CreatureFromParams("Andra", 0f, randGender, EepyVariantGroup.type, EepyVariantGroup.size, EepyVariantGroup.variants[randIndex], tempTraits);

        return creature;
    }

    public Creature CreatureFromParams(string _nickname, float _age, int _gender, CreatureType _type, CreatureSize _size, Variant _variant, string[] _traits)
    {
        Creature creature = new Creature();
        creature.Init(_nickname, _age, _gender, _type, _size, _variant, _traits);

        return creature;
    }

    public Creature BreedNewCreature(Creature parentA, Creature parentB)
    {
        Creature offspring = new Creature();

        return offspring;
    }

    public void DebugRandomCreature()
    {
        //for now, just send it over
        transform.GetComponent<StorageManager>().AddCreature(RandomCreature());
    }

}