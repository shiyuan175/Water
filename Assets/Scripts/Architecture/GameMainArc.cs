using GameDefine;
using QFramework;
using UnityEngine;

public class GameMainArc : Architecture<GameMainArc>
{
    protected override void Init()
    {
        ResKit.Init();
        RegisterUtilitys();
        RegisterModels();
        CreateInstance();
    }
    
    

    private void RegisterModels()
    {
        RegisterModel(new GameGlobalModel());
        RegisterModel(new PotionActivityModel());
        RegisterModel(new RankDataModel());
        //RegisterModel(new TurnTableADActivityModel());
    }

    private void RegisterUtilitys()
    {
        RegisterUtility(new SaveDataUtility());
        RegisterUtility(new LanguageUtility());
        RegisterUtility(new LevelManagerUtility());
        RegisterUtility(new TwoBitUtility());
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
    }
}
