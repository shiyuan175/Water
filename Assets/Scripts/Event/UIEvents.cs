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

public struct LevelClearEvent
{

}

public struct CoinChangeEvent
{
    public int coin;
}

public struct VitalityChangeEvent
{
}

public struct UnlimtItemEvent
{
}

public struct VitalityTimeChangeEvent
{
    public long timeOffset;
}

public struct UnlockSceneEvent
{
    public int scene;
    public int part;
}

public struct ReturnMainEvent
{
}

public struct RefreshItemEvent
{
}

public struct GameStartEvent
{
}

public struct AvatarEvent
{
    public int AvatarId;
    public int AvatarFrameId;
}