using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;
using UnityEngine.InputSystem;

public class GeraltAttacks : MonoBehaviour
{
    public PlayerIndex playerIndex;
    private GamePadState state;

    public AttackList attacks;

    

    private Attack entryPoint;//This is not really an attack, it's the idle that goes to x and y

    private Animator anim;

    private Attack currAttack;
    private float lastInputTime = 0f;
    public float extraInputWindow = 1f;//In seconds.
    private string nextInput = "";

    private float hardcodedOffset = 0.2f;
    //There seems to be some errors when comparing the animation length and the time passed in an animation.
    //And the result is that the length is a little bit longer.
    //So we substract this offset to avoid it getting stuck on the last frame of the animation.

    Gamepad gamepad = null;

    Dictionary<UnityEngine.InputSystem.Controls.ButtonControl, string> dic = new Dictionary<UnityEngine.InputSystem.Controls.ButtonControl, string>();

    private void Start()
    {
        anim = GetComponent<Animator>();

        dic.Add(gamepad.buttonSouth, "a");
        dic.Add(gamepad.buttonWest, "x");
        dic.Add(gamepad.buttonNorth, "y");
        dic.Add(gamepad.buttonWest, "b");

        entryPoint = attacks.attacks.Find(attack => attack.name == "_");//_ is idle
        CurrAttack = entryPoint;
        lastInputTime = Time.time;
    }

    private void Update()
    {
        state = GamePad.GetState(playerIndex);



        //Debug.Log(Time.time - lastInputTime);

        RegisterNewInput(gamepad.buttonWest);
        RegisterNewInput(gamepad.buttonNorth);
        InputOnIdle();
        PlayNextCombo();
        ComboTimeout();
    }

    //INFO: Idle is the exception among "attacks", it doesn't wait to finish its animation to start the next attack
    private void InputOnIdle()
    {
        if (CurrAttack.name == "_" && nextInput != "")
        {
            CurrAttack = attacks.attacks.Find(attack => attack.name == "_" + nextInput);
            lastInputTime = Time.time;
            nextInput = "";
        }
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

    private void PlayNextCombo()
    {
        //If the combo has finished
        if (Time.time - lastInputTime >= GetAnimationClip(CurrAttack.animation_id).length / GetAnimatorStateSpeed(CurrAttack.name))
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
                    CurrAttack = attacks.attacks.Find(attack => attack.name == "_" + nextInput);
                    lastInputTime = Time.time;
                    Debug.Log("Next input: " + nextInput);
                }
                nextInput = "";
                //Debug.Log("CURRENT ATTACK: " + currAttack.name);
            }
            else
            {
                anim.CrossFade("_", extraInputWindow);
            }
        }
    }

    //INFO: Reset combo window input time passes (just a little bit (extraInputWindow) after the animation finishes)
    private void ComboTimeout()
    {
        if (Time.time - lastInputTime > anim.GetCurrentAnimatorStateInfo(0).length + extraInputWindow)
        {
            CurrAttack = attacks.attacks.Find(attack => attack.name == "_");
            lastInputTime = Time.time;
        }
    }

    private void RegisterNewInput(UnityEngine.InputSystem.Controls.ButtonControl button)
    {
        if (button.wasPressedThisFrame)
        {
            nextInput = dic[button];
        }
    }

    //INFO: Returns null if there isn't an attack that follows with the given input
    private Attack FindNextAttack(Attack currAttack, string input)
    {
        //INFO: If we decide that the names of the attacks aren't the combination of their buttons, this should go through all the "currAttack.nextAttack" list and see if any of them matches our "input"
        return attacks.attacks.Find(attack => attack.name == currAttack.name + input);
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
