using QFramework;
using QFramework.Example;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.UI;
using TMPro;

public class BombAni : MonoBehaviour
{
   
    public GameObject bomb;
    public SpriteRenderer boom;
    public TextMeshProUGUI textTime;

    [Header("动画控制")]
    [Range(0.1f, 5f)]
    public float animationSpeed = 1f;
    
    public Animator animator;         

    [Header("结束事件")]
    public UnityEvent Boomed;
    private void Start()
    { 
        // 获取动画组件并设置速度
       
        if (animator != null)
        {
            animator.speed = animationSpeed;
        }
       
       
    }
    public void SetBomb(bool isBomb=false,string count="")
    {
        textTime.text = count;
        bomb.SetActive(isBomb);
     
    }
    // 引爆炸弹
    public void BombBoom()
    { 
        boom.enabled = true;            
        animator.enabled = true;
        bomb.SetActive(false);         
       
        
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
        
        bomb.SetActive(false);

        UIKit.OpenPanel<UIRetry>();
    }
}
