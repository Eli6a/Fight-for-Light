using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroductionController : MonoBehaviour
{
    Animator animator;
    AnimatorStateInfo animatorStateInfo;
    public float normalizedTime;
    public bool animationFinished;
    public CanvasScript canvasScript;
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        try
        {

            ActionInputControl();
            AnimationTextFinished();
            if (animationFinished)
            {
                canvasScript.LoadNextLevel();
                animationFinished = false;
            }
        }
        catch
        {

        }
        
    }

    void AnimationTextFinished()
    {
        animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        normalizedTime = animatorStateInfo.normalizedTime;

        if (normalizedTime > 1.0f)
        {
            animationFinished = true;
        }
    }

    void ActionInputControl()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            animationFinished = true;
        }

    }

    public void AnimationFinished()
    {
        animationFinished = true;
    }
}
