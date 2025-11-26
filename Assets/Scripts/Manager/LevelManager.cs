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

    public LevelCreateCtrl.BottleProperty emptyBottle = new();
    public Transform gameCanvas;
    public List<GameObject> createFx = new List<GameObject>();
    public LevelCreateCtrl nowLevel;
    public Color ItemColor;

    public int levelId = 1, bombMaxNum, countDownNum, playingHideAnimCount;
    public bool ISPlayingHideAnim => playingHideAnimCount == 0;

    public int moveNum = 0;

    //机制道具Spine合成生成的实例父节点(用于将渲染置顶)
    public Transform mSpineIniPar;
    public GameObject broomBullet;
    public SkeletonGraphic mahoujinSpine;
    bool isFinish = false;
    public bool isPlayAnim, isPlayFxAnim;
    // 表示关卡彩色水瓶子是否添加
    public bool isFlashWaterBottleAdded = true;

    public BottleCtrl nowHalf;

    //携带的道具
    public List<int> takeItem = new List<int>();
    public List<LevelManagerRecord> LevelManagerRecords = new List<LevelManagerRecord>();

    [SerializeField] private HorizontalLayoutGroup BottomBottleLayoutGroup;
    [SerializeField] private HorizontalLayoutGroup TopBottleLayoutGroup;
    [SerializeField] private List<BottleCtrl> bottles = new List<BottleCtrl>();

    private LevelManagerUtility levelManagerUtility;
    private ResLoader mResLoader = ResLoader.Allocate();
    [HideInInspector]
    public TMP_FontAsset redFont;
    [HideInInspector]
    public TMP_FontAsset blueFont;
    [HideInInspector]
    public TMP_FontAsset greenFont;


    #region 新机制记录存储结构
    public Dictionary<BottleCtrl, int> bubbleDict = new();
    public List<BottleCtrl> bombList = new();
    public GlobalMechanism globalMechanism;
    public int GlobalMechanismContinueSetps;
    public int GlobalMechanismBeginSetp;
    public bool haveBooming = false;
    #endregion


    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }

    private void Awake()
    {
        GameMainArc.InitArchitecture();
        Instance = this;

        levelManagerUtility = this.GetUtility<LevelManagerUtility>();
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
            StartGame(levelId);
            UIKit.OpenPanel<UIGameNode>();
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
        foreach (var i in nowBottles)
            i.DisInit();
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

        iceBottles.Clear();
        levelId = id;
        LevelCreateCtrl levelInfo = levels[levelId - 1];

        nowLevel = levelInfo;
        countDownNum = levelInfo.countDownNum;
        moveNum = 0;
        clearList = new List<int>(levelInfo.clearList);
        hideColor = new List<int>(levelInfo.hideList);
        globalMechanism = levelInfo.globalMechanism;
        GlobalMechanismBeginSetp = levelInfo.GlobalMechanismBeginSetp;
        GlobalMechanismContinueSetps = levelInfo.GlobalMechanismContinueSetps;

        int _i = 0;


        nowBottles.Clear();
        bubbleDict.Clear();
        bombList.Clear();
        nowHalf = null;

        TopBottleLayoutGroup.Show();
        BottomBottleLayoutGroup.Show();
        InitLevels(levelInfo);


        //新机制初始化

        //泡沫
        foreach (var i in nowBottles)
        {
            for (int j = 0; j < i.waterItems.Count; j++)
            {
                // 泡沐
                if (i.waterItems[j] == WaterItem.Bubble_Origin)
                {
                    bubbleDict.Add(i, levelInfo.bubbleCount[_i++]);
                }
                // 炸弹
                if (i.waterItems[j] == WaterItem.Bomb || i.waterItems[j] == WaterItem.FlyBomb)
                {
                    if (!bombList.Contains(i))
                        bombList.Add(i);
                }
            }

        }
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

            // 道具使用引导(只保留去黑和魔法棒)
            //case (int)GameDefine.UIGuideLevel.UIGuideLevelStepBack:
            //    UIKit.OpenPanel<UIPaidItemsGuide>(UILevel.PopUI, new UIPaidItemsGuideData()
            //    {
            //        PropType = NormalRewardsType.StepBack,
            //    });
            //    break;

            case (int)GameDefine.UIGuideLevel.UIGuideLevelRemoveHide:
                UIKit.OpenPanel<UIPaidItemsGuide>(UILevel.PopUI, new UIPaidItemsGuideData()
                {
                    PropType = NormalRewardsType.RemoveHide,
                });
                break;

            //case (int)GameDefine.UIGuideLevel.UIGuideLevelAddBottle:
            //    UIKit.OpenPanel<UIPaidItemsGuide>(UILevel.PopUI, new UIPaidItemsGuideData()
            //    {
            //        PropType = NormalRewardsType.AddOneBottle,
            //    });
            //    break;

            //case (int)GameDefine.UIGuideLevel.UIGuideLevelHalfBottle:
            //    UIKit.OpenPanel<UIPaidItemsGuide>(UILevel.PopUI, new UIPaidItemsGuideData()
            //    {
            //        PropType = NormalRewardsType.AddHalfBottle,
            //    });
            //    break;

            case (int)GameDefine.UIGuideLevel.UIGuideLevelRemoveAll:
                UIKit.OpenPanel<UIPaidItemsGuide>(UILevel.PopUI, new UIPaidItemsGuideData()
                {
                    PropType = NormalRewardsType.RemoveAll,
                });
                break;
        }

        // 新机制引导
        if (GameDefine.GameConst.GameplayTutorialInfo.TryGetValue(levelId,
        out var info))
        {
            RectTransform GetNode(int idx) =>
                idx >= 0 ? bottles[idx].mGuideNode : null;

            UIKit.OpenPanel<UIGuideAnimPop>(UILevel.PopUI, new UIGuideAnimPopData
            {
                GuideText = info.guideInfo,
                Node1 = GetNode(info.bottleIndex_1),
                Node2 = GetNode(info.bottleIndex_2),
            });
        }
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

            //黑水炸弹
            case ItemType.BombBlackWater:
                RandomHalfBlackWater();
                break;

        }
    }
    /// <summary>
    /// 黑水炸弹随机生成一半水数量的黑水
    /// </summary>
    public void RandomHalfBlackWater()
    {
        for (int i = 0; i < clearList.Count * 2; i++)
        {
            BottleCtrl a = levelManagerUtility.RandomBarkWaterBottle(nowBottles);
            // 提早结束黑水生成，因为已经没有可以生成的位置了
            if (a == null)
                break;
        }
        foreach (var i in nowBottles)
            i.SetBottleColor();
        UIKit.ClosePanel<UIMask>();

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
        if (clearList.Contains(color))
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
    public void FinishClear()
    {
        StopCoroutine(WaitCheckFinish());
        StartCoroutine(WaitCheckFinish());
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
    public IEnumerator WaitCheckFinish()
    {
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

    /// <summary>
    /// 检测倒水后是否死局
    /// </summary>
    /// <returns>Ture is dead</returns>
    public bool CheckDeadAfterPour()
    {
        //能否倒水或接水（到处或接收的颜色）
        List<(BottleCtrl outBottle, int outColor)> outBottles = new();
        List<(BottleCtrl inBottle, int inColor)> inBottles = new();

        foreach (var bottle in nowBottles)
        {
            if (bottle.isFinish || bottle.isClearHide || bottle.isNearHide)
                continue;

            //限制瓶特判
            if (bottle.limitColor > 0)
            {
                //限制瓶满了是isFinish状态,所以不管限制瓶有没有水，就当做是接水瓶处理
                inBottles.Add((bottle, bottle.limitColor));

                //限制瓶非空瓶,且水块不是冰块状态，说明能倒水(倒出的水一定是限制颜色)
                if (bottle.topIdx >= 0 && bottle.waterItems[bottle.topIdx] != WaterItem.Ice)
                    outBottles.Add((bottle, bottle.limitColor));
                continue;
            }

            //空瓶
            if (bottle.topIdx < 0)
                return false;

            //非空瓶
            int color = bottle.GetMoveOutTop();
            //能倒水记录
            if (!bottle.isFreeze && bottle.waterItems[bottle.topIdx] != WaterItem.Ice)
                outBottles.Add((bottle, color));
            //能接水记录
            if (bottle.waters.Count != bottle.maxNum)
                inBottles.Add((bottle, color));
        }

        // 死局检测
        foreach ((BottleCtrl outBottle, int outColor) in outBottles)
        {
            //Debug测试
            /*foreach ((BottleCtrl inBottle, int inColor) in inBottles)
            {
                if (inBottle != outBottle && inColor == outColor)
                {
                    Debug.Log($"能倒水颜色:{outColor}");
                    Debug.Log($"倒水瓶:{outBottle}");
                    Debug.Log($"接水瓶:{inBottle}");
                    return false;
                }
            }*/

            if (inBottles.Any(inB => inB.inBottle != outBottle && inB.inColor == outColor))
                return false;
        }
        return true;
    }

    #endregion

    #region 关卡机制相关

    #region 泡沐

    /// <summary>
    /// 清理消失泡沐
    /// </summary>
    /// <param name="bottleCtrl"></param>
    public void DeleteBubble(BottleCtrl bottleCtrl)
    {
        bubbleDict.Remove(bottleCtrl);
    }

    /// <summary>
    /// 生成泡沐前调用,
    /// </summary>
    public void CheckBubbleDict()
    {
        var keysToRemove = bubbleDict.Where(i => i.Value < moveNum)
                                    .Select(i => i.Key)
                                    .ToList();

        foreach (var key in keysToRemove)
        {
            bubbleDict.Remove(key);
        }
    }

    public void CreateBubble()
    {
        for (int i = 0; i < bubbleDict.Count; i++)
        {
            // 失败表示没有可生成的位置
            if (levelManagerUtility.RandomBubleWaterBottle(nowBottles) == false)
                break;
        }
        return;
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
        #region 依赖步数的全局机制

        #region 魔法猫
        if (globalMechanism == GlobalMechanism.BlackMagicCar)
        {
            if (moveNum >= GlobalMechanismBeginSetp && moveNum <= GlobalMechanismBeginSetp + GlobalMechanismContinueSetps)
            {
                BottleCtrl _bottleCtrl = levelManagerUtility.RandomBarkWaterBottle(nowBottles);
                if (_bottleCtrl != null)
                {
                    _bottleCtrl.SetHideShow(true);
                    StringEventSystem.Global.Send("MagicCatEven");
                }
            }

        }
        else if (globalMechanism == GlobalMechanism.WhiteMagicCar)
        {
            if (moveNum >= GlobalMechanismBeginSetp && moveNum <= GlobalMechanismBeginSetp + GlobalMechanismContinueSetps)
            {
                BottleCtrl _bottleCtrl = levelManagerUtility.RandomRomveBarkWaterBottle(nowBottles);
                if (_bottleCtrl != null)
                {
                    _bottleCtrl.SetHideShow(true);
                    StringEventSystem.Global.Send("MagicCatEven");
                }

            }
        }

        #endregion

        #endregion

    }
    public bool CheckBomb()
    {
        foreach (var item in bombList.ToList())
        {
            if (item.CheckBoomFailure())
            {
                BombClear();
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 炸弹表现更新
    /// </summary>
    public void BombUIUpdate()
    {
        Debug.Log(bombList.Count);
        foreach (var item in bombList.ToList())
        {
            item.UpdateBomb();
        }



    }
    /// <summary>
    /// 清空游戏中的炸弹
    /// </summary>
    public void BombClear()
    {
        foreach (var item in bombList.ToList())
        {
            item.ClearBomb();
            /* item.Key.textItem.text = (item.Value - moveNum).ToString();
             item.Key.bombCtrl.SetBomb(aniType: "bomp_remove");*/
            bombList.Remove(item);
        }
    }
    #endregion

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
        //原布局
        //TopBottleLayoutGroup.spacing = (8 - activeCount) * SPACING_UNIT;

        //新布局 
        TopBottleLayoutGroup.spacing = -25f - (8 - activeCount) * 100f;
    }

    /// <summary>
    /// 更新下方瓶子布局间距
    /// </summary>
    private void UpdateButtomLayoutSpcing()
    {
        if (BottomBottleLayoutGroup == null) return;

        int activeCount = GetActiveChildCount(BottomBottleLayoutGroup.transform);
        //BottomBottleLayoutGroup.spacing = (8 - activeCount) * SPACING_UNIT;

        BottomBottleLayoutGroup.spacing = -25f - (8 - activeCount) * 100f;
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
        record.bubbleDict = new(bubbleDict);
        record.bombList = new(bombList);
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
        //避免回退重复添加
        cantChangeColorList.Clear();
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
            bubbleDict = record.bubbleDict;
            bombList = record.bombList;
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
}

[Serializable]
public class BottleRecord
{
    public bool isFinish, isFreeze, isClearHide, isNearHide;
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
    public Dictionary<BottleCtrl, int> bubbleDict;
    public List<BottleCtrl> bombList;
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