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
    private List<Attack> attacks;
    public string jsonName;

    private void Start()
    {
        attacks = new List<Attack>();

        WriteTestAttack();

        //ReadAttacks(Application.dataPath + "/" + jsonName + ".json");
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

    private static void ReadAttacks(string path)
    {
        if (!File.Exists(path))
        {
            // Create the file.
            using (FileStream fs = File.Create(path))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file.");

                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }
        }
        else
        {
            Debug.Log("File does not exist");
        }

        // Open the stream and read it back.
        using (FileStream fs = File.OpenRead(path))
        {
            byte[] b = new byte[1024];
            UTF8Encoding temp = new UTF8Encoding(true);

            while (fs.Read(b, 0, b.Length) > 0)
            {
                Attack attack = JsonUtility.FromJson<Attack>(temp.GetString(b));
                Debug.Log(attack);
            }
        }
    }


    //An attack represents the minimum piece of a combo
    //For example, pressing X once is an Attack of the XXXXX combo
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


}
