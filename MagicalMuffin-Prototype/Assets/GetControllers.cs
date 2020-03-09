using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GetControllers : MonoBehaviour
{
   
    public GameObject player1;
    public GameObject player2;

    void Start()
    {
        Gamepad[] g_array = Gamepad.all.ToArray();
       
       player1.GetComponent<playerController>().SetGamepad(g_array[0]);
      player2.GetComponent<playerController>().SetGamepad(g_array[1]);

        


    }


  

}
