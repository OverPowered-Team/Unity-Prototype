using System;
using System.IO;
using System.Text;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Unity json https://www.youtube.com/watch?v=4oRVMCRCvN0

public class ComboManager : MonoBehaviour
{
    [SerializeField] private Attack[] attacks;
    public string jsonName;

    private void Start()
    {
        //ReadAttacks(Application.dataPath + "/" + jsonName + ".json");
        //WriteTestAttacks();
        ReadAttacks(Application.dataPath + "/" + jsonName + ".json");
    }

    private void WriteTestAttack()
    {
        Attack attack = new Attack();
        attack.name = "best attack";
        attack.animation_id = 1;
        attack.damage = 1;
        attack.startDamageTime = 0;
        attack.endDamageTime = 1;

        string json = JsonUtility.ToJson(attack);
        File.WriteAllText(Application.dataPath + "/" + jsonName + ".json", json);
    }

    private void WriteTestAttacks()
    {
        attacks = new Attack[2];

        int i = 0;

        attacks[i] = new Attack();
        attacks[i].name = "attack 1";
        attacks[i].damage = 1;
        attacks[i].animation_id = 1;
        attacks[i].startDamageTime = 0f;
        attacks[i].endDamageTime = 1f;
        i++;

        attacks[i] = new Attack();
        attacks[i].name = "attack 2";
        attacks[i].damage = 2;
        attacks[i].animation_id = 2;
        attacks[i].startDamageTime = 0f;
        attacks[i].endDamageTime = 1f;
        i++;

        string json = JsonUtility.ToJson(this, true);
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
                    attacks = new Attack[2];
                    string json = temp.GetString(b);
                    attacks = JsonUtility.FromJson<Attack[]>(json);
                    Debug.Log(attacks[0].damage);
                }
            }
        }
        else
        {
            Debug.Log("File does not exist");
        }
    }
}

//An attack represents the minimum piece of a combo
//For example, pressing X once is an Attack of the XXXXX combo
[Serializable]
public class Attack
{
    public string name;
    public int animation_id;
    public int damage;
    public float startDamageTime;
    public float endDamageTime;
    //collider?
    //collider movement / scale ?
}
