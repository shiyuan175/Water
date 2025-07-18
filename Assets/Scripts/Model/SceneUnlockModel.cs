using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class SceneUnlockModel : AbstractModel,ICanGetUtility
{
    //如果后续要新增场景，需要在做一个界面，ScrollView相关的，滑动查看解锁进度的,应该是坐在这

    private const string USED_STAR_SIGN = "A_UsedStar";
    private const string SCENE_UNLOCK_SIGN = "A_SceneUnlock";
    private const string SECNE_UNLOCK_UNIT_SIGN = "A_SceneUnlockUnit";

    private BindableProperty<int> mSceneIndex;
    private BindableProperty<int> mUnitIndex;
    private BindableProperty<int> mUsedStar;
    private SaveDataUtility mStorage;

    //当前关卡 - 1
    public int CountStar => mStorage.GetCurrentLevel() - 1;
    public int UsedStar => mUsedStar.Value;
    public int RemainingStar => CountStar - UsedStar;
    public int SceneIndex => mSceneIndex.Value;
    public int SceneUnlockUnitIndex => mUnitIndex.Value;
    //场景编号从0计算
    public bool SceneUnLockOverState => mSceneIndex.Value + 1 > GameDefine.GameConst.SceneUnlock.Count;

    protected override void OnInit()
    {
        mStorage = this.GetUtility<SaveDataUtility>();
        mUsedStar = new BindableProperty<int>();
        mSceneIndex = new BindableProperty<int>();
        mUnitIndex = new BindableProperty<int>();

        mUsedStar.SetValueWithoutEvent(mStorage.LoadIntValue(USED_STAR_SIGN));
        mUsedStar.Register(value =>
        {
            mStorage.SaveInt(USED_STAR_SIGN, value);

        });

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
        mUsedStar.Value += value;
    }

    public void AddSceneIndex()
    {
        mSceneIndex.Value++;
    }

    public void AddUnitIndex()
    {
        mUnitIndex.Value++;
    }

    private void SaveUnitIndex(int value)
    {
        mStorage.SaveInt($"{SECNE_UNLOCK_UNIT_SIGN}{mSceneIndex.Value}", value);
    }
}
