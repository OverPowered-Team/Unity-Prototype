using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class playerController : MonoBehaviour
{
    public float speed = 0.05f;
    public float dashSpeed = 1.0f;
    public float maxDashTime = 1.0f;
    public float dashStopSpeed = 0.1f;

   

    Rigidbody rb;

    public float fall_mult =2.5f;
    public float jump_mult = 2f;
    private float currentDashTime;
    Gamepad gamepad = null;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Debug.Log(rb);
    }
    void Start()
    {
        currentDashTime = maxDashTime;
    }
    public void SetGamepad(Gamepad gp)
    {
        gamepad = gp;

    }
    void Update()
    {


       
        if (gamepad == null)
            return;

        //if(rb.velocity.y < 0)
        //{
        //    rb.velocity += Vector3.up * Physics.gravity.y * (fall_mult - 1);
        //}
        //else if(rb.velocity.y >0 && gamepad.buttonSouth.wasPressedThisFrame)
        //{
        //    rb.velocity += Vector3.up * Physics.gravity.y * (jump_mult - 1);
        //}

       

        
        Vector2 move = gamepad.leftStick.ReadValue();
        transform.position = new Vector3(transform.position.x + move.x * speed, transform.position.y, transform.position.z + move.y * speed);

        if (gamepad.buttonEast.wasPressedThisFrame)
        {
            currentDashTime = 0.0f;
            Debug.Log("pressed");
        }

   
        Vector3 move_dir;

        if (currentDashTime < maxDashTime)
        {

            if (move.x == 0 && move.y == 0)
                move_dir = transform.forward;

            else move_dir = new Vector3(move.x * dashSpeed, 0, move.y * dashSpeed);
            currentDashTime += dashStopSpeed;
            Debug.Log("Dashing");

        }
        else
        {
            move_dir = Vector3.zero;
        }


        transform.position += move_dir;
    }

}

    //void OrientValue(Vector2 direction)
    //{
    //    Vector3 localdir = new Vector3(transform.position.x + direction.x, transform.position.y, transform.position.z + direction.y);
    //   // Vector3 or_vec = cam.GetComponent<Transform>().rotation.eulerAngles * localdir;
    //   // transform.position = new Vector3(transform.position.x + or_vec.x * speed, transform.position.y, transform.position.z + or_vec.y * speed);
    //}


//    private void Move()
//    {

//        transform.position = new Vector3(transform.position.x + dir.x * speed, transform.position.y, transform.position.z + dir.y * speed);
//    }

//    private void OnMove(InputValue value)
//    {
//        dir = value.Get<Vector2>();


//    }
//    private void OnJump()
//    {
//        transform.Translate(transform.up);
//        Debug.Log("que apsa");
//    }
//}
