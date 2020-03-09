using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class playerController : MonoBehaviour
{
    Vector2 dir;
    public float speed = 0.05f;
    public int gamepad_n;
    private int id;
    Gamepad gamepad = null;
    void Start()
    {
    }

   public void SetGamepad(Gamepad gp)
    {

        gamepad = gp;
    }
    // Update is called once per frame
    void Update()
    {
        
      
        
        if (gamepad == null)
            return;

        id =  Gamepad.current.deviceId;
        Debug.Log(id);
        Vector2 move = gamepad.leftStick.ReadValue();
        transform.position = new Vector3(transform.position.x + move.x * speed, transform.position.y, transform.position.z + move.y * speed);
        //Debug.Log(move);
     //   InputSystem.GetDevice("1").;
        //Move();

        //InputSystem.GetDeviceById()
    }

    private void Move()
    {

        transform.position = new Vector3(transform.position.x + dir.x * speed, transform.position.y, transform.position.z + dir.y * speed);
    }

    private void OnMove(InputValue value)
    {
        dir = value.Get<Vector2>();


    }
    private void OnJump()
    {
        transform.Translate(transform.up);
    }
}
