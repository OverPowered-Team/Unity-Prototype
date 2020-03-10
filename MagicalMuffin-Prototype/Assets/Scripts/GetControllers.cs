using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GetControllers : MonoBehaviour
{
    // Start is called before the first frame update
    Gamepad[] g_array;
    playerController[] playercontroller;
    //public GameObject[] players;
    public GameObject player1;
    public GameObject player2;

    bool dirt = false;
    void Start()
    {
        g_array = Gamepad.all.ToArray();
        Debug.Log("Ara");
        Debug.Log(g_array.Length);
       player1.GetComponent<playerController>().SetGamepad(g_array[0]);
      player2.GetComponent<playerController>().SetGamepad(g_array[1]);

        //playercontroller[0].SetGamepad(g_array[0]);
        //playercontroller[1].SetGamepad(g_array[]);


    }


    // Update is called once per frame
    void Update()
    {
        
        //if(dirt == false)
        //{
        //    for (int i = 0; i < players.Length; i++)
        //    {
        //        playercontroller[i] = players[i].GetComponent<playerController>();
        //        Debug.Log("GO to PC");
        //    }
        //    g_array = Gamepad.all.ToArray();
        //    Debug.Log(g_array.Length);
        //    for (int i = 0; i < playercontroller.Length; ++i)
        //    {
        //        Debug.Log("PC to GP");
        //        playercontroller[i].SetGamepad(g_array[i]);
        //        Debug.Log(i);
        //    }
        //    dirt = true;
        //}
    }

}
