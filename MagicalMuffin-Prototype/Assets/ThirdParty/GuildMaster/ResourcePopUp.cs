using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcePopUp : MonoBehaviour
{

    public int type;
    string text_info = "";

    private void Start()
    {
        switch (type)
        {
            case 1:
                text_info = "Gold is used to upgrade Buildings and level up Members. Obtainable from Quests.";
                break;
            case 2:
                text_info = "Crowns are used to upgrade the Guild Hall. Obtainable from Quests.";
                break;
            case 3:
                text_info = "Shields are used to upgrade the Blacksmith. Obtainable from Quests.";
                break;
            case 4:
                text_info = "Potions allow a member to heal 50% stamina when it's stamina drops below 25%. Obtainable from Mage work.";
                break;
            case 5:
                text_info = "Meat greatly increases the success chance of Adventure Quests. Obtainable from Hunter work.";
                break;
            case 6:
                text_info = "Flames greatly increase the success chance of Bounty Quests. Obtainable from Warrior work.";
                break;
            default:
                break;
        }

        transform.GetChild(1).GetChild(0).GetComponent<Text>().text = text_info;
    }

    public void ShowPopUp()
    {
        transform.GetChild(1).gameObject.SetActive(true);
    }

    public void HidePopUp()
    {
        transform.GetChild(1).gameObject.SetActive(false);
    }
}
