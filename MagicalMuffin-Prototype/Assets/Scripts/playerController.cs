using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerController : MonoBehaviour
{
    public float speed = 7.5f;
    public float dashSpeed = 140f;
    public float maxDashTime = 1.0f;
    public float dashStopSpeed = 0.2f;

    public float fall_mult =2.5f;
    public float jump_mult = 2f;
    private float currentDashTime;
    [HideInInspector] public Gamepad gamepad = null;

    private float gravity = -9.8f;
    private float jump_dist =-10;
    private float count;
    private Transform cam_tansform;
    private Animator _animator;
    private PlayerInput _playerinput;
    private GeraltAttacks _playerCombos;

    public int playerIdx = 0;

    void Awake()
    {
        cam_tansform = Camera.main.transform;
        _playerCombos = GetComponent<GeraltAttacks>();
    }

    void Start()
    {
        _playerinput = GetComponent<PlayerInput>();
        _animator = GetComponent<Animator>();
        count = jump_dist;
        currentDashTime = maxDashTime;

        GetController();
    }

    private bool GetController()
    {
        Gamepad[] gamepads = Gamepad.all.ToArray();
        if (gamepads.Length > playerIdx)
        {
            gamepad = gamepads[playerIdx];
            return true;
        }
        return false;
    }

    float GetPosbyTime(float time)
    {
        return -0.5f * gravity * Mathf.Sqrt(time);
    }

    void Update()
    {
        if ((gamepad != null || GetController()) && _animator != null)
        {
            Jump();
            //TODO: Also cancel the current combo
            Vector2 move = gamepad.leftStick.ReadValue();
            if (!_playerCombos.DoingAttack() || _playerCombos.FinishedAttack())
            {
                Move(move);
            }
            Dash(move);
            //TODO: Cancel the current combo
        }
    }

    private void Move(Vector2 move)
    {
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
    }

    private void Dash(Vector2 move)
    {
        if (gamepad.buttonEast.wasPressedThisFrame)
        {
            Debug.Log("enter here");
            currentDashTime = 0.0f;
            _animator.SetBool("roll", true);
            _animator.CrossFade("_b", 0.01f);
        }

        Vector3 move_dir;

        if (currentDashTime < maxDashTime)
        {
            if (move.x == 0 && move.y == 0)
            {
                move_dir = transform.forward * Time.deltaTime * dashSpeed;
            }
            else
            {
                move_dir = new Vector3(move.x * dashSpeed * Time.deltaTime, 0, move.y * dashSpeed * Time.deltaTime);
            }
            currentDashTime += dashStopSpeed;
        }
        else
        {
            _animator.SetBool("roll", false);
            move_dir = Vector3.zero;
        }

        transform.position += move_dir;
    }

    private void Jump()
    {
        if (gamepad.buttonSouth.wasPressedThisFrame && jump_dist == count)
        {
            jump_dist = -count;
        }
        if (jump_dist <= -count && jump_dist > 0f)
        {

            transform.position += Vector3.up;
            jump_dist -= 1f;
        }
        else if (jump_dist <= 0f && jump_dist > count)
        {
            transform.position -= Vector3.up;
            jump_dist -= 1f;
        }
    }

    private void BlendAnim(Vector2 value)
    {
        _animator.SetFloat("VelX",value.x);
        _animator.SetFloat("VelY", value.y);
        Debug.Log(value);
    }
}