using QFramework;
using System.Collections.Generic;
using UnityEngine;
using GameDefine;
using System;
using QFramework.Example;
using System.Collections;
using Spine.Unity;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System.Reflection;

[MonoSingletonPath("[Level]/LevelManager")]
public class LevelManager : MonoBehaviour, IController, ICanSendEvent
{
    public static LevelManager Instance;
    public List<LevelCreateCtrl> levels = new List<LevelCreateCtrl>();
    public List<int> clearList = new List<int>();
    //带有阻碍的颜色(魔法布，藤曼，冰冻)
    public List<int> cantChangeColorList = new List<int>();
    public List<BottleCtrl> nowBottles = new List<BottleCtrl>();
    public List<BottleCtrl> iceBottles = new List<BottleCtrl>();

    public List<BottleCtrl> topBottle = new List<BottleCtrl>();
    public List<BottleCtrl> bottomBottle = new List<BottleCtrl>();
    public List<BottleCtrl> hideBottleList = new List<BottleCtrl>();

    public List<int> hideColor = new List<int>();
    public List<Color> waterColor = new List<Color>();
    public List<WaterSpriteInfo> waterSpriteInfos; 
    //public List<Sprite> waterTopSp;
    //public List<Sprite> waterSp;


    public LevelCreateCtrl.BottleProperty emptyBottle = new();
    public Transform gameCanvas;
    public List<GameObject> createFx = new List<GameObject>();
    public LevelCreateCtrl nowLevel;
    public Color ItemColor;

    public Material shineMaterial;

    public int levelId = 1, bombMaxNum, countDownNum, playingHideAnimCount;
    public bool ISPlayingHideAnim => playingHideAnimCount == 0;

    public int moveNum = 0;

    public GameObject broomBullet;
    public SkeletonGraphic mahoujinSpine;
    bool isFinish = false, isBomb = false;
    public bool isPlayAnim, isPlayFxAnim;

    public BottleCtrl nowHalf;
    public Material selectMaterial;
    public GameObject hideBg;
    //携带的道具
    public List<int> takeItem = new List<int>();
    public List<LevelManagerRecord> LevelManagerRecords = new List<LevelManagerRecord>();

    [SerializeField] private HorizontalLayoutGroup BottomBottleLayoutGroup;
    [SerializeField] private HorizontalLayoutGroup TopBottleLayoutGroup;
    [SerializeField] private List<BottleCtrl> bottles = new List<BottleCtrl>();

    private StageModel stageModel;

