namespace Game.Water
{
    public class ABResourceDefine
    {
        public const string RANK_LEVEL_ATLAS_ASSETNAME = "RankLevelAtlas";
        public const string RANK_LEVEL_ATLAS_BUNDLENAME = "ranklevelatlas_spriteatlasv2";

        internal const string RANK_LEVEL_SPRITE_PREFIX = "RankLevel_";
    }

    public static partial class GameUtils
    {
        //后续配合枚举拓展(仅适用于图集内的所有精灵前缀名相同)
        public static string GetAtlasSpriteName(int suffix)
        {
            return $"{ABResourceDefine.RANK_LEVEL_SPRITE_PREFIX}{suffix}";
        }
    }
}

