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
    private float inputBlockTime = 0f;//TODO: Define if this should be individual for each attack or the same for all attacks
    private Attack currAttack;

    private Attack entryPoint;//This is not really an attack, it's the idle that goes to x and y

    private void Start()
    {
        xButton = new AuxButton();
        xButton.name = "x";
        yButton = new AuxButton();
        yButton.name = "y";

        entryPoint = attacks.attacks.Find(attack => attack.name == "_");
        currAttack = entryPoint;
    }

    private void Update()
    {
        state = GamePad.GetState(playerIndex);
        xButton.UpdateValue(state.Buttons.X);
        yButton.UpdateValue(state.Buttons.Y);

        RegisterNewInput(xButton);
        RegisterNewInput(yButton);
        //TODO: Reset combo window input time passes (just a little bit after the animation finishes)
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
                    currAttack = nextAttack;
                }
                else
                {
                    currAttack = attacks.attacks.Find(attack => attack.name == "_" + button.name);
                }
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
}
