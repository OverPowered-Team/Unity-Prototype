﻿using System.Collections;
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
    private float extraInputWindow = 0.2f;//In seconds.
    private Attack currAttack;

    private Attack entryPoint;//This is not really an attack, it's the idle that goes to x and y

    private Animator anim;

    private string nextInput = "";

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
        PlayNextCombo();
        ComboTimeout();
    }

    //TODO: Idle should be the exception, it shouldnt' wait to start the next attack
    private void PlayNextCombo()
    {
        //If the combo has finished
        if (Time.time - lastInputTime > anim.GetCurrentAnimatorStateInfo(0).length)
        {
            if (nextInput == "")
            {
                CurrAttack = attacks.attacks.Find(attack => attack.name == "_");
            }
            else
            {
                //INFO: Check if the input given matches any of the inputs of the next attacks
                Attack nextAttack = FindNextAttack(currAttack, nextInput);
                if (nextAttack != null)
                {
                    lastInputTime = Time.time;
                    CurrAttack = nextAttack;
                }
                else
                {
                    CurrAttack = attacks.attacks.Find(attack => attack.name == "_" + nextInput);
                }
                nextInput = "";
                Debug.Log("CURRENT ATTACK: " + currAttack.name);
            }

        }
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
