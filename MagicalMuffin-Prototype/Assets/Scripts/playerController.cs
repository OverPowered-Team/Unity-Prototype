using UnityEngine;
using UnityEngine.InputSystem;

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
    private GeraltAttacks _playerCombo;

    void Awake()
    {
        cam_tansform = Camera.main.transform;
        _playerCombo = GetComponent<GeraltAttacks>();
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
                    if (CheckAttackInput())
                    {
                        _playerCombo.UpdateAttack();
                        currState = PlayerState.ATTACK;
                    }
                    if (CheckDashInput())
                    {
                        StartDash();
                        currState = PlayerState.DASH;
                    }
                    if (move != Vector2.zero)
                    {
                        Move(move);
                        _animator.Play("Movement");
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
                    }
                    if (CheckAttackInput())
                    {
                        _playerCombo.UpdateAttack();
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
                    _playerCombo.UpdateAttack();
                    if (CheckDashInput())
                    {
                        StartDash();
                        _playerCombo.CancelCombo();
                        currState = PlayerState.DASH;
                    }
                    break;
                case PlayerState.ATTACK_RETURN:
                    if (_playerCombo.lastAttackFinishTime + _playerCombo.extraInputWindow >= Time.time)
                    {
                        float remainingTransition = _playerCombo.extraInputWindow - (Time.time - _playerCombo.lastAttackFinishTime);
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
                        Debug.Log("Finished transition");
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
                        _playerCombo.UpdateAttack();
                        currState = PlayerState.ATTACK;
                    }
                    if (CheckDashInput())
                    {
                        StartDash();
                        currState = PlayerState.DASH;
                    }
                    break;
            }
            //SendMovementParameters(move);
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

    private void StartDash()
    {
        dashDir = transform.forward;
        currentDashTime = 0.0f;
        _animator.Play("dash");
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
        _animator.SetFloat("VelX",value.x);
        _animator.SetFloat("VelY", value.y);
    }
}