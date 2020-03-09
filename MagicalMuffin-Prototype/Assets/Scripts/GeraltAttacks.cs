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

    private float lastInputTime = 0f;
    private float inputBlockTime = 0f;//In seconds. TODO: Define if this should be individual for each attack or the same for all attacks
    private float extraInputWindow = 0.2f;//In seconds.
    private Attack currAttack;

    private Attack entryPoint;//This is not really an attack, it's the idle that goes to x and y

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();

        xButton = new AuxButton();
        xButton.name = "x";
        yButton = new AuxButton();
        yButton.name = "y";

        entryPoint = attacks.attacks.Find(attack => attack.name == "_");
        CurrAttack = entryPoint;
    }

    private void Update()
    {
        state = GamePad.GetState(playerIndex);
        xButton.UpdateValue(state.Buttons.X);
        yButton.UpdateValue(state.Buttons.Y);

        RegisterNewInput(xButton);
        RegisterNewInput(yButton);
        ComboTimeout();
    }

    //INFO: Reset combo window input time passes (just a little bit (extraInputWindow) after the animation finishes)
    private void ComboTimeout()
    {
        if (Time.time - lastInputTime > anim.GetCurrentAnimatorStateInfo(0).length + extraInputWindow)
        {
            CurrAttack = attacks.attacks.Find(attack => attack.name == "_");
        }
    }

    private void RegisterNewInput(AuxButton button)
    {
        if (button.state == KEY_STATE.KEY_DOWN)
        {
            if (lastInputTime + inputBlockTime < Time.time)
            {
                //Check if the input given matches any of the inputs of the next attacks
                Attack nextAttack = FindNextAttack(currAttack, button.name);
                if (nextAttack != null)
                {
                    lastInputTime = Time.time;
                    CurrAttack = nextAttack;
                }
                else
                {
                    CurrAttack = attacks.attacks.Find(attack => attack.name == "_" + button.name);
                }
                //TODO: Wait until the previous attack finishes to start playig this animation
                //TODO: Keep the last attack, and if the player presses another button in the time limit, change the combo (test if this helps the player or is frustrating)

                //TODO: Keep the current attack before switching to the next animation
                Debug.Log("CURRENT ATTACK: " + currAttack.name);
            }
        }
    }

    //Returns null if there isn't an attack that follows with the given input
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
