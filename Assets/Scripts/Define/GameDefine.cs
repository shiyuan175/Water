using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using UnityEngine;
using UnityEngine.UI;

namespace GameDefine
{
    public static class GameConst
    {
        public const int MaxVitality = 5;
        public const int RecoveryTime = 1800;
    }
    public enum GameType
    {
        Normal = 0,
        Bomb = 1,
        Count = 2,
        Step = 3,
        Hide = 4,
    }

    public enum WaterItem
    {
        None = 0,
        Ice = 1,
        BreakIce = 2,
        Bomb = 3,
    }

    public enum BottleType
    {
        None = 0,
        ClearShow = 1,
        NearShow = 2
    }

    public enum ItemType
    {
        ClearItem = 1001,       // ���Բ���Ч��
        MagnetItem = 1002,      // ħ���飬�������Debuff(�ϰ�������Ч��)
        MakeColorItem = 1003,   // ������ص���ɫ��ƿ����
        ChangeGreen = 2001,     // ��ĳ����ɫ��Ϊ��ɫ-���1
        ChangeOrange = 2002,    // ��ĳ����ɫ��Ϊ��ɫ-���7
        ChangePink = 2003,      // ��ĳ����ɫ��Ϊ��ɫ-���3
        ChangePurple = 2004,    // ��ĳ����ɫ��Ϊ��ɫ-���10
        ChangeYellow = 2005,    // ��ĳ����ɫ��Ϊ��ɫ-���6
        ChangeDarkBlue = 2006,  // ��ĳ����ɫ��Ϊ����ɫ-���4
        ClearPink = 3001,       // ������з�ɫˮ��-���3
        ClearOrange = 3002,     // ������г�ɫˮ��-���7
        ClearBlue = 3003,       // ���������ɫˮ��-���4
        ClearYellow = 3004,     // ������л�ɫˮ��-���6
        ClearDarkGreen = 3005,  // �����������ɫˮ-����9
        ClearRed = 3006,        // ������к�ɫˮ��-���2
        ClearGreen = 3007,      // ���������ɫˮ��-���1
    }
    public enum LanguageType
    {
        zh = 0,
        ja = 1,
        en = 2,
        ko = 3,
    }
}
