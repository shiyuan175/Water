using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 刷新文本事件
/// </summary>
public struct RefreshUITextEvent
{

}

public struct LevelStartEvent
{

}

public struct UnLockItem
{
    public NormalRewardsType PropType;
}

/// <summary>
/// 游戏胜利事件
/// </summary>
public struct ReturnToMainEvent
{
    public bool PassLevel;
}

public struct VitalityChangeEvent
{
}

public struct UnlockSceneBackEvent
{
   
}

public struct RefreshItemEvent
{
    public int itemID;
}

public struct GameStartEvent
{
}

/// <summary>
/// 头像/头像框切换事件
/// </summary>
public struct AvatarEvent
{
    public int AvatarId;
    public int AvatarFrameId;
}

public struct OnActivityStatusChanged
{
    public IGameActivity Sender;
    public Enum Status;
}

public struct ReportLevelEvent
{
    public int level;
    public int type;    //1.进入关卡 2.关卡结束
    public int? iswin;  //1.过关 2.失败;
}