    private ResLoader mResLoader = ResLoader.Allocate();
    [HideInInspector]
    public TMP_FontAsset redFont;
    [HideInInspector]
    public TMP_FontAsset blueFont;
    [HideInInspector]
    public TMP_FontAsset greenFont;

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }

    private void Awake()
    {
        Instance = this;
        stageModel = this.GetModel<StageModel>();

        redFont = mResLoader.LoadSync<TMP_FontAsset>("font", "SourceHanSansCN-Bold SDF Red");
        blueFont = mResLoader.LoadSync<TMP_FontAsset>("font", "SourceHanSansCN-Bold SDF Blue");
        greenFont = mResLoader.LoadSync<TMP_FontAsset>("font", "SourceHanSansCN-Bold SDF Green");

        InitBottle();
    }

    private void Start()
    {
        //清空携带道具
        StringEventSystem.Global.Register("ClearTakeItem", () =>
        {
            takeItem.Clear();

        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        emptyBottle.numCake = 4;
        levelId = this.GetUtility<SaveDataUtility>().GetCurrentLevel();

        UIKit.OpenPanel<UIBegin>();

        if (levelId <= GameConst.NEWBIE_LEVEL_COUNT)
        {
            UIKit.OpenPanel<UIGameNode>();
            StartGame(levelId);
        }
    }

    private void OnDestroy()
    {
        mResLoader.Recycle2Cache();
        mResLoader = null;
    }

    #region 关卡重置初始化/进入关卡初始化

    /// <summary>
    /// 将瓶子数据初始化
    /// 初始化时调用/游戏结束时调用/退出关卡调用
    /// </summary>
    public void InitBottle()
    {
        TopBottleLayoutGroup.Hide();
        BottomBottleLayoutGroup.Hide();
        //foreach (var item in bottles)
        //{
        //    item.Init(emptyBottle, 0);
        //}
    }

    /// <summary>
    /// 开始游戏&初始化
    /// </summary>
    /// <param name="id"></param>
    public void StartGame(int id)
    {
        //cantClearColorList.Clear();
        cantChangeColorList.Clear();
        hideBottleList.Clear();
        levelId = id;
        var levelInfo = levels[levelId - 1];
        iceBottles.Clear();
        nowLevel = levelInfo;
        bombMaxNum = levelInfo.bombNum;
        countDownNum = levelInfo.countDownNum;

        // 关卡道具的开关可以移动到每个瓶子
        if (bombMaxNum > 0)
            isBomb = true;

        clearList = new List<int>(levelInfo.clearList);
        hideColor = new List<int>(levelInfo.hideList);
        nowBottles.Clear();

        nowHalf = null;

        TopBottleLayoutGroup.Show();
        BottomBottleLayoutGroup.Show();
        InitLevels(levelInfo);
    }

    /// <summary>
    /// 关卡重置初始化/进入关卡初始化
    /// </summary>
    public void InitLevels(LevelCreateCtrl levelInfo)
    {
        this.SendEvent<LevelStartEvent>();

        //清空操作记录 
        foreach (var bottle in bottles)
        {
            bottle.moveRecords.Clear();
        }
        LevelManagerRecords.Clear();

        // 清空步数 
        moveNum = 0;
        GameCtrl.Instance.InitPouringCount();
        //重置魔法布统计
        playingHideAnimCount = 0;
        isFinish = false;

        //Debug.Log("关卡重置初始化/首次进入关卡初始化");
        ShowBottleGo();
        InitBottle(levelInfo);
        BottleLayoutRefresh();
        UpdapeTopLayoutSpcing();
        UpdateButtomLayoutSpcing();

        //连胜去黑水
        if (levelId >= (int)GameDefine.UnLockMechanism.RemoveHideWinStreakLevel
            && stageModel.RemoveHideStreakWinNum >= GameConst.TEN_CONTINUE_WIN_NUM)
            StringEventSystem.Global.Send(GameConst.STREAK_WIN_REMOVE_HIDE);

        CheckGuideLevel();
    }

    public void CheckGuideLevel()
    {
        // 关卡引导
        switch (levelId)
        {
            // 新手关卡引导
            case (int)GameDefine.UIGuideLevel.UIGuideLevel1:
                UIKit.OpenPanel<UIGuideLevel1And2>(UILevel.PopUI, new UIGuideLevel1And2Data { level = 1 });
                break;
            case (int)GameDefine.UIGuideLevel.UIGuideLevel2:
                UIKit.OpenPanel<UIGuideLevel1And2>(UILevel.PopUI, new UIGuideLevel1And2Data { level = 2 });
                //UIKit.OpenPanel<UIGuideLevel2>(UILevel.PopUI);
                break;

            // 道具使用引导
            case (int)GameDefine.UIGuideLevel.UIGuideLevelStepBack:
                UIKit.OpenPanel<UIPaidItemsGuide>(UILevel.PopUI, new UIPaidItemsGuideData()
                {
                    PropType = NormalRewardsType.StepBack,
                });
                break;

            case (int)GameDefine.UIGuideLevel.UIGuideLevelRemoveHide:
                UIKit.OpenPanel<UIPaidItemsGuide>(UILevel.PopUI, new UIPaidItemsGuideData()
                {
                    PropType = NormalRewardsType.RemoveHide,
                });
                break;

            case (int)GameDefine.UIGuideLevel.UIGuideLevelAddBottle:
                UIKit.OpenPanel<UIPaidItemsGuide>(UILevel.PopUI, new UIPaidItemsGuideData()
                {
                    PropType = NormalRewardsType.AddOneBottle,
                });
                break;

            case (int)GameDefine.UIGuideLevel.UIGuideLevelHalfBottle:
                UIKit.OpenPanel<UIPaidItemsGuide>(UILevel.PopUI, new UIPaidItemsGuideData()
                {
                    PropType = NormalRewardsType.AddHalfBottle,
                });
                break;

            case (int)GameDefine.UIGuideLevel.UIGuideLevelRemoveAll:
                UIKit.OpenPanel<UIPaidItemsGuide>(UILevel.PopUI, new UIPaidItemsGuideData()
                {
                    PropType = NormalRewardsType.RemoveAll,
                });
                break;
        }

        // 新机制引导
        if (GameDefine.GameConst.GuideLevelInfo.TryGetValue(levelId, out (string guideText, string guideAnimName) value))
            UIKit.OpenPanel<UIGuideAnimPop>(new UIGuideAnimPopData
            {
                GuideText = value.guideText,
                GuideAnimName = value.guideAnimName
            });
    }

    /// <summary>
    /// 判断显示那些瓶子（现用于初始化关卡的瓶子）
    /// </summary>
    /// <param name="userItemSign"></param>
    /// public void ShowBottleGo(int num)
    public void ShowBottleGo()
    {
        nowBottles.Clear();

        for (int i = 0; i < topBottle.Count; i++)
        {
            var useBottle = topBottle[i];
            var num = nowLevel.topNum;
            useBottle.gameObject.SetActive(i < num);
            if (i < num)
                nowBottles.Add(useBottle);
        }

        for (int i = 0; i < bottomBottle.Count; i++)
        {
            var useBottle = bottomBottle[i];
            var num = nowLevel.bottomNum;
            useBottle.gameObject.SetActive(i < num);
            if (i < num)
                nowBottles.Add(useBottle);
        }
    }

    /// <summary>
    /// 根据数据初始化瓶子
    /// </summary>
    /// <param name="levelInfo"></param>
    public void InitBottle(LevelCreateCtrl levelInfo)
    {
        for (int i = 0; i < levelInfo.bottles.Count; i++)
        {
            var bottle = nowBottles[i];
            //瓶子还没做初始配置，直接全部设置4（要改）
            bottle.maxNum = 4;
            bottle.Init(levelInfo.bottles[i], i);
        }
    }

    /*/// <summary>
    /// 重置关卡(暂时保留重置关卡功能代码)
    /// </summary>
    public void RefreshLevel()
    {
        //Debug.Log("重置关卡");
        clearList = new List<int>(nowLevel.clearList);
        hideColor = new List<int>(nowLevel.hideList);
        changeList = new List<ChangePair>(nowLevel.changeList);
        hideBottleList.Clear();
        //cantClearColorList.Clear();

        nowHalf = null;
        InitLevels(nowLevel);

        //会触发两次重置(事件调用了StartGame，里面调用了InitLevels)，
        //如果后续有问题，直接在这调用StartGame做那些数据处理
        //this.SendEvent<GameStartEvent>();

        GameCtrl.Instance.InitGameCtrl();
    }*/

    /// <summary>
    /// 刷新瓶子布局(根节点)
    /// </summary>
    /// 取决于第二列是否有瓶子
    private void BottleLayoutRefresh()
    {
        var active = false;
        foreach (RectTransform child in BottomBottleLayoutGroup.transform)
        {
            if (child.gameObject.activeSelf)
            {
                active = true;
                break;
            }
        }
        BottomBottleLayoutGroup.gameObject.SetActive(active);
    }

    #endregion

    #region 局内机制道具相关

    /// <summary>
    /// 使用道具
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="fromTarget"></param>
    public void UseItem(int itemId, Transform fromTarget)
    {
        //记录触发位置
        Vector3 fromPos = fromTarget.position;
        ItemType _type = (ItemType)itemId;

        // 获取特性
        var _field = typeof(ItemType).GetField(_type.ToString());
        var _attr = _field?.GetCustomAttribute<GameAttributes.WaterColorState>();

        // 清除色块
        if (_attr is GameAttributes.ClearItemState _clearAttr)
        {
            ClearColor(_clearAttr.TargetIndex, fromPos);
            return;
        }

        // 改变颜色
        if (_attr is GameAttributes.ChangeColorItemState _changeAttr)
        {
            int fromColor = 0;
            for (int i = 0; i < nowLevel.changeList.Count; i++)
            {
                if (nowLevel.changeList[i].item == _type)
                {
                    fromColor = nowLevel.changeList[i].NeedChangeColor;
                    break;
                }
            }

            ChangeColor(fromColor, _changeAttr.TargetIndex, fromTarget);
            return;
        }

        switch ((ItemType)itemId)
        {
            //移除随机单色
            case ItemType.ClearRandomWaterItem:
                var clearlist = CheckCanClearList();
                int clearColorIdx = UnityEngine.Random.Range(0, clearlist.Count);
                int clearColor = clearlist[clearColorIdx];
                ClearColor(clearColor, fromPos);
                break;

            case ItemType.MagnetItem:
                ShowMahoujin();
                break;

            //添加单色(目前机制无使用)
            case ItemType.MakeColorItem:
                StartCoroutine(AddColor(fromPos));
                break;
        }
    }

    /// <summary>
    /// 获取能移除的颜色
    /// </summary>
    /// <returns></returns>
    public List<int> CheckCanClearList()
    {
        List<int> ret = new List<int>();
        foreach (var color in clearList)
        {
            //if (!cantClearColorList.Contains(color))
            if (!cantChangeColorList.Contains(color))
            {
                ret.Add(color);
            }
        }
        return ret;
    }

    /// <summary>
    /// 移除单色(扫帚)
    /// </summary>
    /// <param name="color"></param>
    /// <param name="fromPos"></param>
    public void ClearColor(int color, Vector3 fromPos)
    {
        StartCoroutine(ClearColorCoroutine(color, fromPos));
    }

    /// <summary>
    /// 移除单色以及动画后的逻辑
    /// </summary>
    /// <param name="color"></param>
    /// <param name="fromPos"></param>
    /// <returns></returns>
    IEnumerator ClearColorCoroutine(int color, Vector3 fromPos)
    {
        isPlayFxAnim = true;
        yield return new WaitForSeconds(1f);
        AudioKit.PlaySound("resources://Audio/BroomBullet");

        nowBottles.ForEach(bottle => bottle.PlayBroomBullet(color, fromPos));

        yield return new WaitForSeconds(0.2f);

        if (clearList.Contains(color))
            clearList.Remove(color);

        //先获取要移除单色的分布的瓶子列表(用于判断瓶子内是否都是该颜色)
        List<BottleCtrl> removeColorBottles = new List<BottleCtrl>();

        foreach (var bottle in nowBottles)
        {
            var bottleCtrl = bottle.CheckRemoveOneColor(color);
            if (bottleCtrl != null && !removeColorBottles.Contains(bottleCtrl))
                removeColorBottles.Add(bottleCtrl);

            //bottle.RemoveAllOneColor(color);
            bottle.CheckUnlockHide(color);
        }

        foreach (var bottle in removeColorBottles)
        {
            bottle.RemoveAllOneColor(color, bottle.waters.All(w => w == color));
        }

        StartCoroutine(WaitCheckFinish());
        UIKit.ClosePanel<UIMask>();
    }

    /// <summary>
    /// 魔法阵动画
    /// </summary>
    public void ShowMahoujin()
    {
        StartCoroutine(ShowMahoujinCoroutine());
    }

    /// <summary>
    /// 魔法阵动画
    /// </summary>
    /// <returns></returns>
    public IEnumerator ShowMahoujinCoroutine()
    {
        AudioKit.PlaySound("resources://Audio/MagicCircle");

        isPlayFxAnim = true;
        mahoujinSpine.Show();
        mahoujinSpine.AnimationState.SetEmptyAnimation(0, 0f);
        yield return new WaitForSeconds(2f);
        mahoujinSpine.AnimationState.SetAnimation(0, "attack", false);

        yield return new WaitForSeconds(3.34f);
        UIKit.ClosePanel<UIMask>();
        //Debug.Log("去除遮罩");
        RemoveAll();
        mahoujinSpine.Hide();
        isPlayFxAnim = false;
    }

    /// <summary>
    /// 添加颜色(魔法帽)
    /// </summary>
    /// <param name="fromPos"></param>
    /// <returns></returns>
    IEnumerator AddColor(Vector3 fromPos)
    {
        AudioKit.PlaySound("resources://Audio/AddColor");

        yield return new WaitForSeconds(1f);
        var bottleList = GetMakeColorBottle();
        List<BottleCtrl> useBottles = new List<BottleCtrl>();
        int addIdx = 0;
        while (hideColor.Count != 0)
        {
            int addColorIdx = UnityEngine.Random.Range(0, hideColor.Count);
            int addColor = hideColor[addColorIdx];
            var useBottle = bottleList[addIdx];
            if (useBottle.waters.Count < useBottle.maxNum)
            {
                useBottles.Add(useBottle);
                useBottle.AddColor(addColor, fromPos);
                hideColor.RemoveAt(addColorIdx);
                //Debug.Log("添加颜色 " + addColor);
            }
            else
            {
                addIdx++;
            }
        }

        foreach (var bottle in useBottles)
        {
            StartCoroutine(bottle.FinishHide());
        }

        UIKit.ClosePanel<UIMask>();
    }

    /// <summary>
    /// 判断能加色的瓶子
    /// </summary>
    /// <returns></returns>
    public List<BottleCtrl> GetMakeColorBottle()
    {
        List<BottleCtrl> ret = new List<BottleCtrl>();
        foreach (var bottle in nowBottles)
        {
            if (!bottle.isFreeze && bottle.waters.Count < 4 && !bottle.isClearHide && !bottle.isNearHide)
            {
                ret.Add(bottle);
            }
        }
        return ret;
    }

    /// <summary>
    /// 变色(药水瓶)
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="target"></param>
    public void ChangeColor(int from, int to, Transform target)
    {
        AudioKit.PlaySound("resources://Audio/ChangeColor");
        if (clearList.Contains(from))
        {
            clearList.Remove(from);
            clearList.Add(to);
        }
        foreach (var bottle in nowBottles)
        {
            bottle.ChangeColor(from, to, target);
            bottle.CheckHide();
            bottle.CheckFinish(true);
        }
        UIKit.ClosePanel<UIMask>();
    }

    #endregion

    #region 完成相关

    /// <summary>
    /// 合成完毕逻辑
    /// </summary>
    /// <param name="clearColor"></param>
    /// <param name="botterIdx"></param>
    public void FinishClear(int clearColor, int botterIdx)
    {
        //Debug.Log($"颜色编号：{clearColor}");
        //Debug.Log($"idx：{botterIdx}");
        foreach (var item in nowBottles)
        {
            item.CheckUnlockHide(clearColor);
            item.CheckNearHide(botterIdx);
        }

        StopCoroutine(WaitCheckFinish());
        StartCoroutine(WaitCheckFinish(clearColor));
    }

    /// <summary>
    /// 测试用方法
    /// </summary>
    public IEnumerator TestFinish()
    {
        isFinish = true;
        UIKit.OpenPanel<UIMask>(UILevel.PopUI);
        yield return new WaitForSeconds(1);
        this.GetUtility<SaveDataUtility>().SaveLevel(levelId + 1);

        //前五关
        if (levelId < 5)
        {
            StartGame(levelId + 1);
            UIKit.ClosePanel<UIMask>();
        }
        else
        {
            this.SendCommand<PassLevelCommand>();
            StringEventSystem.Global.Send(GameConst.VICTORY_EVENT);
        }
    }

    /// <summary>
    /// 胜利后&胜利动画后逻辑处理
    /// </summary>
    /// <param name="clearColor"></param>
    /// <returns></returns>
    public IEnumerator WaitCheckFinish(int clearColor = 0)
    {
        if (clearColor != 0 && clearList.Count > 0)
            clearList.Remove(clearColor);

        if (clearList.Count == 0 && !isFinish)
        {
            isFinish = true;
            //Debug.Log("胜利");
            UIKit.OpenPanel<UIMask>(UILevel.PopUI);
            float waitTime = levelId < 5 ? 3f : 2f;
            yield return new WaitForSeconds(waitTime);
            this.GetUtility<SaveDataUtility>().SaveLevel(levelId + 1);

            if (levelId < 5)
            {
                StartGame(levelId + 1);
                UIKit.ClosePanel<UIMask>();
            }
            else
            {
                this.SendCommand<PassLevelCommand>();
                StringEventSystem.Global.Send(GameConst.VICTORY_EVENT);
            }
        }
        else
            yield return null;
    }

    #endregion

    #region 炸弹相关

    /// <summary>
    /// 移动步数记录
    /// </summary>
    public void AddMoveNum(bool flag = true)
    {
        if (flag)
            moveNum++;
        else
            moveNum--;
    }

    /// <summary>
    /// 炸弹更新
    /// </summary>
    public bool BombUpdate(BottleCtrl bottleCtrl = null)
    {
        bool flag = false;
        foreach (var bottle in bottles)
        {
            bottle.UpdateBomb(bottleCtrl);

            if (bottle.CheckBoomFailure())
                flag = true;
        }
        return flag;
    }
    public void BombClear()
    {
        foreach(BottleCtrl bottle in bottles)
        {
            bottle.ClearBomb();
            bottle.isBomb = false;
        }
    }

    #endregion

    #region 付费道具相关

    #region Add Bottle

    /// <summary>
    /// 使用道具添加瓶子(单个或整瓶)
    /// </summary>
    /// <param name="isHalf"></param>
    public void AddBottle(bool isHalf, Action action = null)
    {
        //增加瓶子时一定会激活底部瓶子节点
        if (!BottomBottleLayoutGroup.gameObject.activeSelf)
            BottomBottleLayoutGroup.Show();

        // 半瓶道具逻辑(有容量可补充)
        if (isHalf && nowHalf != null && nowHalf.maxNum <= 4)
        {
            nowHalf.maxNum++;
            nowHalf.SetMaxBottle();
            if (nowHalf.maxNum == 4)
                nowHalf = null;
            action?.Invoke();
            return;
        }

        // 整瓶道具/开新半瓶逻辑
        if (nowBottles.Count < (topBottle.Count + bottomBottle.Count))
        {
            UseItemAddBottle();
            MoveAndAddBottle(isHalf, action);
        }
    }

    /// <summary>
    /// 增加瓶子
    /// </summary>
    private void UseItemAddBottle()
    {
        //对比上下排各激活瓶子数
        int _topAc = topBottle.Count(b => b.gameObject.activeSelf);
        int _bomAc = bottomBottle.Count(b => b.gameObject.activeSelf);
        //Debug.Log($"上排激活了{topAc}");
        //Debug.Log($"下排激活了{bomAc}");
        if (_topAc > _bomAc)
        {
            //索引刚好对应下一个要激活的瓶子
            bottomBottle[_bomAc].Show();
            nowBottles.Add(bottomBottle[_bomAc]);
            UpdateButtomLayoutSpcing();
        }
        else
        {
            topBottle[_topAc].Show();
            nowBottles.Add(topBottle[_topAc]);
            UpdapeTopLayoutSpcing();
        }

        //修改瓶子ID
        int _idx = 0;
        foreach (var b in topBottle.Where(b => b.gameObject.activeSelf))
            b.bottleIdx = _idx++;
        foreach (var b in bottomBottle.Where(b => b.gameObject.activeSelf))
            b.bottleIdx = _idx++;
    }

    /// <summary>
    /// 添加瓶子并初始化瓶子数据
    /// </summary>
    /// <param name="isHalf"></param>
    /// <param name="action"><扣除道具的回调/param>
    private void MoveAndAddBottle(bool isHalf, Action action)
    {
        var _newBottle = nowBottles.Last();

        if (isHalf)
        {
            _newBottle.Init(emptyBottle, _newBottle.bottleIdx);
            _newBottle.maxNum = 1;
            nowHalf = _newBottle;
        }
        else
        {
            _newBottle.Init(emptyBottle, _newBottle.bottleIdx);
            _newBottle.maxNum = 4;
        }
        action?.Invoke();
        _newBottle.SetMaxBottle();

        //对瓶子列表重新排序
        nowBottles = nowBottles.OrderBy(bottle => bottle.bottleIdx).ToList();
    }

    private const float SPACING_UNIT = -170f;
    /// <summary>
    /// 更新上方瓶子布局间距
    /// </summary>
    private void UpdapeTopLayoutSpcing()
    {
        if (TopBottleLayoutGroup == null) return;

        int activeCount = GetActiveChildCount(TopBottleLayoutGroup.transform);
        TopBottleLayoutGroup.spacing = (8 - activeCount) * SPACING_UNIT;
    }

    /// <summary>
    /// 更新下方瓶子布局间距
    /// </summary>
    private void UpdateButtomLayoutSpcing()
    {
        if (BottomBottleLayoutGroup == null) return;

        int activeCount = GetActiveChildCount(BottomBottleLayoutGroup.transform);
        BottomBottleLayoutGroup.spacing = (8 - activeCount) * SPACING_UNIT;
    }

    /// <summary>
    /// 获取激活的子物体数量
    /// </summary>
    private int GetActiveChildCount(Transform parent)
    {
        int count = 0;
        foreach (Transform child in parent)
        {
            if (child.gameObject.activeSelf)
            {
                count++;
            }
        }
        return count;
    }

    #endregion

    #region 返回上一步道具相关

    /// <summary>
    /// 记录所有瓶子
    /// </summary>
    public void RecordLast()
    {
        LevelManagerRecord record = new LevelManagerRecord();
        record.clearList = new List<int>(clearList);
        record.hideColor = new List<int>(hideColor);

        LevelManagerRecords.Add(record);

        nowBottles.ForEach(bottle => bottle.RecordLast());
    }

    /// <summary>
    /// 返回上一步
    /// </summary>
    /// <returns>是否能回退</returns>
    public bool ReturnLast()
    {
        bool ret = false;
        moveNum--;
        foreach (var bottle in nowBottles)
        {
            var needRet = bottle.ReturnLast();
            ret = ret || needRet;
        }

        if (ret)
        {
            var record = LevelManagerRecords.LastOrDefault();
            clearList = record.clearList;
            hideColor = record.hideColor;
            LevelManagerRecords.Remove(record);
        }
        else
            moveNum++;
        return ret;
    }

    #endregion

    #region 清除所有负面状态相关(魔法阵/魔法棒道具)

    /// <summary>
    /// 清除所有附加道具
    /// </summary>
    /// <param name="action">主动道具调用可传入委托</param>
    public void RemoveAll(Action action = null)
    {
        nowBottles.ForEach(bottle => bottle.SetNormal());
        cantChangeColorList.Clear();

        action?.Invoke();
    }

    /// <summary>
    /// 判断是否有阻碍效果
    /// </summary>
    /// <returns></returns>
    public bool CheckAllDebuff()
    {
        //是否有黑水
        if (hideBottleList.Count != 0)
        {
            return true;
        }

        foreach (var bottle in nowBottles)
        {
            //是否有阻碍效果
            if (bottle.isFreeze || bottle.limitColor != 0 || bottle.isNearHide || bottle.isClearHide)
            {
                return true;
            }

            foreach (var item in bottle.waterItems)
            {
                //是否有冰冻，荆棘等
                if (item != WaterItem.None)
                {
                    return true;
                }
            }
        }
        return false;
    }

    #endregion

    /// <summary>
    /// 移除所有黑水
    /// </summary>
    /// <param name="num"></param>
    /// <param name="action">使用道具回调</param>
    public void RemoveHide(Action action = null)
    {
        nowBottles.ForEach(bottle => bottle.RemovHide());
        action?.Invoke();
    }

    #endregion

    #region 其他

    /// <summary>
    /// 获取冰块瓶
    /// </summary>
    /// <returns></returns>
    public BottleWaterCtrl BreakIce()
    {
        int iceIdx = UnityEngine.Random.Range(0, iceBottles.Count);

        var bottle = iceBottles[iceIdx];
        iceBottles.RemoveAt(iceIdx);
        return bottle.FindIceWater();
    }

    #endregion

    #region 道具选择 后续可删除

    /// <summary>
    /// 道具选择(替换背景，更换瓶子材质)
    /// </summary>
    public void ShowItemSelect()
    {
        hideBg.SetActive(true);
        for (int i = 0; i < nowBottles.Count; i++)
        {
            nowBottles[i].ShowItemSelect();
        }
    }

    /// <summary>
    /// 取消道具选择
    /// </summary>
    public void HideItemSelect()
    {
        hideBg.SetActive(false);
        for (int i = 0; i < nowBottles.Count; i++)
        {
            nowBottles[i].HideItemSelect();
        }
    }

    #endregion
}

[Serializable]
public class BottleRecord
{
    public bool isFinish, isFreeze, isClearHide, isNearHide, isFlyBomb;
    public int limitColor;
    public List<int> waters = new List<int>();
    public List<bool> hideWaters = new List<bool>();
    public List<WaterItem> waterItems = new List<WaterItem>();
    public List<int> bombCount = new List<int>();
}

[Serializable]
public class LevelManagerRecord
{
    public List<int> clearList;
    public List<int> hideColor;
    public List<ChangePair> changeList;
}

[Serializable]
public class WaterSpriteInfo
{
    //颜色编号
    public int color;
    //水柱头
    public Sprite waterTopSp;
    //水柱
    public Sprite waterSp;
}