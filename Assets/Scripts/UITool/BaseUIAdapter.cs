using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public abstract class BaseUIAdapter : MonoBehaviour
{
    [SerializeField]
    [Tooltip("延后帧数，默认为1，设置此帧数控制不同的UIAdapter的执行顺序")] protected int frameCount = 1; 
    #region 动态调整的触发与初始化
    void OnEnable()
    {
        // 延后执行，保证不受到动态ui的影响
        ExecuteAfterFrames(Adapter);
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
