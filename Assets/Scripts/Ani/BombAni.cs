using QFramework;
using QFramework.Example;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class BombAni : MonoBehaviour
{
   
    public SpriteRenderer bomb;
    public SpriteRenderer boom;


    [Header("动画控制")]
    [Range(0.1f, 5f)]
    public float animationSpeed = 1f;
    
    public Animator animator;         // 爆炸动画控制器

    [Header("结束事件")]
    public UnityEvent Boomed;
    private void Start()
    {
        
        
        // 获取动画组件并设置速度
        animator = boom.GetComponent<Animator>();
        if (animator != null)
        {
            animator.speed = animationSpeed;
        }
    }
    public void SetBomb(bool isBomb=false)
    {
        
        bomb.enabled = isBomb;
        
    }
    // 引爆炸弹
    public void BombBoom()
    {
        bomb.enabled = false;          
        boom.enabled = true;            
        animator.enabled = true;
        Debug.Log(animator);
        if (animator != null)
        {
            animator.Play("boom", -1, 0f); 
        }
    }

    // 动态调整动画速度
    public void SetAnimationSpeed(float speed)
    {
        animationSpeed = Mathf.Clamp(speed, 0.1f, 5f);

        if (animator != null)
        {
            animator.speed = animationSpeed;
        }
    }

    public void OnBoomAniEnd()
    {
        Boomed.Invoke();
        animator.enabled = false;
        
        boom.enabled = false;
        bomb.enabled = false;
        Debug.Log(boom.enabled);
        UIKit.OpenPanel<UIRetry>();
    }
}
