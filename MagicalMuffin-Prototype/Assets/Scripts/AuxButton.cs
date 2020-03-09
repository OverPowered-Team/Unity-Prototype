using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class AuxButton
{
    public string name;
    public KEY_STATE state = KEY_STATE.KEY_IDLE;

    public void UpdateValue(ButtonState new_state)
    {
        if (new_state == ButtonState.Pressed)
        {
            if (state == KEY_STATE.KEY_IDLE)
            {
                state = KEY_STATE.KEY_DOWN;
            }
            else if (state == KEY_STATE.KEY_DOWN)
            {
                state = KEY_STATE.KEY_REPEAT;
            }

        }
        if (new_state == ButtonState.Released)
        {
            if (state == KEY_STATE.KEY_REPEAT || state == KEY_STATE.KEY_DOWN)
            {
                state = KEY_STATE.KEY_UP;
            }
            else
            {
                state = KEY_STATE.KEY_IDLE;
            }
        }
    }
}
public enum KEY_STATE
{
    KEY_DOWN,
    KEY_REPEAT,
    KEY_UP,
    KEY_IDLE,
}