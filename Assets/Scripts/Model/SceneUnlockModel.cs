using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class SceneUnlockModel : AbstractModel,ICanGetUtility ,ICanGetModel
{
    private const string SCENE_UNLOCK_SIGN = "A_SceneUnlock";
    private const string SECNE_UNLOCK_UNIT_SIGN = "A_SceneUnlockUnit";
    
    private BindableProperty<int> mSceneIndex;
    private BindableProperty<int> mUnitIndex;
    private SaveDataUtility mStorage;
    private StageModel mStageModel;

    public int RemainingStars => mStageModel.RemainingStars;
    public int SceneIndex => mSceneIndex.Value;
    public int SceneUnlockUnitIndex => mUnitIndex.Value;

    protected override void OnInit()
    {
        mStageModel = this.GetModel<StageModel>();
        mStorage = this.GetUtility<SaveDataUtility>();
        mSceneIndex = new BindableProperty<int>();
        mUnitIndex = new BindableProperty<int>();

        mSceneIndex.SetValueWithoutEvent(mStorage.LoadIntValue(SCENE_UNLOCK_SIGN));
        mSceneIndex.Register(value =>
        {
            mStorage.SaveInt(SCENE_UNLOCK_SIGN, value);
            mUnitIndex.SetValueWithoutEvent(mStorage.LoadIntValue($"{SECNE_UNLOCK_UNIT_SIGN}{value}"));
        });

        mUnitIndex.SetValueWithoutEvent(
            mStorage.LoadIntValue($"{SECNE_UNLOCK_UNIT_SIGN}{mSceneIndex.Value}"));
        mUnitIndex.Register(value =>
        {
            SaveUnitIndex(value);
        });
    }

    public void UseStar(int value)
    {
        mStageModel.UsedStar(value);
    }

    public void UpdateSceneIdx(int value)
    {
        mSceneIndex.Value = value;
        StringEventSystem.Global.Send(GameDefine.GameConst.UNLOCK_NEW_SCENES);
    }

    public void AddUnitIndex()
    {
        mUnitIndex.Value++;
    }

    public int GetSceneUnitIndex(int sceneIdx)
    {
        return mStorage.LoadIntValue($"{SECNE_UNLOCK_UNIT_SIGN}{sceneIdx}");
    }

    private void SaveUnitIndex(int value)
    {
        mStorage.SaveInt($"{SECNE_UNLOCK_UNIT_SIGN}{mSceneIndex.Value}", value);
    }
}
