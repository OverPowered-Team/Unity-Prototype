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
    public Stat strength;

    //Input
    [HideInInspector] public Gamepad gamepad = null;
    public int playerIdx = 0;

    //FSM
    [HideInInspector] public PlayerState currState = PlayerState.IDLE;

    //Movement
    public float speed;

    //Dash
    public float dashSpeed;
    public float maxDashTime;
    private float currentDashTime = 0f;
    private Vector3 dashDir;

    //Components
    private Transform cam_tansform;
    private Animator _animator;
    private PlayerInput _playerInput;
    [HideInInspector] public GeraltAttacks _playerCombos;

    public List<Effect> effects;
    public List<Relic> relics;

    public GameObject hpUI;
    public GameObject combosUI;

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
        currentDashTime = maxDashTime;

        GetController();
    }

    void Update()
    { 
        if ((gamepad != null || GetController()) && _animator != null)
        {
            Vector2 move = gamepad.leftStick.ReadValue();

            switch (currState)
            {
                case PlayerState.IDLE:
                    if (move != Vector2.zero)
                    {
                        Move(move);
                        _animator.Play("Movement");
                        currState = PlayerState.MOVE;
                    }
                    if (CheckAttackInput())
                    {
                        _playerCombos.UpdateAttack();
                        currState = PlayerState.ATTACK;
                    }
                    if (CheckDashInput())
                    {
                        StartDash(move);
                        currState = PlayerState.DASH;
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
                    }
                    if (CheckAttackInput())
                    {
                        _playerCombos.UpdateAttack();
                        currState = PlayerState.ATTACK;
                    }
                    if (CheckDashInput())
                    {
                        StartDash(move);
                        currState = PlayerState.DASH;
                    }
                    break;
                case PlayerState.DASH:
                    if (!ContinueDash())
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
                case PlayerState.ATTACK:
                    RotateCharacter(move);
                    _playerCombos.UpdateAttack();
                    if (CheckDashInput())
                    {
                        StartDash(move);
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
                            Move(move);
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
                            Move(move);
                            currState = PlayerState.MOVE;
                        }
                    }
                    if (CheckAttackInput())
                    {
                        _playerCombos.UpdateAttack();
                        currState = PlayerState.ATTACK;
                    }
                    if (CheckDashInput())
                    {
                        StartDash(move);
                        currState = PlayerState.DASH;
                    }
                    break;
            }
            //SendMovementParameters(move);

            UIControls();
        }
    }

    private void UIControls()
    {
        if (gamepad.dpad.right.wasPressedThisFrame)
        {
            hpUI.SetActive(false);
            combosUI.SetActive(true);
        }
        if (gamepad.dpad.left.wasPressedThisFrame)
        {
            hpUI.SetActive(true);
            combosUI.SetActive(false);
        }
    }

    private bool CheckDashInput()
    {
        return gamepad.rightTrigger.wasPressedThisFrame;
    }

    private bool CheckAttackInput()
    {
        return (gamepad.buttonWest.wasPressedThisFrame
            || gamepad.buttonNorth.wasPressedThisFrame);
    }

    private void RotateCharacter(Vector2 move)
    {
        //Get angle between cam and player
        if (move != Vector2.zero && move != null)
        {
            Vector3 dst = transform.position + GetInputRelativeToCamera(move);
            if (!float.IsNaN(dst.x) && !float.IsNaN(dst.z))
            {
                transform.LookAt(dst, Vector3.up);
            }
        }
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

    private void StartDash(Vector2 move)
    {
        if (move == Vector2.zero)
        {
            dashDir = transform.forward;
        }
        else
        {
            dashDir = GetInputRelativeToCamera(move.normalized);
        }
        currentDashTime = 0.0f;
        _animator.Play("dash");
    }

    private Vector3 GetInputRelativeToCamera(Vector2 joystickInput)
    {
        Vector2 cam_pos = new Vector2(cam_tansform.transform.position.x, cam_tansform.transform.position.z);
        float temp = Vector2.Dot(cam_pos, joystickInput);
        float cam_pos_mag = cam_pos.SqrMagnitude();
        float dir_mag = joystickInput.SqrMagnitude();
        float angle = Mathf.Acos(temp / (cam_pos_mag * dir_mag));

        //Rotate the direction move of the joystick vector
        return new Vector3(
            joystickInput.x * Mathf.Cos(angle) - joystickInput.y * Mathf.Sin(angle),
            0f,
            joystickInput.x * Mathf.Sin(angle) + joystickInput.y * Mathf.Cos(angle));
    }

    private void Move(Vector2 move)
    {
        //Get angle between cam and player
        if (move != Vector2.zero && move != null)
        {
            SendMovementParameters(move);
            Vector3 dst = transform.position + GetInputRelativeToCamera(move) * speed * Time.deltaTime;
            if (!float.IsNaN(dst.x) && !float.IsNaN(dst.z))
            {
                transform.LookAt(dst, Vector3.up);
                transform.position = dst;
            }
        }
    }

    //Returns true while in the dash, returns false when it ends
    private bool ContinueDash()
    {
        if (currentDashTime < maxDashTime)
        {
            transform.position += dashDir * dashSpeed * Time.deltaTime;
            currentDashTime += Time.deltaTime;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SendMovementParameters(Vector2 value)
    {
        _animator.SetFloat("VelX", value.x);
        _animator.SetFloat("VelY", value.y);
        //Debug.Log(value);
    }

    void AddEffect(Effect new_effect)
    {
        Debug.Log("Adding effect" + new_effect.name);

        effects.Add(new_effect);

        //RecalculateStats(); 

        if (new_effect is AttackEffect)
        {
            _playerCombos.OnAddAttackEffect(((AttackEffect)new_effect).GetAttackIdentifier());
        }
    }

    public void PickUpRelic(Relic new_relic)
    {
        Debug.Log("Picked up relic " + new_relic.relic_name + ": " + new_relic.description);
        relics.Add(new_relic);
        foreach (Effect effect in new_relic.effects)
        {
            AddEffect(effect);
        }  
    }

    public Stat GetStrength()
    {
        return strength;
    }
}