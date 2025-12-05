using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public abstract class BaseUIAdapter : MonoBehaviour
{
    [SerializeField] [Tooltip("UIAdapter更新顺序")]
    protected int frameCount = 1;

    [SerializeField] [Tooltip("是否只执行一次")]
    bool oneTime = false;
    bool mulitTime = true;

    #region 初始化
    void OnEnable()
    {
        // 设置是否只执行一次
        if(oneTime && mulitTime)
        {
            mulitTime = false;
            ExecuteAfterFrames(Adapter);
        }
        else if(!oneTime)
        {
            ExecuteAfterFrames(Adapter);
        }
            
        
    }
    public void ExecuteAfterFrames(System.Action action)
    {

        StartCoroutine(ExecuteAfterFramesCoroutine(action));
    }
    private IEnumerator ExecuteAfterFramesCoroutine(System.Action action)
    {
        for (int i = 0; i < frameCount; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        action?.Invoke();
    }
    #endregion

    protected abstract void Adapter();
}
