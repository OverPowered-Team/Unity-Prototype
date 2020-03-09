using System;
using System.IO;
using System.Text;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Unity json https://www.youtube.com/watch?v=4oRVMCRCvN0

[Serializable]
public class ComboManager : MonoBehaviour
{
    AttackList attackList;
    string jsonName = "attacks";

    private void Start()
    {
        //ReadAttacks(Application.dataPath + "/" + jsonName + ".json");
        WriteTestAttacks();
        //ReadAttacks(Application.dataPath + "/" + jsonName + ".json");
    }

    private void WriteTestAttack()
    {
        Attack attack = new Attack();
        attack.name = "best attack";
        attack.animation_id = "x";
        attack.damage = 1;
        attack.startDamageTime = 0;
        attack.endDamageTime = 1;

        string json = JsonUtility.ToJson(attack);
        File.WriteAllText(Application.dataPath + "/" + jsonName + ".json", json);
    }

    private void WriteTestAttacks()
    {
        attackList = new AttackList();
        attackList.attacks = new List<Attack>();

        Attack attack1 = new Attack();
        attack1.name = "attack 1";
        attack1.damage = 1;
        attack1.animation_id = "x";
        attack1.startDamageTime = 0f;
        attack1.endDamageTime = 1f;
        attackList.attacks.Add(attack1);

        Attack attack2 = new Attack();
        attack2.name = "attack 2";
        attack2.damage = 2;
        attack2.animation_id = "xx";
        attack2.startDamageTime = 0f;
        attack2.endDamageTime = 1f;
        attackList.attacks.Add(attack2);

        string json = JsonUtility.ToJson(attackList, true);
        Debug.Log(json);
        File.WriteAllText(Application.dataPath + "/" + jsonName + ".json", json);
    }

    private static void ReadAttack(string path)
    {
        if (File.Exists(path))
        {
            // Open the stream and read it back.
            using (FileStream fs = File.OpenRead(path))
            {
                byte[] b = new byte[1024];
                UTF8Encoding temp = new UTF8Encoding(true);

                while (fs.Read(b, 0, b.Length) > 0)
                {
                    Attack attack = JsonUtility.FromJson<Attack>(temp.GetString(b));
                    Debug.Log(attack.damage);
                }
            }
        }
        else
        {
            Debug.Log("File does not exist");
        }
    }

    private void ReadAttacks(string path)
    {
        if (File.Exists(path))
        {
            // Open the stream and read it back.
            using (FileStream fs = File.OpenRead(path))
            {
                byte[] b = new byte[1024];
                UTF8Encoding temp = new UTF8Encoding(true);

                while (fs.Read(b, 0, b.Length) > 0)
                {
                    string json = temp.GetString(b);
                    attackList = JsonUtility.FromJson<AttackList>(json);
                    Debug.Log(attackList.attacks[1].name);
                }
            }
        }
        else
        {
            Debug.Log("File does not exist");
        }
    }
}

[Serializable]
public class AttackList
{
    [SerializeField]
    public List<Attack> attacks;
}

//An attack represents the minimum piece of a combo
//For example, pressing X once is an Attack of the XXXXX combo
[Serializable]
public class Attack
{
    public string name;
    public string animation_id;
    public int damage;
    public float startDamageTime;
    public float endDamageTime;
    //collider?
    //collider movement / scale ?
}
