namespace Game.Water
{
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

    public struct RefreshItemEvent
    {
        public int itemID;
    }

    public struct GameStartEvent
    {
    }
}
