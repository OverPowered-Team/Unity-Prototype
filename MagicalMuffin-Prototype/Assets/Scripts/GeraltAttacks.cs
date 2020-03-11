using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GeraltAttacks : MonoBehaviour
{
    public AttackList attacks;

    private Animator anim;
    private List<Attack> startingCombos;

    private Attack currAttack = null;
    private float lastInputTime = 0f;
    [Tooltip("In seconds")]
    public float extraInputWindow;
    private string nextInput = "";

    private playerController _playerController;
    [HideInInspector] public float lastAttackFinishTime;

    Dictionary<UnityEngine.InputSystem.Controls.ButtonControl, string> buttonString;

    private void Start()
    {
        anim = GetComponent<Animator>();
        _playerController = GetComponent<playerController>();
        
        buttonString = new Dictionary<UnityEngine.InputSystem.Controls.ButtonControl, string>();
        buttonString.Add(_playerController.gamepad.buttonSouth, "a");
        buttonString.Add(_playerController.gamepad.buttonWest,  "x");
        buttonString.Add(_playerController.gamepad.buttonNorth, "y");
        buttonString.Add(_playerController.gamepad.buttonEast,  "b");

        startingCombos = new List<Attack>();
        startingCombos.Add(attacks.attacks.Find(attack => attack.name == "x"));
        startingCombos.Add(attacks.attacks.Find(attack => attack.name == "y"));

        lastInputTime = Time.time;
    }

    float GetAnimatorStateSpeed(string name)
    {
        UnityEditor.Animations.AnimatorController ac = anim.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
        UnityEditor.Animations.ChildAnimatorState[] states = ac.layers[0].stateMachine.states;
        foreach (UnityEditor.Animations.ChildAnimatorState state in states)
        {
            if (state.state.name == name)
            {
                return state.state.speed;
            }
        }
        return 1f;
    }

    private bool FinishedAttack()
    {
        if (CurrAttack != null)
        {
            return Time.time - lastInputTime >= GetAnimationClip(CurrAttack.animation_id).length / GetAnimatorStateSpeed(CurrAttack.name);
        }
        else
        {
            return true;
        }
        
    }

    public void CancelCombo()
    {
        currAttack = null;
    }

    public void UpdateAttack()
    {
        if (lastAttackFinishTime - Time.time > extraInputWindow)
        {
            CurrAttack = null;
        }

        RegisterNewInput(_playerController.gamepad.buttonWest);
        RegisterNewInput(_playerController.gamepad.buttonNorth);
        PlayNextCombo();
    }

    public void PlayNextCombo()
    {
        //If the combo has finished
        if (FinishedAttack())
        {
            if (nextInput != "")
            {
                //INFO: Check if the input given matches any of the inputs of the next attacks
                Attack nextAttack = FindNextAttack(CurrAttack, nextInput);
                if (nextAttack != null)
                {
                    CurrAttack = nextAttack;
                    lastInputTime = Time.time;
                }
                else
                {
                    CurrAttack = attacks.attacks.Find(attack => attack.name == nextInput);
                    lastInputTime = Time.time;
                }
                nextInput = "";
            }
            else
            {
                lastAttackFinishTime = Time.time;

                Vector2 move = _playerController.gamepad.leftStick.ReadValue();
                if (move == Vector2.zero)
                {
                    anim.CrossFade("idle", extraInputWindow);
                }
                else
                {
                    anim.CrossFade("Movement", extraInputWindow);
                }
                _playerController.currState = PlayerState.ATTACK_RETURN;
            }
        }
    }

    public void RegisterNewInput(UnityEngine.InputSystem.Controls.ButtonControl button)
    {
        if (button.wasPressedThisFrame)
        {
            nextInput = buttonString[button];
        }
    }

    //INFO: Returns null if there isn't an attack that follows with the given input
    private Attack FindNextAttack(Attack currAttack, string input)
    {
        string attackName;

        if (CurrAttack == null)
        {
            attackName = input;
        }
        else
        {
            attackName = currAttack.name + input;
        }

        return attacks.attacks.Find(attack => attack.name == attackName);
    }

    public Attack CurrAttack
    {
        set
        {
            currAttack = value;
            anim.Play(value.animation_id);
        }
        get
        {
            return currAttack;
        }
    }

    private AnimationClip GetAnimationClip(string clipName)
    {
        foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip;
            }
        }
        return null;
    }

    //INFO: Repeatable 1 attack combos
    //- Unity doesn't let us play the same animation after one has finished
    //- If for example, _x didn't have a combo, and we were to press x after the animation has ended and it's in the inputWindowTime, it wouldn't play it again
    //- To work around this, you can add a transition from that single repetable combo to the idle state
}
