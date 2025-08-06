using GameDefine;
using QFramework;

public class RegisterActivitiesCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        int currentLevel = this.GetUtility<SaveDataUtility>().GetCurrentLevel();
        if (currentLevel == GameConst.VA_BEGIN_LEVEL)
        {
            GameActivityManager.Instance.RegisterActivity<VolcanicActivity>();
        }
        if (currentLevel == GameConst.RA_BEGIN_LEVEL)
        {
            GameActivityManager.Instance.RegisterActivity<RocketActivity>();
        }
        if (currentLevel == GameConst.HTA_BEGIN_LEVEL)
        {
            GameActivityManager.Instance.RegisterActivity<HighTowerActivity>();
        }

        //通过第七关开启连胜活动
        if (currentLevel == GameConst.WIN_STREAK_BEGIN_LEVEL)
        {
            StringEventSystem.Global.Send(GameConst.START_POTION_ACTIVITY);
            //开启排行榜活动
            CountDownTimerManager.Instance.StartTimer(GameConst.RANKA_ACTIVITY_SIGN, 1440f);
        }
    }
}
