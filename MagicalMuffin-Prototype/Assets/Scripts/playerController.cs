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
    Collider coll;
    public float fall_mult =2.5f;
    public float jump_mult = 2f;
    private float currentDashTime;
    Gamepad gamepad = null;
    private float jump_dist =-10;
    private float count;
    void Awake()
    {
       // rb = GetComponent<Rigidbody>();
        Debug.Log(rb);
        coll = GetComponentInChildren<Collider>();
        
    }
    void Start()
    {
        count = jump_dist;
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

      //  if(!coll.bounds.Intersects.)
        //if(rb.velocity.y < 0)
        //{
        //    rb.velocity += Vector3.up * Physics.gravity.y * (fall_mult - 1);
        //}
        //else if(rb.velocity.y >0 && gamepad.buttonSouth.wasPressedThisFrame)
        //{
        //    rb.velocity += Vector3.up * Physics.gravity.y * (jump_mult - 1);
        //}

       
      //if(coll.attachedRigidbody.velocity.y <0 && !coll.bounds.Intersects(coll.bounds))
      //  {
      //      Debug.Log("velocity under 0");
      //      transform.position += Vector3.up * Physics.gravity.y * (fall_mult - 1);
      //  }
      //  else if (coll.attachedRigidbody.velocity.y == 0)
      //  {
      //      Debug.Log("velocity equal 0");
      //  }
      //  if (coll.attachedRigidbody.velocity.y > 0)
      //  {
      //      transform.position += Vector3.up * Physics.gravity.y * (fall_mult - 1);
      //  }

        if(gamepad.buttonSouth.wasPressedThisFrame && jump_dist == count)
        {
            jump_dist = -count;
        }
        if (jump_dist <= -count && jump_dist > 0)
        {
            transform.position += Vector3.up;
            jump_dist -= 1;
            Debug.Log(jump_dist);
        }
        else if (jump_dist <= 0 && jump_dist > count)
        {
            transform.position -= Vector3.up;
            jump_dist -= 1;
            Debug.Log(jump_dist);
        }
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


