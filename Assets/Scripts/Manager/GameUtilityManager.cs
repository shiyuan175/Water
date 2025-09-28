using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TickTask;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.Collections.Specialized.BitVector32;

namespace TickTask
{
    public class ScheduledTask
    {
        public CancellationTokenSource Cts {  get; }
        public Action Action { get; }

        public ScheduledTask(Action action) 
        { 
            Action = action;
            Cts = new CancellationTokenSource();
        }
    }
}

public class GameUtilityManager : MonoSingleton<GameUtilityManager>
{
    private Dictionary<object, ScheduledTask> mTasks;
    private CancellationTokenSource mCts;

    protected override void OnDestroy()
    {
        mCts.Cancel();
        base.OnDestroy();
    }

    public override void OnSingletonInit()
    {
        mTasks = new();
        mCts = new CancellationTokenSource();
        _ = RunAsync(mCts.Token);
    }

    /// <summary>
    ///  将源 UI 对象的位置映射到目标 UI 对象所在 Canvas 的局部坐标
    /// </summary>
    ///<param name="sourceObj">源目标对象</param>
    /// <param name="targetObj">要应用的对象</param>
    public void GetLocalPositionInCanvas(RectTransform sourceObj, RectTransform targetObj, Action<Vector2> callback = null)
    {
        Canvas _sourceObjCanvas = sourceObj.GetComponentInParent<Canvas>().rootCanvas;
        Canvas _targetObjCanvas = targetObj.GetComponentInParent<Canvas>().rootCanvas;

        //延迟一帧构建Layout Group
        ActionKit.DelayFrame(1, () =>
        {
            //目标对象坐标转为世界坐标
            var _screenPoint = _sourceObjCanvas.worldCamera.WorldToScreenPoint(sourceObj.position);

            // 转化画布
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                 targetObj.parent as RectTransform,
                 _screenPoint,
                 _targetObjCanvas.worldCamera,
                 out Vector2 _localPoint);
            targetObj.anchoredPosition = _localPoint;
            callback?.Invoke(_localPoint);
        }).Start(this);
    }

    /// <summary>
    /// 获取是否点击到目标UI
    /// </summary>
    /// <param name="screenPosition"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool IsPointerOverTargetUI(Vector2 screenPosition, GameObject target)
    {
        if (target == null)
            return false;

        Canvas _targetCanvas = target.GetComponentInParent<Canvas>();

        GraphicRaycaster raycaster = _targetCanvas.GetComponent<GraphicRaycaster>();
        if (raycaster == null)
            return false;

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject == target)
                return true;
        }

        return false;
    }

    /// <summary>
    /// 向调度器注册一个每秒执行一次的任务
    /// </summary>
    /// <param name="scheduled"></param>
    public void RegisterTask(object owner, Action action)
    {
        var task = new ScheduledTask(action);
        mTasks[owner] = task;
    }

    /// <summary>
    /// 取消任务
    /// </summary>
    /// <param name="owner"></param>
    public void UnregisterTask(object owner)
    {
        if (mTasks.TryGetValue(owner, out var task))
        {
            task.Cts.Cancel();  
            mTasks.Remove(owner); 
        }
    }

    private async Task RunAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            foreach (var task in mTasks.Values)
            {
                try
                {
                    task.Action.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"任务调度器异常: {ex}");
                }
            }

            try
            {
                await Task.Delay(1000, token);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }
}
