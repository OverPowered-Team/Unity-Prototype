using System;
using System.IO;
using System.Text;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public class Stat
{
    [SerializeField]
    string name;
    [SerializeField]
    float base_value;
    [SerializeField]
    float current_value;

    public void CalculateStat(List<Effect> effects)
    {
        float additive_value = 0;
        float mult_value = 0;

        foreach (var effect in effects)
        {
            additive_value += effect.GetAdditiveAmount(name);
            mult_value += effect.GetMultiplicativeAmount(name);
        }

        current_value = base_value;
        current_value += additive_value;
        current_value += current_value * mult_value;
    }

    public float GetValue()
    {
        return current_value;
    }

    public void SetBaseStat(float value)
    {
        base_value = value;
        current_value = base_value;
    }
}
