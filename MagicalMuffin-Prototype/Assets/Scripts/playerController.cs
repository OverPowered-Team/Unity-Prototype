using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public enum PlayerState
{
    IDLE,
    MOVE,
    ATTACK,
    ATTACK_RETURN,//Transition between attack and MOVE or IDLE where you can still continue the combo
    DASH
}

public class playerController : MonoBehaviour
{
    public float speed = 7.5f;
    public float dashSpeed = 140f;
    public float maxDashTime = 1.0f;
    public float dashStopSpeed = 0.2f;

    public Stat strength;

    public float fall_mult = 2.5f;
    public float jump_mult = 2f;
    private float currentDashTime;
    [HideInInspector] public Gamepad gamepad = null;

    private float gravity = -9.8f;
    private float jump_dist = -10;
    private float count;
    private Transform cam_tansform;
    private Animator _animator;
    private PlayerInput _playerInput;
    private GeraltAttacks _playerCombos;

    private Vector3 dashDir;
    //INFO: You cannot change the direction in the middle of the dash
    //It's quite fast so you won't almost notice

    public List<Effect> effects;
    public List<Relic> relics;

    public int playerIdx = 0;
    [HideInInspector] public PlayerState currState = PlayerState.IDLE;

    void Awake()
    {
        cam_tansform = Camera.main.transform;
        _playerCombos = GetComponent<GeraltAttacks>();

        effects = new List<Effect>();
        relics = new List<Relic>();
        strength.SetBaseStat(10);
    }

    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _animator = GetComponent<Animator>();
        count = jump_dist;
        currentDashTime = maxDashTime;

        GetController();
    }

    void Update()
    {
        if(gamepad.buttonSouth.wasPressedThisFrame)
        {
            Debug.Log("Adding effect");
            //Testing
            AttackEffect test_effect = new AttackEffect();
            test_effect.SetAttackIdentifier("x");
            test_effect.AddFlatModifer(0.1f, "Attack_Damage");
            test_effect.AddFlatModifer(0.5f, "Attack_Range");
            test_effect.on_hit_delegate = EffectFunctions.ApplyBurnOnHit;

            AddEffect(test_effect);
        }
        

        if ((gamepad != null || GetController()) && _animator != null)
        {
            Vector2 move = gamepad.leftStick.ReadValue();

            switch (currState)
            {
                case PlayerState.IDLE:
                    if (CheckAttackInput())
                    {
                        _playerCombos.UpdateAttack();
                        currState = PlayerState.ATTACK;
                        _playerCombos.OnHit();
                    }
                    if (CheckDashInput())
                    {
                        StartDash();
                        currState = PlayerState.DASH;
                    }
                    if (move != Vector2.zero)
                    {
                        Move(move);
                        currState = PlayerState.MOVE;
                    }
                    break;
                case PlayerState.MOVE:
                    if (move == Vector2.zero)
                    {
                        _animator.Play("idle");
                        currState = PlayerState.IDLE;
                    }
                    else
                    {
                        Move(move);
                        currState = PlayerState.MOVE;
                    }
                    if (CheckAttackInput())
                    {
                        _playerCombos.UpdateAttack();
                        currState = PlayerState.ATTACK;
                    }
                    if (CheckDashInput())
                    {
                        StartDash();
                        currState = PlayerState.DASH;
                    }
                    break;
                case PlayerState.DASH:
                    if (!ContinueDash())
                    {
                        if (move == Vector2.zero)
                        {
                            currState = PlayerState.IDLE;
                        }
                        else
                        {
                            currState = PlayerState.MOVE;
                        }
                    }
                    break;
                case PlayerState.ATTACK:
                    _playerCombos.UpdateAttack();
                    if (CheckDashInput())
                    {
                        StartDash();
                        _playerCombos.CancelCombo();
                        currState = PlayerState.DASH;
                    }
                    break;
                case PlayerState.ATTACK_RETURN:
                    if (_playerCombos.lastAttackFinishTime + _playerCombos.extraInputWindow >= Time.time)
                    {
                        float remainingTransition = _playerCombos.extraInputWindow - (Time.time - _playerCombos.lastAttackFinishTime);
                        if (move == Vector2.zero)
                        {
                            _animator.CrossFade("idle", remainingTransition);
                        }
                        else
                        {
                            _animator.CrossFade("Movement", remainingTransition);
                        }
                    }
                    else
                    {
                        if (move == Vector2.zero)
                        {
                            _animator.Play("idle");
                            currState = PlayerState.IDLE;
                        }
                        else
                        {
                            _animator.Play("Movement");
                            currState = PlayerState.MOVE;
                        }
                    }
                    break;
            }
            SendMovementParameters(move);
        }
    }

    private bool CheckDashInput()
    {
        return gamepad.buttonEast.wasPressedThisFrame;
    }

    private bool CheckAttackInput()
    {
        return (gamepad.buttonWest.wasPressedThisFrame
            || gamepad.buttonNorth.wasPressedThisFrame);
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

    private void StartDash()
    {
        dashDir = transform.forward;
        currentDashTime = 0.0f;
        _animator.Play("_b");
        //_animator.SetBool("roll", true);//TODO: Remove. Not needed since we force it to play
    }

    private void Move(Vector2 move)
    {
        //Get angle between cam and player
        if (move != Vector2.zero || move == null)
        {
            SendMovementParameters(move);

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

    //Returns true while in the dash, returns false when it ends
    private bool ContinueDash()
    {
        if (currentDashTime < maxDashTime)
        {
            Vector3 currDash = dashDir * dashSpeed * Time.deltaTime;
            transform.position += currDash;
            currentDashTime += dashStopSpeed;
            return true;
        }
        else
        {
            _animator.SetBool("roll", false);
            return false;
        }
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

    private void SendMovementParameters(Vector2 value)
    {
        _animator.SetFloat("VelX", value.x);
        _animator.SetFloat("VelY", value.y);
        Debug.Log(value);
    }

    void AddEffect(Effect new_effect)
    {
        effects.Add(new_effect);

        //RecalculateStats(); 

        if (new_effect is AttackEffect)
        {
            _playerCombos.OnAddAttackEffect(((AttackEffect)new_effect).GetAttackIdentifier());
        }

    }
}