using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUpDown : MonoBehaviour
{

    public LeanTweenType easeType;
    public float targetPosition;
    public float time;

    private void OnEnable()
    {
        LeanTween.moveY(gameObject, targetPosition, time).setLoopPingPong().setEase(easeType);
    }

}
