using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect
{
    public delegate void OnHit();
    public OnHit on_hit_delegate;

    List<Modifier> additive_modifiers;
    List<Modifier> multiplicative_modifers;

    public Effect()
    {
        additive_modifiers = new List<Modifier>();
        multiplicative_modifers = new List<Modifier>();
    }


    public void AddFlatModifer(float value, string identifier)
    {
        Modifier new_mod;
        new_mod.amount = value;
        new_mod.identifier = identifier;

        additive_modifiers.Add(new_mod);
    }
    public void AddMultiplicativeModifer(float value, string identifier)
    {
        Modifier new_mod;
        new_mod.amount = value;
        new_mod.identifier = identifier;

        multiplicative_modifers.Add(new_mod);
    }

    public float GetAdditiveAmount(string identifier)
    {
        float final_value = 0;
   
        foreach (Modifier mod in additive_modifiers)
        {
            if (mod.identifier == identifier)
            {
                final_value += mod.amount;
            }
        }

        return final_value;
    }

    public float GetMultiplicativeAmount(string identifier)
    {
        float final_value = 0;

        foreach (Modifier mod in multiplicative_modifers)
        {
            if (mod.identifier == identifier)
            {
                final_value += mod.amount;
            }
        }

        return final_value;
    }
}

public class AttackEffect: Effect
{
    string attack_name;
    public void SetAttackIdentifier(string identifier)
    {
        attack_name = identifier;
    }
    public string GetAttackIdentifier()
    {
        return attack_name;
    }
}

struct Modifier
{
    public float amount;
    public string identifier;
}

