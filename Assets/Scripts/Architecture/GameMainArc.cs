using GameDefine;
using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMainArc : Architecture<GameMainArc>
{
    protected override void Init()
    {
        ResKit.Init();
        RegisterUtilitys();
        CheckUpdateJsonFile();

        RegisterModels();
        
        CreateInstance();
        ActivityStart();
    }

    private void RegisterModels()
    {
        RegisterModel(new StageModel());
        RegisterModel(new PotionActivityModel());
        RegisterModel(new BannerActivityModel());
        RegisterModel(new RankDataModel());
        RegisterModel(new TierRankActivityModel());
        RegisterModel(new VolcanicActivityModel());
        RegisterModel(new RocketActivityModel());
        RegisterModel(new HighTowerActivityModel());
        RegisterModel(new MagicStreakActivityModel());
        RegisterModel(new TurnTableADActivityModel());
        RegisterModel(new SceneUnlockModel());
        RegisterModel(new DailyRewardADActivityModel());
        RegisterModel(new BattlePassModel());
        RegisterModel(new SepecialOfferADActivityModel());
        RegisterModel(new RemoveADACtivityModel());
    }

    private void RegisterUtilitys()
    {
        RegisterUtility(new SaveDataUtility());
        RegisterUtility(new RewardGrantUtility());
        RegisterUtility(new LanguageUtility());
        RegisterUtility(new JsonFileUtility());
    }

    //Json数据更新
    private void CheckUpdateJsonFile()
    {
        JsonFileUtility jsonUtility = this.GetUtility<JsonFileUtility>();
        _ = jsonUtility.UpdateJsonFiles();
    }

    //单例构建
    private void CreateInstance()
    {
        //ResourceManager.Instance.LoadABPackage("uieveladdheart_prefab");
        //ResourceManager.Instance.LoadABPackage("uilevelclear_prefab");
        //ResourceManager.Instance.LoadABPackage("uilevelmain_prefab");
        //ResourceManager.Instance.LoadFont();
        TextManager textManager = TextManager.Instance;
        ShareManager shareManager = ShareManager.Instance;
        AnalyticsManager analyticsManager = AnalyticsManager.Instance;
        TenjinManager tenjinManager = TenjinManager.Instance;
        TopOnADManager topOnADManager = TopOnADManager.Instance;
        HealthManager healthManager = HealthManager.Instance;
        CountDownTimerManager.Instance.StartEasternMidnightTimer(GameDefine.GameConst.FIRST_LAUNCH_SIGN);
    }

    //活动开启
    private void ActivityStart()
    {
        //首个横幅活动
        var saveData = this.GetUtility<SaveDataUtility>();
        if (saveData.GetCurrentLevel() >= GameConst.WIN_STREAK_BEGIN_LEVEL)
            CountDownTimerManager.Instance.StartTimer(GameConst.RANKA_ACTIVITY_SIGN, 1440f);
    }
}
