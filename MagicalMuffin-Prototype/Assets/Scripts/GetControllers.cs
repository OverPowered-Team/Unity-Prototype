using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GetControllers : MonoBehaviour
{
    // Start is called before the first frame update
    Gamepad[] g_array;
    public GameObject player1;
    public GameObject player2;

    void Start()
    {
        g_array = Gamepad.all.ToArray();
        Debug.Log(g_array.Length);
        player1.GetComponent<playerController>().SetGamepad(g_array[0]);
        player2.GetComponent<playerController>().SetGamepad(g_array[1]);
    }

}
