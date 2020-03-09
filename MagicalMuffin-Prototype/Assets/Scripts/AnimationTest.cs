using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTest : MonoBehaviour
{
    public AnimationClip animClip;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.Play("jump");
    }

    void Update()
    {
        
    }
}
