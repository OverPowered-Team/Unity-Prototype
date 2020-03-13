using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YenneferStats : MonoBehaviour
{
    public Slider HpBar;
    public Image FillImage;
    static public int hp_value = 100;
    private playerController playerController;
    public GameObject deathParticle;

    void Start()
    {
        playerController = GetComponent<playerController>();
    }

    void Update()
    {
        if (hp_value < 0)
        {
            hp_value = 0;
            playerController.enabled = false;//TAKING CONTROL FROM THE PLAYER = DYING
            deathParticle.SetActive(true);
        }
        if (HpBar.value <= HpBar.maxValue / 2)
        {
            FillImage.color = Color.yellow;
        }
        if (HpBar.value <= HpBar.maxValue / 4)
        {
            FillImage.color = Color.red;
        }
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "MutantAttack")
        {
            GetHit(15);
        }
        if (other.transform.tag == "MutantChargedJump")
        {
            GetHit(30);
        }
        if (other.transform.tag == "Arrow")
        {
            GetHit(10);
        }
    }
}
