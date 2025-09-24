using GameDefine;
using JsonFileData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyTaskADActivity : BaseGameADActivity<GameObject>
{
    public override string ActivitySign => GameConst.TURNTABLE_AD_ACTIVITY_SIGN;
    public override string ActivityID => GetType().Name;
    public override int ActivityBeginLevel => GameConst.DT_BEGIN_LEVEL;


    private DailyTaskActivityData dailyTaskActivityData;
    public override GameActivityStatus ActivityStatus
    {
        get
        {
            if (mSaveUtility.GetCurrentLevel() < ActivityBeginLevel)
            {
                return GameActivityStatus.Locked;
            }
            else
                return GameActivityStatus.Active;
        }
    }

    public override void StartActivity()
    {
        throw new System.NotImplementedException();
    }

    public override void RestartActivity()
    {
        throw new System.NotImplementedException();
    }

    public override void ADPlaybackCompleted(GameObject target)
    {
        throw new System.NotImplementedException();
    }
}
