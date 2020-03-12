using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect
{
    public delegate void OnHit();

    List<Modifier> additive_modifiers;
    List<Modifier> multiplicative_modifers;

    float ApplyModifiers(string identifier, float current_val)
    {
        float final_value = current_val;

        foreach (Modifier mod in multiplicative_modifers)
        {
            if(mod.identifier == identifier)
            {
                final_value *= mod.amount;
            }
        }
        foreach (Modifier mod in additive_modifiers)
        {
            if (mod.identifier == identifier)
            {
                final_value += mod.amount;
            }
        }

        return final_value;
    }
}

public class AttackEffect:Effect
{
    string attack_name;
}

struct Modifier
{
    public float amount;
    public string identifier;
}

