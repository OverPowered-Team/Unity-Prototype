using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class playerController : MonoBehaviour
{
    public float speed = 1f;
    public float dashSpeed = 0.5f;
    public float maxDashTime = 1.0f;
    public float dashStopSpeed = 0.2f;
   


    public float fall_mult =2.5f;
    public float jump_mult = 2f;
    private float currentDashTime;
    Gamepad gamepad = null;

    private float gravity = -9.8f;
    private float jump_dist =-10;
    private float count;
    Transform cam_tansform;
    private Animator _animator;
    float GetPosbyTime(float time) {

        return -0.5f * gravity * Mathf.Sqrt(time);
    }

    void Awake()
    {
        cam_tansform = Camera.main.transform;
    }
    void Start()
    {
        _animator = GetComponent<Animator>();
        count = jump_dist;
        currentDashTime = maxDashTime;
    }
    public void SetGamepad(Gamepad gp)
    {
        gamepad = gp;

    }
    void Update()
    {


       
        if (gamepad == null || _animator == null)
            return;


        if(gamepad.buttonSouth.wasPressedThisFrame && jump_dist == count)
        {
            jump_dist = -count;
        }
        if (jump_dist <= -count && jump_dist > 0)
        {
          
            transform.position += Vector3.up;
            jump_dist -= 1;
        }
        else if (jump_dist <= 0 && jump_dist > count)
        {
            transform.position -= Vector3.up;
            jump_dist -= 1;
        }

        //Read
        Vector2 move = gamepad.leftStick.ReadValue();
        //Get angle between cam and player
        if (move != Vector2.zero || move == null)
        {

            BlendAnim(move);

            Vector2 cam_pos = new Vector2(cam_tansform.transform.position.x, cam_tansform.transform.position.z);
            float temp = Vector2.Dot(cam_pos, move);
            float cam_pos_mag = cam_pos.SqrMagnitude();
            float dir_mag = move.SqrMagnitude();
            float angle = Mathf.Acos(temp / (cam_pos_mag * dir_mag));


            //Rotate the direction move of the joystick vector
            move = new Vector2(move.x * Mathf.Cos(angle) - move.y * Mathf.Sin(angle), move.x * Mathf.Sin(angle) + move.y * Mathf.Cos(angle));


            Vector3 dst = new Vector3(transform.position.x + move.x * speed * Time.deltaTime, transform.position.y, transform.position.z + move.y * speed * Time.deltaTime);
            transform.LookAt(dst, Vector3.up);
            transform.position = dst;

        }
        if (gamepad.buttonEast.wasPressedThisFrame)
        {
            currentDashTime = 0.0f;
        }

   
        Vector3 move_dir;

        if (currentDashTime < maxDashTime)
        {

            if (move.x == 0 && move.y == 0)
                move_dir = transform.forward;

            else move_dir = new Vector3(move.x * dashSpeed, 0, move.y * dashSpeed);
            currentDashTime += dashStopSpeed;

        }
        else
        {
            move_dir = Vector3.zero;
        }


        transform.position += move_dir;
    }


    private void BlendAnim(Vector2 value)
    {
        _animator.SetFloat("VelX",value.x);
        _animator.SetFloat("VelY", value.y);
        Debug.Log(value);

    }
}


