using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class GeraltAttacks : MonoBehaviour
{
    public PlayerIndex playerIndex;
    private GamePadState state;

    public AttackList attacks;

    private AuxButton xButton;
    private AuxButton yButton;

    private Attack entryPoint;//This is not really an attack, it's the idle that goes to x and y

    private Animator anim;

    private Attack currAttack;
    private float lastInputTime = 0f;
    private float extraInputWindow = 1f;//In seconds.
    private string nextInput = "";

    private void Start()
    {
        anim = GetComponent<Animator>();

        xButton = new AuxButton();
        xButton.name = "x";
        yButton = new AuxButton();
        yButton.name = "y";

        entryPoint = attacks.attacks.Find(attack => attack.name == "_");//_ is idle
        CurrAttack = entryPoint;
        lastInputTime = Time.time;
        Debug.Log("HERE 1");
        UnityEditor.Animations.AnimatorController controller = anim.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
        foreach (UnityEditor.Animations.ChildAnimatorState state in controller.layers[0].stateMachine.states)
        {
            state.state.timeParameterActive = true;
            state.state.= state.time + extraInputWindow;
        }
    }

    private void Update()
    {
        state = GamePad.GetState(playerIndex);
        xButton.UpdateValue(state.Buttons.X);
        yButton.UpdateValue(state.Buttons.Y);

        //Debug.Log(Time.time - lastInputTime);

        RegisterNewInput(xButton);
        RegisterNewInput(yButton);
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
            Debug.Log("HERE 2");
        }
    }

    private void PlayNextCombo()
    {
        //If the combo has finished
        if (Time.time - lastInputTime >= anim.GetCurrentAnimatorStateInfo(0).length)
        {
            if (nextInput != "")
            {
                //INFO: Check if the input given matches any of the inputs of the next attacks
                Attack nextAttack = FindNextAttack(CurrAttack, nextInput);
                if (nextAttack != null)
                {
                    CurrAttack = nextAttack;
                    lastInputTime = Time.time;
                    Debug.Log("HERE 3");
                }
                else
                {
                    CurrAttack = attacks.attacks.Find(attack => attack.name == "_" + nextInput);
                    lastInputTime = Time.time;
                    Debug.Log("Next input: " + nextInput);
                    Debug.Log("HERE 4");
                }
                nextInput = "";
                //Debug.Log("CURRENT ATTACK: " + currAttack.name);
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
            Debug.Log("HERE 5");
        }
    }

    private void RegisterNewInput(AuxButton button)
    {
        if (button.state == KEY_STATE.KEY_DOWN)
        {
            nextInput = button.name;
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
}
