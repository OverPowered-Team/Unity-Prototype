using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeraltStats : MonoBehaviour
{
    public Slider HpBar;
    public Image FillImage;
    static public int hp_value = 100;

    void Start()
    {
        
    }

    void Update()
    {
        if (hp_value < 0)
            hp_value = 0;

        if(HpBar.value <= HpBar.maxValue/2)
            FillImage.color = Color.yellow;
        if (HpBar.value <= HpBar.maxValue/4)
            FillImage.color = Color.red;

        HpBar.value = hp_value;
    }

    public void GetHit(int value)
    {
        StartCoroutine("RestLife", value);
    }

    IEnumerator RestLife(int value)
    {
        for (int i = 0; i < value; i++)
        {
            yield return new WaitForSeconds(0.02f);
            hp_value --;

        }
    }
}
