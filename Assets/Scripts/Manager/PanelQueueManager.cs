using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class PanelQueueManager : MonoSingleton<PanelQueueManager>
{
    public readonly Queue<Func<bool>> mPanelQueue = new ();
    //作用于需要链式开启多面板场景(胜利结算UIGetCoin必须作为最后一个面板传入、因为有加金币等动画)
    //使用方法:
    //在胜利/退出结算时将所有面板入列，面板关闭的时候发送事件(是否发送事件参照下方注释)
    //这些面板在 UIPanelData 记录一个可空bool
    //手动打开不传,关闭时不发事件
    //由堆栈打开的时候，会传入true。也就是结算时 面板入列的委托 打开面板传入True，这样关闭的时候会发送事件
    public override void OnSingletonInit()
    {
        StringEventSystem.Global.Register(GameDefine.GameConst.MANAGER_OPEN_NEXT_PANEL, () => NotifyPanelClosed())
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    /// <summary>
    /// 面板入列
    /// </summary>
    /// <param name="openPanelFunc"></param>
    public void Enqueue(Func<bool> openPanelFunc)
    {
        if (!mPanelQueue.Contains(openPanelFunc))
            mPanelQueue.Enqueue(openPanelFunc);
    }

    /// <summary>
    /// 启用(弹出首个面板)
    /// </summary>
    public void PopFirstPanel()
    {
        ExecuteNext();
    }
    
    /// <summary>
    /// 由面板关闭时调用：告知堆栈继续
    /// </summary>
    private void NotifyPanelClosed()
    {
        ExecuteNext();
    }

    private void ExecuteNext()
    {
        if (mPanelQueue.Count == 0) return;

        var _next = mPanelQueue.Dequeue();
        bool _opened = _next?.Invoke() ?? false;

        if (!_opened)
            ExecuteNext();
    }
}
