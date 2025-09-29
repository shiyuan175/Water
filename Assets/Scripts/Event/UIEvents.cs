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