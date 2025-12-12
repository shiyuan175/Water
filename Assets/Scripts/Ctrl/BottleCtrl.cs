using DG.Tweening;
using GameAttributes;
using GameDefine;
using QFramework;
using QFramework.Example;
using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static LevelCreateCtrl;


public class BottleCtrl : MonoBehaviour, IController, ICanSendEvent
{
    //  已完成、藤曼底座、遮挡布、陶瓷瓶、播放动画状态
    public bool isFinish, isFreeze, isClearHide, isNearHide, isPlayAnim;

    //  播放去除动画、正在解锁
    [SerializeField] private bool isClearHideAnim, hasUnlockHidePlayed;

    // 区分上下排
    public bool isUp;

    // Mes和hide的优先级
    public bool hidePriority;

    // 瓶子编号和最大容量
    public int bottleIdx;

    public int maxNum = 4,
        limitColor, // 限制可倒入的颜色(0表示无限制)
        unlockClear; // 解锁魔法布的颜色编号
    
    // 是否是浮空炸弹
    /*    private bool isFlyBomb = false;*/
    //瓶子中的水块标识
    public List<int> waters = new();

    //瓶中黑水类
    /*public List<bool> hideWaters = new();*/
    public List<HideWaterType> hideTypes = new();

    //瓶中的水的状态(冰块/火焰)
    public List<WaterItem> waterItems = new();

    //水块脚本持有
    public List<BottleWaterCtrl> waterImg = new();

    //原始泡沐数量 - 已移至LevelManager.cs管理，此处不再使用
    // public Dictionary<BottleCtrl, int> bubbleDict = new();
    // 瓶中泡沐是否是原始泡沐
    public bool[] IsOriginalBubble = new bool[4];

    // 炸弹步数
    public List<int> bombCounts = new();

    // 纯黑水瓶子 
    public GameObject BlackWaterGoPar;
    public List<GameObject> blackWaterGos = new();
    public bool IsBlackBottle = false;
    public List<bool> IsBlackBottles = new();
    public int curtainHight;

    // 操作记录(用于撤销功能)
    public List<BottleRecord> moveRecords = new();

    // 水面动画位置节点(水面位置)、加水位置节点
    public List<Transform> spineNode = new();
    public List<Transform> waterNode = new();

    // 机制引导的箭头点位
    public RectTransform mGuideNode;

    public Transform
        spineGo, // 倒水过程水花动画父节点(当前水面位置)
        spineGoPosition, // 专门用于计算spine位置的替代品
        modelGo, // 瓶子初始点位
        leftMovePlace, // 向该瓶子倒水时的目标位置 
        freezeGo; // 藤曼底座节点  

    public Animator bottleAnim, fillWaterGoAnim;

    public SkeletonGraphic
        spine, // 倒水过程水花动画
        finishSpine, // 完成状态动画
        freezeSpine, // 藤曼底座动画
        bubbleSpine; // 气泡特效动画

    //           水柱顶部、   水柱底部、    容量1瓶子、   容量2的瓶子、 容量3的瓶子、   容量4的瓶子 
    public Image ImgWaterTop, ImgWaterDown, ImgBottleOne, ImgBottleTwo, ImgBottleThree, ImgBottleFour;

    public SkeletonGraphic
        nearHide, // 消除遮挡布动画
        clearHide, // 消除陶瓷瓶动画 
        limitColorSpine; // 消除颜色限制动画

    // 完成状态动画对象
    public GameObject finishGo;

    // 新机制相关

    /// <summary>
    ///     索引0-4
    /// </summary>
    public CurtainCtrl curtainCtrl;

    public Button bottle;

    // 瓶子的初始属性配置
    private BottleProperty originProperty;

    // 用于接水次数计数
    // 修正（待确认）用于动画播放的锁问题
    private int ReceiveCount;

    // 当前顶部水块的索引
    /// <summary>
    ///     索引从0-3
    /// </summary>
    public int topIdx =>
        // 通过列表长度动态计算
        waters.Count - 1;

    private void Start()
    {
        bottle.onClick.AddListener(OnSelectedClick);
        //初始化瓶子位置
        this.RegisterEvent<GameStartEvent>(e => { OnCancelSelect(); }).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }

    /// <summary>
    ///     初始化
    /// </summary>
    /// <param name="property"></param>
    /// <param name="idx"></param>
    public void Init(BottleProperty property, int idx)
    {
        originProperty = property;

        ReceiveCount = 0;
        //配置初始容量       
        maxNum = property.numCake;
        if (property.numCake == 0)
            maxNum = 4;
        isFinish = property.isFinish;
        isClearHideAnim = false;
        finishGo.SetActive(isFinish);
        bubbleSpine.gameObject.SetActive(false);

        waters = new List<int>(property.waterSet);
        /*hideWaters = new List<bool>(property.isHide);*/
        waterItems = new List<WaterItem>(property.waterItem);
        bombCounts = new List<int>(property.bombCounts);
        hideTypes = new List<HideWaterType>(property.hideTypes);

        #region 瓶子标记初始化

        isClearHide = property.isClearHide;
        isNearHide = property.isNearHide;
        isFreeze = property.isFreeze;
        unlockClear = property.lockType;
        limitColor = property.limitColor;
        IsBlackBottle = property.isBlackBottle;
        curtainHight = property.CurtainHight;
        bottleIdx = idx;
        hasUnlockHidePlayed = false;
        // 不进入记录，只用于生成黑水判断
        IsBlackBottles = new List<bool>(property.BlackBottleList);

        #endregion

        // 根据瓶子最大容量动态调整IsOriginalBubble数组长度
        IsOriginalBubble = new bool[maxNum];

        nearHide.gameObject.SetActive(isNearHide);
        if (nearHide) nearHide.AnimationState.SetAnimation(0, "idle", true);

        foreach (var bottle in waterImg) bottle.waterImg.fillAmount = 1;

        LevelManager.Instance.iceBottles.RemoveAll(b => b == this);
        for (var i = 0; i < waters.Count; i++)
        {
            var color = waters[i];
            // 初始化闪亮水
            if (waters[i] == (int)ItemType.FlashWater)
                if (LevelManager.Instance.isFlashWaterBottleAdded)
                    LevelManager.Instance.isFlashWaterBottleAdded = false;

            if (curtainHight != 0 || isClearHide || isNearHide || waterItems[i] == WaterItem.Ice)
                LevelManager.Instance.cantChangeColorList.Add(color);
        }
        
        // 黑色瓶子初始化 利用标记IsBlackBottle区分初始化和游戏中回退
        if (IsBlackBottle)
        {
            BlackWaterGoPar.SetActive(true);
        }
        else
        {
            for (int i = 0; i < property.BlackBottleList.Count; i++)
            {
                if (property.BlackBottleList[i])
                {
                    blackWaterGos[i].SetActive(true);
                    BlackWaterGoPar.SetActive(true);
                    IsBlackBottle = true;
                }
            }
        }
        SetBottleColor(true);

        foreach (var item in waterItems)
            if (item == WaterItem.Ice)
                LevelManager.Instance.iceBottles.Add(this);
        //CheckFinish();

        freezeGo.gameObject.SetActive(isFreeze);
        //!isFinish针对回退时是否触发
        if (limitColor != 0 && !isFinish)
        {
            limitColorSpine.gameObject.SetActive(true);
            if (limitColor > 0 && limitColor < (int)EIdleAnim.IDLE_MAX)
                limitColorSpine.AnimationState.SetAnimation(0,
                    GameEnum.GetDescription((EIdleAnim)limitColor), false);
        }
        else
        {
            limitColorSpine.gameObject.SetActive(false);
        }


        if (isFreeze) freezeSpine.AnimationState.SetAnimation(0, "idle", false);

        for (var i = 0; i < hideTypes.Count; i++)
            if (hideTypes[i] != HideWaterType.None && !LevelManager.Instance.hideBottleList.Contains(this))
            {
                LevelManager.Instance.hideBottleList.Add(this);
                break;
            }

        if (curtainHight != 0) curtainCtrl.InitCurtain(curtainHight);

        SetMaxBottle();
    }

    /// <summary>
    ///     结束游戏的时候清空瓶子的状态，类似于真正的初始化瓶子
    /// </summary>
    public void DisInit()
    {
        foreach (var i in waterImg)
            i.textItem.text = "";

        BlackWaterGoPar.SetActive(false);
        foreach (var blackwater in blackWaterGos)
        {
            blackwater.SetActive(false);
        }
    }
    /// <summary>
    ///     设置瓶子最大装水数
    /// </summary>
    public void SetMaxBottle()
    {
        ImgBottleOne.gameObject.SetActive(maxNum == 1);
        ImgBottleTwo.gameObject.SetActive(maxNum == 2);
        ImgBottleThree.gameObject.SetActive(maxNum == 3);
        ImgBottleFour.gameObject.SetActive(maxNum == 4);
    }

    /// <summary>
    ///     设置魔法布Spine
    /// </summary>
    private void SetClearHide()
    {
        if (!isClearHideAnim)
        {
            clearHide.gameObject.SetActive(isClearHide);
            if (isClearHide)
                if (unlockClear > 0 && unlockClear < (int)EIdleAnim.IDLE_MAX)
                    clearHide.AnimationState.SetAnimation(0, GameEnum.GetDescription((EIdleAnim)unlockClear),
                        false);
        }
    }

    /// <summary>
    ///     判断黑色类水块
    /// </summary>
    /// <param name="isFirst"></param>
    public void CheckHide(bool isFirst = false)
    {
        if (hideTypes.Count > waters.Count)
            while (hideTypes.Count > waters.Count)
                hideTypes.RemoveAt(hideTypes.Count - 1);
        else if (hideTypes.Count < waters.Count)
            while (hideTypes.Count < waters.Count)
                hideTypes.Add(HideWaterType.None);

        if (!isFirst)
            if (hideTypes.Count > 0 && waters.Count > 0)
            {
                //最上层的黑水块显示
                hideTypes[waters.Count - 1] = HideWaterType.None;
                if (waterItems[waters.Count - 1] == WaterItem.Bubble_Origin ||
                    waterItems[waters.Count - 1] == WaterItem.Bubble)
                {
                    waterItems[waters.Count - 1] = WaterItem.None;
                    waterImg[waters.Count - 1].textItem.text = "";
                    waterImg[waters.Count - 1].bubbleCtrl.BubbleDead(IsOriginalBubble[waters.Count - 1]);
                    LevelManager.Instance.DeleteBubble(this);
                }

                if (waterItems[waters.Count - 1] == WaterItem.GrassBomb)
                {
                    waterItems[waters.Count - 1] = WaterItem.None;
                    waterImg[waters.Count - 1].grassWaterCtrl.Bombing();

                    LevelManager.Instance.grassList.Remove(this);
                    LevelManager.Instance.GrassBombing();
                }

                

                //黑水块的颜色与顶层是否相同(相同显示)
                for (var i = waters.Count - 1; i >= 0; i--)
                    if (topIdx >= 0 && waters[i] == waters[topIdx])
                    {
                        hideTypes[waters.Count - 1] = HideWaterType.None;
                        
                        if (waterItems[waters.Count - 1] == WaterItem.Bubble_Origin ||
                            waterItems[waters.Count - 1] == WaterItem.Bubble)
                        {
                            waterItems[i] = WaterItem.None;
                            waterImg[i].textItem.text = "";
                            waterImg[i].bubbleCtrl.BubbleDead(IsOriginalBubble[i]);
                            LevelManager.Instance.DeleteBubble(this);
                        }

                        if (waterItems[i] == WaterItem.GrassBomb)
                        {
                            waterItems[i] = WaterItem.None;
                            waterImg[i].grassWaterCtrl.Bombing();
                            LevelManager.Instance.grassList.Remove(this);
                            LevelManager.Instance.GrassBombing();
                        }
                    }
                    else
                    {
                        break;
                    }
            }

        //判断该瓶子是否还存在黑水块
        var hasHide = false;
        for (var i = 0; i < hideTypes.Count; i++)
            if (hideTypes[i] != HideWaterType.None)
            {
                hasHide = true;
                break;
            }

        if (!hasHide) LevelManager.Instance.hideBottleList.Remove(this);
    }

    /// <summary>
    ///     设置水块颜色
    /// </summary>
    /// <param name="isFirst"></param>
    /// <param name="nowaitHide"></param>
    public void SetBottleColor(bool isFirst = false, bool nowaitHide = false)
    {
        CheckHide(isFirst);

        //已完成，清除黑水块
        if (isFinish)
            for (var i = 0; i < hideTypes.Count; i++)
                hideTypes[i] = HideWaterType.None;

        // 遍历每层水块，设置颜色和状态
        for (var i = 0; i < waters.Count; i++)
        {
            // 计算颜色索引，减一是因为useColor颜色编号从 1 开始，而waterColor数组索引从 0 开始
            var useColor = waters[i] - 1;
            // 普通颜色水块
            if (useColor < 1000)
                //Debug.Log("UseColor " + useColor);
                waterImg[i].SetColorState(ItemType.UseColor, LevelManager.Instance.waterColor[useColor], topIdx == i);
            // 特殊道具水块
            else
                // 根据道具类型设置对应的显示和动画
                waterImg[i].SetColorState((ItemType)waters[i], LevelManager.Instance.ItemColor, topIdx == i, i);

            //将隐藏水块显示
            if (hideTypes.Count > 0) SetHideShow(true, i);

            waterImg[i].waterColor = useColor;
        }

        // 更新水块的显示状态
        for (var i = 0; i < waterImg.Count; i++)
            waterImg[i].gameObject.SetActive(i < waters.Count || waterImg[i].isPlayItemAnim);

        // 检查水块的道具状态 / 初始化道具spine
        CheckWaterItem();
        // 更新炸弹
        /*      UpdateBomb();*/
        // 更新魔法布遮挡状态
        SetClearHide();
        // 更新水面位置
        var spinePosIdx = topIdx + 1;
        SetNowSpinePos(spinePosIdx);
        PlaySpineWaitAnim();
    }

    /// <summary>
    ///     设置隐藏水块显示
    /// </summary>
    /// <param name="nowaitHide">是否立即触发</param>
    /// <param name="idx"></param>
    public void SetHideShow(bool nowaitHide, int idx = -1)
    {
        if (idx >= 0)
        {
            if (hideTypes.Count > 0)
                waterImg[idx].SetHide(hideTypes[idx], nowaitHide);
        }
        else
        {
            for (var i = 0; i < waters.Count; i++)
                if (hideTypes.Count > 0)
                    // waterImg[idx].SetHide(hideWaters[idx], nowaitHide);
                    waterImg[i].SetHide(hideTypes[i], nowaitHide);
        }
    }

    /// <summary>
    ///     判断水块道具 /初始化水块道具spine（setcolor里调用）清空状态调整到init实现
    /// </summary>
    public void CheckWaterItem()
    {
        for (var i = 0; i < waterItems.Count; i++)
        {
            if (!waterImg[i].isPlayItemAnim)
            {
                waterImg[i].fireRuneGo.SetActive(false);
                waterImg[i].iceGo.SetActive(false);
            }

            switch (waterItems[i])
            {
                case WaterItem.None:
                    break;
                case WaterItem.Ice:
                    waterImg[i].iceGo.SetActive(true);
                    break;
                case WaterItem.Bomb:
                    if (bombCounts[i] != BOMBREMOVE_SIGN)
                        waterImg[i].bombCtrl.SetBomb(true, (bombCounts[i] - LevelManager.Instance.moveNum).ToString(),
                            "idle");
                    break;
                case WaterItem.FlyBomb:
                    if (bombCounts[i] != BOMBREMOVE_SIGN)
                        waterImg[i].bombCtrl.SetBomb(true, (bombCounts[i] - LevelManager.Instance.moveNum).ToString(),
                            "idle", true);
                    break;
                case WaterItem.Bubble_Origin:
                    IsOriginalBubble[i] = true;
                    goto case WaterItem.Bubble;
                case WaterItem.Bubble:
                    waterImg[i].bubbleCtrl.BubbleAppend(waterItems[i] == WaterItem.Bubble_Origin);
                    break;

                case WaterItem.GrassBomb:
                    waterImg[i].grassWaterCtrl.BombApeend();
                    break;
                    
                case WaterItem.BreakIce:
                    waterImg[i].fireRuneGo.SetActive(true);
                    if (waters[i] > 0 && waters[i] < (int)EIdleAnim.IDLE_MAX)
                        waterImg[i].fireRuneSpine.AnimationState.SetAnimation(0,
                            GameEnum.GetDescription((EIdleAnim)waters[i]), false);

                    break;
            }
        }
    }

    /// <summary>
    ///     入场动画(水面Spine动画)
    /// </summary>
    public void PlaySpineWaitAnim()
    {
        //空瓶
        if (topIdx < 0)
        {
            spineGo.Hide();
            return;
        }

        var spineAnimName = "";

        //两种方式
        //1、找到道具下的普通水作为水面
        //2、顶部水为道具直接隐藏水面(不适用、因为水面只有一个。当最上层是普通水，道具底下的水面无法显示。仅当道具作为最上层水块时才有效果)

        //方案1
        var color = waters[topIdx];
        WaterAttrCache.Dict.TryGetValue((ItemType)color, out var _attr);
        if (color < 1000 || _attr is RainBowWaterState)
        {
            spineAnimName = GameEnum.GetDescription((ERuChangAnim)color);
            //黑色水面特判
            if (topIdx >= 0 && topIdx < hideTypes.Count && hideTypes[topIdx] != HideWaterType.None)
                spine.AnimationState.SetAnimation(0, "ruchanghuangdong_mask", false);

            spineGo.Show();
            if (!string.IsNullOrEmpty(spineAnimName))
                spine.AnimationState.SetAnimation(0, spineAnimName, false);
        }
        else
        {
            spineGo.Hide();
            //spine.AnimationState.SetAnimation(0, "ruchanghuangdong_mask", false);
        }

        //Debug.Log($"水面动画名:{spineAnimName},瓶子：{this.gameObject.name}");

        CheckHide();


        /*
        //方案2
        //获取可作为水面的颜色
        int spinePosIdx = -1;

        for (int i = topIdx; i >= 0; i--)
        {
            WaterAttrCache.Dict.TryGetValue((ItemType)waters[i], out WaterColorState _attr);
            if (waters[i] < 1000 || _attr?.SpineType is EColorStateSpineType.ERainBowWater)
            {
                spinePosIdx = i;
                break;
            }
        }

        //瓶内只有道具(无可作为水面水块)
        if (spinePosIdx == -1)
        {
            spineGo.Hide();
            return;
        }
        var color = waters[spinePosIdx];

        spineAnimName = GameEnum.GetDescription<ERuChangAnim>((ERuChangAnim)color);
        spineGo.Show();
        spine.AnimationState.SetAnimation(0, spineAnimName, false);


        Debug.Log($"水面动画名:{spineAnimName},瓶子：{this.gameObject.name}");
        CheckHide();
        //黑色水面特判
        if (topIdx >= 0 && spinePosIdx < hideWaters.Count && hideWaters[spinePosIdx])
        {
            spine.AnimationState.SetAnimation(0, "ruchanghuangdong_mask", false);
        }*/
    }

    /// <summary>
    ///     设置水面位置
    /// </summary>
    public void SetNowSpinePos(int node)
    {
        var useNode = node;
        //Debug.Log("当前节点 " + node);
        if (useNode - 1 < waters.Count)
            for (var i = node - 1; i >= 0; i--)
            {
                var _type = (ItemType)waters[i];
                WaterAttrCache.Dict.TryGetValue(_type, out var _attr);

                if (waters[i] < 1000 || _attr is RainBowWaterState)
                {
                    useNode = i + 1;
                    break;
                }
            }

        spineGoPosition.localPosition = spineNode[useNode].localPosition;
        //spineGoPosition.localPosition = spineNode[waters.Count].localPosition;
    }

    /// <summary>
    ///     检查触发哪种机制(合成)道具
    /// </summary>
    public void CheckItem()
    {
        //记录本次触发的所有道具类型
        List<int> _items = new();
        //是否有道具消除标记
        var _hasPair = false;

        //记录上一个检测到的道具ID
        var _itemId = 0;
        //记录上一个道具所在的层(索引)
        var _itemPlace = 0;
        // 遍历瓶子中的水，判断是否有相邻道具
        for (var i = 0; i < waters.Count; i++)
        {
            var _waterColor = waters[i];

            var _type = (ItemType)_waterColor;
            WaterAttrCache.Dict.TryGetValue(_type, out var _attr);

            // 普通水/特殊水/非可合成类道具
            if (_waterColor <= 1000 || _attr is RainBowWaterState)
            {
                _itemId = 0;
                continue;
            }

            // 没有记录过道具，则记录当前位置与ID
            if (_itemId == 0)
            {
                _itemId = _waterColor;
                _itemPlace = i;
                continue;
            }

            // 只处理相同类型且相邻的道具(i - itemPlace != 1 冗余条件)
            if (_waterColor != _itemId || i - _itemPlace != 1)
            {
                _itemId = _waterColor;
                _itemPlace = i;
                continue;
            }

            // 相邻且同类的道具
            _hasPair = true;
            _items.Add(_waterColor);
            switch (_type)
            {
                case ItemType.ClearRandomWaterItem:
                    waterImg[i - 1].PlayUseBroom(waterImg[i]);
                    break;

                case ItemType.MakeColorItem:
                    waterImg[i - 1].PlayUseCreate(this, waterImg[i]);
                    waterImg[i].color = new Color(1, 1, 1, 0);
                    waterImg[i].broomItemGo.SetActive(false);
                    waterImg[i].createItemGo.SetActive(false);
                    waterImg[i].changeItemGo.SetActive(false);
                    waterImg[i].magnetItemGo.SetActive(false);
                    break;

                case ItemType.MagnetItem:
                    StringEventSystem.Global.Send("CacheMagnetWater", waterImg[i - 1]);
                    waterImg[i - 1].PlayUseMagnet(waterImg[i]);
                    break;
                case ItemType.BombBlackWater:
                    waterImg[i - 1].PlayUseItem(waterImg[i], _type);
                    break;
                default:

                    if (_attr is ChangeColorItemState)
                        waterImg[i - 1].PlayUseChange(waterImg[i]);
                    else if (_attr is ClearItemState)
                        waterImg[i - 1].PlayUseBroom(waterImg[i]);
                    //else
                    //    Debug.Log("其他机制道具");
                    break;
            }

            // 清除被使用的道具,并清空标记
            waters[_itemPlace] = 0;
            waters[i] = 0;
            /* waters[_itemPlace] = 0;*/
            _itemId = 0;
        }

        // 清除道具所在的水块(重新排列水块)
        if (_hasPair)
        {
            RemoveItem();
            for (var i = 0; i < _items.Count; i++)
            {
                var useItem = _items[i];
                LevelManager.Instance.UseItem(useItem, waterImg[_itemPlace].transform);
            }

            CheckHide();
            SetHideShow(false);
        }

        /*  标记
        //spineGo.gameObject.SetActive(topIdx >= 0);
        //机制道具生效不需要更新水面，接收水的地方会更新
        //PlaySpineWaitAnim();

        //合并到_hasPair触发
        //if (_items.Count > 0)
        //{
        //    CheckHide();
        //    SetHideShow(false);
        //}
        */
    }

    /// <summary>
    ///     判断瓶子是否有负面状态
    /// </summary>
    /// <returns></returns>
    public bool CheckDebuff()
    {
        if (isClearHide || isFreeze || isNearHide || limitColor != 0)
            //Debug.Log("1、" + (isClearHide || isFreeze || isNearHide || limitColor != 0));
            return true;

        foreach (var item in waterItems)
            //除火焰外都算负面
            if (item != WaterItem.None && item != WaterItem.BreakIce)
                //Debug.Log("2、" + (item != WaterItem.None && item != WaterItem.BreakIce));
                return true;

        foreach (var item in hideTypes)
            if (item != HideWaterType.None)
                //Debug.Log("3、黑水" + (item == true));
                return true;

        return false;
    }

    #region 炸弹标记数值

    private const int FLYBOMBING_SIGN = 400;
    private const int BOMBING_SIGN = 300;
    private const int BOMBREMOVE_SIGN = 100;
    private const int FLYBOMREMOVE_SIGN = 200;
    private const int NULLBOMB_SIGN = 0;

    #endregion

    #region 倒水相关

    #region 瓶子点击事件/选中/取消选中

    /// <summary>
    ///     瓶子点击事件
    /// </summary>
    private void OnSelectedClick()
    {
        if (!isPlayAnim && !LevelManager.Instance.isPlayFxAnim) GameCtrl.Instance.OnSelect(this);
    }

    /// <summary>
    ///     判断是否能选中 如果能 则选中
    /// </summary>
    /// <returns></returns>
    public bool OnSelect(bool needUp)
    {
        // 无法选中状态：魔法布、陶瓷瓶、冰冻、魔法布动画播放时、满帘子瓶子、？倒水锁？待确认
        if ((isFreeze && needUp) || isClearHide || isNearHide || isFinish || isClearHideAnim || ReceiveCount != 0 ||
            curtainHight == 4)
            return false;
        if (needUp)
            modelGo.transform.DOLocalMoveY(modelGo.transform.localPosition.y + 100f, 2f / 30f);
        if (IsBlackBottle)
            BlackWaterGoPar.SetActive(false);
        return true;
    }

    /// <summary>
    ///     取消选中
    /// </summary>
    public void OnCancelSelect()
    {
        modelGo.transform.DOLocalMove(Vector3.zero, 2f / 30f);
        if (IsBlackBottle)
            BlackWaterGoPar.SetActive(true);
    }

    #endregion

    /// <summary>
    ///     判断能否倒出
    /// </summary>
    /// <returns></returns>
    public bool CheckMoveOut()
    {
        // 不能倒水情况，瓶子错误的，顶部水是冰块，顶部水没有高于帘子
        if (topIdx < 0 || waterItems[topIdx] == WaterItem.Ice || topIdx <= curtainHight - 1)
            return false;

        return true;
    }

    /// <summary>
    ///     判断能否倒入
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public bool CheckMoveIn(int color)
    {
        if (topIdx < 0 && limitColor == 0 && !isClearHide && curtainHight == 0)
            return true;

        var top = GetMoveOutTop();

        if (isClearHide || isNearHide || isFinish || GetLeftEmpty() == 0 || (limitColor != 0 && limitColor != color) ||
            topIdx < curtainHight - 1)
            return false;

        //color非道具
        if (color < 1000)
        {
            if (color != top && top != 0) return false; //color == top 且 top不为空
        }
        else
        {
            //判断自身顶部是否为道具 
            if (top > 1000) return top == color; //相同道具才可放置
            return false; //color是道具，top不是道具
        }

        return true;
    }

    /// <summary>
    ///     取得倒出水的颜色
    /// </summary>
    /// <returns></returns>
    public int GetMoveOutTop()
    {
        if (topIdx < 0) return 0;

        return waters[topIdx];
    }

    /// <summary>
    ///     获得剩余空位
    /// </summary>
    /// <returns></returns>
    public int GetLeftEmpty()
    {
        return maxNum - 1 - topIdx;
    }

    /// <summary>
    ///     倒水到另一个瓶子
    /// </summary>
    /// <param name="other"></param>
    public void MoveTo(BottleCtrl other)
    {
        var moveNum = other.GetLeftEmpty();
        var sameNum = 1;

        for (var i = topIdx - 1; i >= 0; i--)
        {
            if (waters[i] == GetMoveOutTop() && waterItems[i] != WaterItem.Ice && i > curtainHight - 1)
                sameNum++;
            else
                break;
        }

        if (moveNum > sameNum)
            moveNum = sameNum;

        var color = GetMoveOutTop();
        MoveToOtherAnim(other, topIdx, moveNum, color);
        PlayOutAnim(moveNum, topIdx, color);
        for (var i = 0; i < moveNum; i++)
        {
            var idx = topIdx;
            // 将炸弹的计时一起传送
            var bombCount = bombCounts.Count > idx ? bombCounts[idx] : 0;

            WaterItem _waterItem;
            if (topIdx < 0) _waterItem = WaterItem.None;
            else _waterItem = waterItems[topIdx];

            //Debug.Log($"瓶子：{this.name} 倒出水：{color}");
            if (waters.Count > 0)
            {
                waterImg[idx].wenhaoFxGo.SetActive(false);
                waterImg[idx].HideGo.SetActive(false);
                waters.RemoveAt(idx);
                waterItems.RemoveAt(idx);
                /*hideWaters.RemoveAt(idx);*/
                hideTypes.RemoveAt(idx);
                // 炸弹为空的时候，直接不进行移动
                if (bombCounts.Count > idx)
                    bombCounts.RemoveAt(idx);

                other.ReceiveWater(color, _waterItem, bombCount);
            }

            GameCtrl.Instance.control = false;
        }

        #region 倒水后触发、玩家走一步触发、倒水后全局机制

        // 瓶子检查
        other.CheckFinish();        
        // 炸弹更新
        if (!LevelManager.Instance.bombList.Contains(other)) LevelManager.Instance.bombList.Add(other);

        LevelManager.Instance.BombUIUpdate();

        // 泡沐机制--生成检查 
        LevelManager.Instance.CheckBubbleDict();
        LevelManager.Instance.CreateBubble();

        // 魔法猫机制检查 后面可以改为全局机制检查，且不冲突，减少重复的入口判断。是否会有全局机制同时生效
        if (LevelManager.Instance.globalMechanism != GlobalMechanism.None)
            LevelManager.Instance.CheckMagicCat();


        //死局判定
        if (LevelManager.Instance.CheckDeadAfterPour())
            UIKit.OpenPanel<UIRetry>();

        #endregion

        //OnCancelSelect();
        other.PlayFillAnim(moveNum, color);
    }

    /// <summary>
    /// 瓶子倒水动画
    /// </summary>
    /// <param name="other"></param>
    /// <param name="topIndex"></param>
    /// <param name="numWater"></param>
    /// <param name="useColor"></param>
    public void MoveToOtherAnim(BottleCtrl other, int topIndex, int numWater, int useColor)
    {
        UnityEngine.UI.Image bottleClickMask = bottle.GetComponent<Image>();

        //获取移动终点
        var (_targetPos, _dir) = GetMoveToPos(transform, other.transform, other.leftMovePlace);

        isPlayAnim = true;
        var bottleRenderUpdate = bottleAnim.GetComponent<BottleRenderUpdate>();
        //水柱相关
        bottleRenderUpdate.SetMoveBottleRenderState(true, other);

        ItemType _type = (ItemType)useColor;
        WaterAttrCache.Dict.TryGetValue(_type, out var _attr);

        //普通水/特殊水块
        if (useColor < 1000 || _attr is RainBowWaterState)
        {
            topIndex += 1;
            //瓶身倾斜动画(BottleIn开始的Z轴大小要以BottleOut的结束为起点)
            //移动时长0.3f = 18帧.在19帧开始二次倾斜瓶子(19帧会播放水柱的动画)
            string bottleAnimName = $"BottleOut{topIndex}_{topIndex - numWater}{_dir}";
            bottleAnim.Play(bottleAnimName);
            //Debug.Log(bottleAnimName);
        }
        else
        {
            bottleAnim.Play($"BottleItemOut{_dir}");
        }

        //移动到目标点位(时长为18帧,19帧处开始二次倾斜)
        bottleClickMask.raycastTarget = false;
        modelGo.transform.DOMove(_targetPos, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            SetDownWaterSp(useColor);
            //普通水/特殊水块
            if (useColor < 1000 || _attr?.SpineType is EColorStateSpineType.ERainBowWater ||
                _attr?.SpineType is EColorStateSpineType.EFlashWater)
            {
                fillWaterGoAnim.Play("FillWater");
                // 因为先Play了动画片段在做的DoTween
                // 等待动画结束时机 = 动画实际播放时长(总帧/60帧s) * Exit Time - 0.3f(上方移动动画时长)
                // 方案一:修改动画总时长 = 移动时长(0.3f/18帧) + 水流时长(0.384f/23帧) 
                // 方案二:保持原55帧长度、将Exit Time调整为0.74618(以0.384f结束计算得到、缺点是二次倾斜没完全做完)    
                ActionKit.Delay(0.384f, () =>
                {
                    //标记--------瓶子移动不需要更新水面(初始化或接水的时候水面都会更新,所以倒水前不需要再次更新)
                    //SetNowSpinePos(topIndex - numWater);
                    //回归原点(瓶子摆正动画需同步0.46f 约等于27帧)
                    modelGo.transform.DOLocalMove(Vector3.zero, 0.46f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        isPlayAnim = false;
                        bottleRenderUpdate.SetMoveBottleRenderState(false);
                        bottleClickMask.raycastTarget = true;

                        OnCancelSelect();
                        //回归原点时更新(此处调用则注释CoroutinePlayOutAnim 中的调用)
                        //SetBottleColor();
                    });
                }).Start(this);
            }
            else
            {
                //采用方案1(要调快调动画长度和等待时长)
                ActionKit.Delay(0.617f, () =>
                {
                    //回归原点
                    modelGo.transform.DOLocalMove(Vector3.zero, 0.46f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        isPlayAnim = false;
                        bottleRenderUpdate.SetMoveBottleRenderState(false);
                        bottleClickMask.raycastTarget = true;
                        //SetBottleColor();
                    });
                }).Start(this);
            }

        });

    }

    /// <summary>
    /// 获取移动方向和位置
    /// </summary>
    /// <param name="thisBottle"></param>
    /// <param name="targetBottle"></param>
    /// <param name="moveToTram"></param>
    /// <returns></returns>
    private (Vector3 pos, string dir) GetMoveToPos(Transform thisBottle, Transform targetBottle, Transform moveToTram)
    {
        // 要用本地坐标取镜像在转为世界坐标
        Vector3 _targetPos = moveToTram.localPosition;
        string _dir;

        var thisParent = thisBottle.parent;
        var targetParent = targetBottle.parent;
        bool isSameRow = thisParent == targetParent;
        int targetRowActiveCount = GetActiveSiblingCount(targetParent);

        // 同一排直接比较 postion.x
        if (isSameRow)
        {
            if (thisBottle.position.x >= targetBottle.position.x)
            {
                //Debug.Log("向左移动、取镜像");
                _targetPos.x *= -1f;
                _dir = "_Left";
            }
            else
            {
                //Debug.Log("向右移动、取原值");
                _dir = "_Right";
            }
        }
        // 不同排,采用左右各一半区分向左向右
        else
        {
            int activeCount = targetRowActiveCount;
            int targetIndex = targetBottle.GetSiblingIndex();
            int mid = activeCount / 2;

            if (targetIndex < mid)
            {
                //Debug.Log("向左移动，取镜像");
                _targetPos.x *= -1f;
                _dir = "_Left";
            }
            else
            {
                //Debug.Log("向右移动，取原值");
                _dir = "_Right";
            }
        }

        return (moveToTram.parent.TransformPoint(_targetPos), _dir);
    }

    private int GetActiveSiblingCount(Transform parent)
    {
        int count = 0;
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).gameObject.activeSelf)
                count++;
        }
        return count;
    }

    /// <summary>
    /// 设置水柱颜色
    /// </summary>
    /// <param name="useColor"></param>
    public void SetDownWaterSp(int useColor)
    {
        var _waterInfo = LevelManager.Instance.waterSpriteInfos.FirstOrDefault(x => x.color == useColor);
        if (_waterInfo is not null)
        {
            ImgWaterTop.sprite = _waterInfo.waterTopSp;
            ImgWaterDown.sprite = _waterInfo.waterSp;
        }
    }

    /// <summary>
    /// 水位上升/下降效果
    /// </summary>
    /// <param name="num"></param>
    public void PlayOutAnim(int num, int useIdx, int useColor)
    {
        StartCoroutine(CoroutinePlayOutAnim(num, useIdx, useColor));
    }

    /// <summary>
    /// 倒水瓶水位变化动画
    /// </summary>
    /// <param name="num"></param>
    /// <param name="useIdx"></param>
    /// <param name="useColor"></param>
    /// <returns></returns>
    IEnumerator CoroutinePlayOutAnim(int num, int useIdx, int useColor)
    {
        float fillAlltime = 0.35f;
        yield return new WaitForSeconds(fillAlltime);
        //float fillAlltime = 1.33f;

        spineGo.gameObject.SetActive(true);
        int startIdx = useIdx;
        //标记-----SetBottleColor会调用水面位置更新
        //SetNowSpinePos(startIdx + 1);

        var _type = (ItemType)useColor;
        WaterAttrCache.Dict.TryGetValue(_type, out WaterColorState _attr);
        bool _isRainBowWater = _attr is RainBowWaterState;

        if (useColor <= 1000 || _isRainBowWater)
        {
            spineGoPosition.DOLocalMove(spineNode[useIdx + 1 - num].localPosition, fillAlltime).SetEase(Ease.Linear).OnComplete(() =>
            {
                if (topIdx < 0)
                    spineGo.gameObject.SetActive(false);
            });
        }
        else
        {
            spineGoPosition.transform.localPosition = spineNode[useIdx + 1 - num].localPosition;
            if (topIdx < 0)
                spineGo.gameObject.SetActive(false);
        }

        //标记——无需主动调用,SetBottleColor里会触发
        //PlaySpineWaitAnim(useColor);

        float fillTime = fillAlltime / num;
        if (useColor > 1000 && !_isRainBowWater) fillTime = 0f;

        for (int i = 0; i < num; i++)
        {
            waterImg[startIdx - i].waterImg.fillAmount = 1;
        }

        for (int i = 0; i < num; i++)
        {
            waterImg[startIdx - i].PlayOutAnim(fillTime);
            yield return new WaitForSeconds(fillTime);
        }

        SetBottleColor();

        //倒水过程结束
        GameCtrl.Instance.ReducePouringCount();
    }

    /// <summary>
    /// 接收水
    /// </summary>
    /// <param name="water"></param>
    /// <param name="item"></param>
    public void ReceiveWater(int water, WaterItem item, int bombCount)
    {
        //Debug.Log($"瓶子：{this.name} 接收水：{water}");
        if (water > 0)
        {
            waters.Add(water);
            waterItems.Add(item);
            bombCounts.Add(bombCount);
            hideTypes.Add(HideWaterType.None);
        }
    }

    /// <summary>
    /// 接水动画
    /// </summary>
    /// <param name="num"></param>
    public void PlayFillAnim(int num, int color)
    {
        StartCoroutine(CoroutinePlayFillAnim(num, color));
    }

    /// <summary>
    /// 接水动画协程
    /// </summary>
    /// <param name="num"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    private IEnumerator CoroutinePlayFillAnim(int num, int color)
    {
        var _hasItemEffect = BottleHasItem();
        if (_hasItemEffect) UIKit.OpenPanel<UIMask>(UILevel.PopUI);

        ++ReceiveCount;
        //等待的时长为瓶子移动的第一段时长。
        float fillAlltime = 0.3f;//0.46f;
        yield return new WaitForSeconds(fillAlltime);
        //不更新水面位置,下方做水面上升动画
        SetBottleColor(NeedUpdateWaterPos:false);

        var startIdx = topIdx + 1 - num;
        var _type = (ItemType)color;
        WaterAttrCache.Dict.TryGetValue(_type, out WaterColorState _attr);
        bool _isRainBowWater = _attr is RainBowWaterState;
        
        if (color < 1000 || _isRainBowWater)
        {
            //原先是 SetBottleColor 把水面更新到顶部了，然后又要模拟水面上升的效果
            //所以再把水面设置到接水前的高度，然后通过动画进行水面上升效果
            //现引入参数，SetBottleColor 时不更新水面位置
            spineGo.gameObject.SetActive(true);
            //SetNowSpinePos(startIdx);
            spineGoPosition.DOMove(spineNode[topIdx + 1].position, fillAlltime).SetEase(Ease.Linear);
        }
        //道具没有水面(道具水面不由Spine控制)
        //else
        //    if (startIdx >= 0) SetNowSpinePos(startIdx);

        PlaySpineAnim();

        var fillTime = fillAlltime / num;
        if (color > 1000 && !_isRainBowWater) fillTime = 0.1f;

        for (var i = 0; i < num; i++) waterImg[startIdx + i].waterImg.fillAmount = 0;

        for (var i = 0; i < num; i++)
        {
            waterImg[startIdx + i].PlayFillAnim(fillTime);
            yield return new WaitForSeconds(fillTime);
        }

        --ReceiveCount;

        //标记——判定条件生效有问题
        //有机制道具生效才执行会出现水花动画变长
        if (_hasItemEffect)
            CheckItem();
        //SetBottleColor();
        //CheckItem();
        //CheckFill();
    }

    /// <summary>
    ///     检查本次倒水瓶子中是否有局内道具生效
    /// </summary>
    /// <returns></returns>
    public bool BottleHasItem()
    {
        // 是否检测到相邻可合成的道具
        var _hasPair = false;

        // 记录上一个检测到的道具
        var _itemId = 0;
        var _itemPlace = 0;

        for (var i = 0; i < waters.Count; i++)
        {
            var _waterColor = waters[i];

            var _type = (ItemType)_waterColor;
            WaterAttrCache.Dict.TryGetValue(_type, out var _attr);

            // 普通水/特殊水/非可合成类道具
            if (_waterColor <= 1000 || _attr is RainBowWaterState)
            {
                _itemId = 0;
                continue;
            }

            // 第一次记录道具
            if (_itemId == 0)
            {
                _itemId = _waterColor;
                _itemPlace = i;
                continue;
            }

            // 检查是否与上一个相邻且同类型
            if (_waterColor == _itemId && i - _itemPlace == 1)
            {
                _hasPair = true;
                break;
            }

            // 不匹配则重置记录
            _itemId = _waterColor;
            _itemPlace = i;
        }

        return _hasPair;
    }

    /// <summary>
    ///     播接水动画(接水水花)
    /// </summary>
    public void PlaySpineAnim()
    {
        var spineAnimName = "";
        var color = GetMoveOutTop();

        WaterAttrCache.Dict.TryGetValue((ItemType)color, out var _attr);

        if ((color > 0 && color < (int)EDaoShuiAnim.IDLE_MAX)
            || _attr is RainBowWaterState)
            spineAnimName = GameEnum.GetDescription((EDaoShuiAnim)color);

        //Debug.Log("水花动画名：" + spineAnimName);
        if (!string.IsNullOrEmpty(spineAnimName))
            spine.AnimationState.SetAnimation(0, spineAnimName, false);
    }

    private void CheckFill()
    {
        for (var i = 0; i < waterImg.Count; i++)
            if (waterImg.Count > i) waterImg[i].waterImg.fillAmount = 1;
            else waterImg[i].waterImg.fillAmount = 0;
    }

    #endregion

    #region 道具/机制相关

    #region 付费道具

    /// <summary>
    ///     记录上一步
    /// </summary>
    public void RecordLast()
    {
        var record = new BottleRecord
        {
            isFinish = isFinish,
            isNearHide = isNearHide,
            isClearHide = isClearHide,
            isFreeze = isFreeze,
            limitColor = limitColor,
            isBlackBottle = IsBlackBottle,
            waters = new List<int>(waters),
            HideWaterTypes = new List<HideWaterType>(hideTypes),
            waterItems = new List<WaterItem>(waterItems),
            bombCount = new List<int>(bombCounts)
        };
        moveRecords.Add(record);
    }

    /// <summary>
    ///     返回上一步
    /// </summary>
    /// <returns></returns>
    public bool ReturnLast()
    {
        if (moveRecords.Count <= 0) return false;

        var record = moveRecords[moveRecords.Count - 1];
        var temp = new BottleProperty
        {
            isFreeze = record.isFreeze,
            isNearHide = record.isNearHide,
            isClearHide = record.isClearHide,
            isFinish = record.isFinish,
            limitColor = record.limitColor,
            isBlackBottle = record.isBlackBottle,
            waterSet = new List<int>(record.waters),
            hideTypes = new List<HideWaterType>(record.HideWaterTypes),
            waterItem = new List<WaterItem>(record.waterItems),
            bombCounts = new List<int>(record.bombCount),

            numCake = originProperty.numCake,
            lockType = originProperty.lockType
        };

        Init(temp, bottleIdx);

        moveRecords.Remove(record);
        finishGo.SetActive(isFinish);
        return true;
    }

    #endregion

    #region 增加水块--魔法帽

    /// <summary>
    ///     增加颜色
    /// </summary>
    /// <returns></returns>
    public void AddColor(int color, Vector3 fromPos)
    {
        if (waters.Count < maxNum)
        {
            waters.Add(color);
            var fx = Instantiate(LevelManager.Instance.createFx[color - 1], fromPos, Quaternion.identity);
            fx.transform.SetParent(LevelManager.Instance.gameCanvas);
            //Debug.Log("fx " + waterImg[topIdx].transform.name);
            var useIdx = topIdx;

            var tween = fx.transform.DOMove(waterNode[useIdx].transform.position, 1f);
            tween.OnComplete(() => { Destroy(fx); })
                .OnUpdate(() => { tween.SetTarget(waterNode[useIdx].transform.position); });

            waterItems.Add(WaterItem.None);
        }
    }

    #endregion

    #region 陶瓷瓶机制

    /// <summary>
    ///     陶瓷瓶消除
    /// </summary>
    /// <param name="idx"></param>
    public void CheckNearHide(int idx)
    {
        if (Mathf.Abs(bottleIdx - idx) == 1
            && LevelManager.Instance.nowBottles[idx].isUp == isUp
            && isNearHide) //只判定isNearHide的瓶子
        {
            foreach (var item in waters) LevelManager.Instance.cantChangeColorList.Remove(item);

            //注释测试
            //SetClearHide();

            StartCoroutine(CoroutinePlayNearHide());
        }
    }

    /// <summary>
    ///     陶瓷瓶消除动画表现相关
    /// </summary>
    /// <param name="nowait"></param>
    /// <returns></returns>
    private IEnumerator CoroutinePlayNearHide(bool nowait = false)
    {
        isNearHide = false;
        if (!nowait)
        {
            yield return new WaitForSeconds(2f);
            AudioKit.PlaySound("resources://Audio/TengMan");
        }

        var trackEntry = nearHide.AnimationState.SetAnimation(0, "attack", false);
        trackEntry.Complete += trackEntry =>
        {
            nearHide.Hide();
            CheckFinish();
        };
    }

    #endregion

    #region 冰块机制

    /// <summary>
    ///     破冰(入口)
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowBreakIce()
    {
        //清除火焰水和冰块水标记
        var breakList =
            new List<(int fireWaterIndex, BottleWaterCtrl iceWater)>();
        for (var i = waterItems.Count - 1; i >= 0; i--)
        {
            var idx = i;
            if (waterItems[idx] == WaterItem.BreakIce)
            {
                var iceWater = GetIceWater();
                waterItems[idx] = WaterItem.None;
                if (iceWater == null) continue;
                breakList.Add((idx, iceWater));
            }
        }

        //等待一秒瓶子动画完成
        //yield return new WaitForSeconds(1f);
        foreach (var (fireWaterIndex, iceWater) in breakList)
        {
            StartCoroutine(waterImg[fireWaterIndex].BreakIce(iceWater));
            yield return new WaitForSeconds(0.3f);
        }

        #region 原方法

        //等待一秒瓶子动画完成
        //yield return new WaitForSeconds(1f);
        //for (int i = waterItems.Count - 1; i >= 0; i--)
        //{
        //    if (waterItems[i] == WaterItem.BreakIce)
        //    {
        //        var breakTo = GetIceWater();
        //        waterItems[i] = WaterItem.None;
        //        StartCoroutine(waterImg[i].BreakIce(breakTo));

        //        //CheckWaterItem();

        //        yield return new WaitForSeconds(0.3f);
        //    }
        //}

        #endregion
    }

    /// <summary>
    ///     获取冰块水
    /// </summary>
    /// <returns></returns>
    private BottleWaterCtrl GetIceWater()
    {
        var iceIdx = Random.Range(0, LevelManager.Instance.iceBottles.Count);

        var bottle = LevelManager.Instance.iceBottles[iceIdx];
        LevelManager.Instance.iceBottles.RemoveAt(iceIdx);

        //获取冰块水 =》得到水的层级索引 =》得到水的颜色 =》从 cantChangeColorList 移除
        var _IceWater = bottle.FindIceWater();
        var _waterIdx = bottle.waterImg.IndexOf(_IceWater);
        if (_waterIdx < 0) return null;
        LevelManager.Instance.cantChangeColorList.Remove(bottle.waters[_waterIdx]);

        return _IceWater;
    }

    /// <summary>
    ///     找冰
    /// </summary>
    /// <returns></returns>
    private BottleWaterCtrl FindIceWater()
    {
        //从上往下找
        for (int i = waterItems.Count - 1; i >= 0; i--)
        {
            if (waterItems[i] == WaterItem.Ice)
            {
                waterItems[i] = WaterItem.None;
                return waterImg[i];
            }
        }

        return null;
    }

    /// <summary>
    ///     破冰
    /// </summary>
    public void UnlockIceWater()
    {
        CheckWaterItem();
        CheckFinish();
    }

    #endregion

    #region 变色机制--药水瓶

    /// <summary>
    ///     改变颜色
    /// </summary>
    /// <param name="from">被替换</param>
    /// <param name="to">替换</param>
    public void ChangeColor(int from, int to, Transform target)
    {
        for (var i = 0; i < waters.Count; i++)
            if (waters[i] == from)
            {
                StartCoroutine(waterImg[i].ChangeShine());
                StartCoroutine(waterImg[i].ShowThunder(target));
            }

        StartCoroutine(CheckChange(from, to, target));
    }

    /// <summary>
    ///     检测变色
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    private IEnumerator CheckChange(int from, int to, Transform target)
    {
        yield return new WaitForSeconds(3f);

        for (var i = 0; i < waters.Count; i++)
            if (waters[i] == from)
                waters[i] = to;

        SetBottleColor();
        PlaySpineWaitAnim();

        CheckFinish();

        //会重复触发
        if (isFinish) CheckFinishChange(to);
    }

    /// <summary>
    ///     判断瓶子完成后是否有对应魔法布解锁
    /// </summary>
    /// <param name="color"></param>
    private void CheckFinishChange(int color)
    {
        foreach (var bottle in LevelManager.Instance.nowBottles) bottle.CheckUnlockHide(color);
    }

    #endregion

    #region 移除单色--扫帚

    /// <summary>
    ///     移除单色道具动画(扫帚动画)
    /// </summary>
    /// <param name="color"></param>
    /// <param name="fromPos"></param>
    public void PlayBroomBullet(int color, Vector3 fromPos)
    {
        var list = new List<BottleWaterCtrl>();
        for (var i = 0; i < waters.Count; i++)
            if (waters[i] == color)
                list.Add(waterImg[i]);

        foreach (var ctrl in list)
        {
            var go = Instantiate(LevelManager.Instance.broomBullet, LevelManager.Instance.mSpineIniPar);

            var fly = go.GetComponent<FlyCtrl>();
            fly.target = ctrl.transform;
            fly.flyTime = 1.2f;
            go.transform.position = fromPos;
            fly.BeginFly();
        }
    }

    /// <summary>
    ///     判断瓶子自身是否有要移除的单色
    /// </summary>
    /// <param name="color"></param>
    public BottleCtrl CheckRemoveOneColor(int color)
    {
        for (var i = 0; i < waters.Count; i++)
            if (waters[i] == color)
                return this;

        return null;
    }

    /// <summary>
    ///     移除单色
    /// </summary>
    /// <param name="color"></param>
    /// <param name="sameBottle">是否在一个瓶子</param>
    public void RemoveAllOneColor(int color, bool sameBottle)
    {
        var list = new List<int>();
        var items = new List<WaterItem>();
        var hides = new List<HideWaterType>();
        List<int> tempbomb = new();
        for (var i = 0; i < waters.Count; i++)
            if (waters[i] == color)
            {
                StartCoroutine(PlayShine(i, sameBottle));
            }
            else
            {
                list.Add(waters[i]);
                items.Add(waterItems[i]);
                // hides.Add(hideWaters[i]);
                hides.Add(hideTypes[i]);
                tempbomb.Add(bombCounts[i]);
            }

        waterItems = items;
        waters = list;
        bombCounts = tempbomb;
        hideTypes = hides;
    }

    /// <summary>
    ///     移除单色动画特效
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    private IEnumerator PlayShine(int i, bool sameBottle)
    {
        isPlayAnim = true;
        var imgcmp = waterImg[i].transform.GetComponent<Image>();
        StartCoroutine(waterImg[i].ShowBroomAfter());


        yield return new WaitForSeconds(2.2f);
        imgcmp.material = null;


        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        SetBottleColor();
        CheckItem();

        if (sameBottle)
        {
            //Debug.Log("在一个瓶子：" + sameBottle);
            isFinish = false;
            finishSpine.Hide();
        }
        //CheckFinish();//移除颜色不会触发完成

        if (topIdx < 0) spineGo.gameObject.SetActive(false);

        isPlayAnim = false;
    }

    #endregion

    #region 炸弹机制

    /// <summary>
    ///     炸弹是否爆炸判断
    /// </summary>
    public bool CheckBoomFailure()
    {
        var _flag = false;
        var moveNum = LevelManager.Instance.moveNum;

        for (var i = 0; i < bombCounts.Count; i++)
            if (bombCounts[i] < moveNum + 1 && bombCounts[i] != 0)
            {
                waterItems[i] = WaterItem.None;
                bombCounts[i] = 0;
                if (waters[i] == 5002) waters[i] = 0;

                waterImg[i].bombCtrl.BombBoom();
                return true;
            }

        // 出现炸弹
        RemoveItem();
        SetBottleColor();

        return false;
    }

    // 先更新的炸弹，后倒的水
    public void UpdateBomb(BottleCtrl bottleCtrl = null, bool Init = false)
    {
        var moveNum = LevelManager.Instance.moveNum;
        var haveBomb = false;
        var _flag = false;
        // 飞天炸弹的连锁更新 
        for (var i = waters.Count - 1; i >= 0; i--)
            if (waters[i] == 5002)
            {
                waterImg[i].bombCtrl.SetBomb(aniType: "flap");
                waterItems[i] = WaterItem.None;
                bombCounts[i] = FLYBOMBING_SIGN;
                waters[i] = 0;
                _flag = true;
            }
            else
            {
                break;
            }

        // UI更新
        for (var i = 0; i < bombCounts.Count; i++)
            switch (bombCounts[i])
            {
                case BOMBREMOVE_SIGN:
                    waterImg[i].bombCtrl.SetBomb(aniType: "bomp_remove");
                    break;
                case FLYBOMBING_SIGN:
                    waterImg[i].bombCtrl.SetBomb(aniType: "flap");
                    break;
                case NULLBOMB_SIGN:
                    // 处理空炸弹逻辑
                    break;
                // 正常炸弹
                default:
                    if (waterItems[i] == WaterItem.Bomb)
                        waterImg[i].bombCtrl.SetBomb(true, (bombCounts[i] - moveNum).ToString(),
                            "idle");
                    else
                        waterImg[i].bombCtrl.SetBomb(true, (bombCounts[i] - moveNum).ToString(),
                            "idle", true);
                    haveBomb = true;
                    break;
            }

        if (!haveBomb)
            LevelManager.Instance.bombList.Remove(this);

        if (_flag)
        {
            RemoveItem();
            SetBottleColor();
        }
    }

    public void ClearBomb()
    {
        for (var i = 0; i < bombCounts.Count; i++)
            if (bombCounts[i] > 0)
            {
                if (waters[i] == 5002) waters[i] = 0;

                waterImg[i].bombCtrl.SetBomb(aniType: "bomp_remove");
                waterImg[i].textItem.text = "";
                bombCounts[i] = 0;
                waterItems[i] = WaterItem.None;
            }

        RemoveItem();
        SetBottleColor();
    }
    /*public void MovingBomb(BottleCtrl other)
    {
        bool is
    }*/

    #endregion

    #region 魔法布机制

    /// <summary>
    ///     检测是否有魔法布解锁
    /// </summary>
    /// <param name="color"></param>
    public void CheckUnlockHide(int color)
    {
        if (isClearHide && !hasUnlockHidePlayed)
        {
            if (unlockClear == color)
            {
                hasUnlockHidePlayed = true;
                ++LevelManager.Instance.playingHideAnimCount;
                UIKit.OpenPanel<UIPropMask>(UILevel.PopUI);
                foreach (var item in waters) LevelManager.Instance.cantChangeColorList.Remove(item);

                LevelManager.Instance.cantChangeColorList.Remove(color);
                StartCoroutine(HideClearHide());
            }
        }
    }

    /// <summary>
    ///     魔法布解锁
    /// </summary>
    /// <returns></returns>
    private IEnumerator HideClearHide()
    {
        isClearHide = false;
        isClearHideAnim = true;
        yield return new WaitForSeconds(1.5f);
        AudioKit.PlaySound("resources://Audio/MagicCloth");

        //加入事件
        TrackEntry trackEntry = null;
        if (unlockClear > 0 && unlockClear < (int)EDisapearAnim.IDLE_MAX)
            trackEntry = clearHide.AnimationState.SetAnimation(0,
                GameEnum.GetDescription((EDisapearAnim)unlockClear), false);

        if (trackEntry != null)
            trackEntry.Complete += entry =>
            {
                clearHide.gameObject.SetActive(false);
                isClearHideAnim = false;
                --LevelManager.Instance.playingHideAnimCount;
                if (LevelManager.Instance.ISPlayingHideAnim) UIKit.ClosePanel<UIPropMask>();
            };

        CheckFinish();
    }

    #endregion

    #region 彩色水机制

    public void ChangeWaterToRainBowWater(int sourceColor)
    {
        for (var i = 0; i < waters.Count; i++)
        {
            var _idx = i;
            if (waters[_idx] == sourceColor)
            {
                //目标是黑水则显示
                if (hideTypes[_idx] != HideWaterType.None)
                {
                    hideTypes[_idx] = HideWaterType.None;
                    waterImg[_idx].HideGo.Hide();
                }

                waters[_idx] = (int)ItemType.RainBowWater;
                LevelManager.Instance.isPlayFxAnim = true;
                waterImg[_idx].changeShineSpine.Show();
                waterImg[_idx].changeShineSpine.AnimationState.SetEmptyAnimation(0, 0f);
                waterImg[_idx].changeShineSpine.AnimationState.SetAnimation(0, "attack", false);

                ActionKit.Delay(2f, () =>
                {
                    waterImg[_idx].changeShineSpine.Hide();
                    LevelManager.Instance.isPlayFxAnim = false;

                    SetBottleColor();
                }).Start(this);
            }
        }
    }

    #endregion

    #region 帘子机制

    public void UpdateCurtain(int stage)
    {
        curtainHight = stage - 1;
        curtainCtrl.SetCurtain(curtainHight);
    }

    public void InitCurtain(int stage)
    {
        curtainHight = stage;
        curtainCtrl.InitCurtain(stage);
    }

    #endregion

    #region 草水机制

    // 清空草炸弹
    public void ClearGrassBomb()
    {
        for (var i = 0; i < waters.Count; i++)
        {
            if (waterItems[i] == WaterItem.GrassBomb)
            {
                waterItems[i] = WaterItem.None;
                waterImg[i].grassWaterCtrl.BombDis();
            }
        }
    }

    #endregion
    /// <summary>
    ///     清空空水块(机制水块)
    /// </summary>
    public void RemoveItem()
    {
        var _tempWater = new List<int>();
        var _tempWaterItem = new List<WaterItem>();
        var _tempBomb = new List<int>();
        var _tempHideWater = new List<HideWaterType>();

        for (var i = 0; i < waters.Count; i++)
            if (waters[i] != 0)
            {
                _tempWater.Add(waters[i]);
                _tempWaterItem.Add(waterItems[i]);
                _tempBomb.Add(bombCounts[i]);
                _tempHideWater.Add(hideTypes[i]);
            }

        waters = _tempWater;
        waterItems = _tempWaterItem;
        bombCounts = _tempBomb;
        hideTypes = _tempHideWater;
    }

    /// <summary>
    ///     移除瓶内黑色水块
    /// </summary>
    public void RemovHide()
    {
        for (var i = 0; i < hideTypes.Count; i++) hideTypes[i] = HideWaterType.None;

        LevelManager.Instance.hideBottleList.Remove(this);

        SetBottleColor();
        CheckFinish();
    }

    /// <summary>
    ///     星星特效(去除黑水)
    /// </summary>
    public void StarSetHideShow()
    {
        for (var i = 0; i < hideTypes.Count; i++)
            if (hideTypes[i] == HideWaterType.HideWater)
            {
                waterImg[i].PlayStarBlackWaterEffect();
                hideTypes[i] = HideWaterType.None;
            }
    }

    /// <summary>
    ///     清除所有特殊情况(魔法阵/魔法棒道具)
    /// </summary>
    /// <param name="removeOne">清除单瓶负面</param>
    public void SetNormal(bool removeOne = false)
    {
        ClearGrassBomb();
        //黑水移除
        for (var i = 0; i < hideTypes.Count; i++) hideTypes[i] = HideWaterType.None;

        //移除WaterItem
        var _needDisWater = false;
        for (var i = 0; i < waterItems.Count; i++)
        {
            switch (waterItems[i])
            {
                // 泡沫
                case WaterItem.Bubble or WaterItem.Bubble_Origin:
                    waterImg[i].bubbleCtrl.BubbleDead(waterItems[i] == WaterItem.Bubble_Origin);
                    LevelManager.Instance.DeleteBubble(this);
                    break;
                // 炸弹
                case WaterItem.Bomb or WaterItem.FlyBomb:
                    waterImg[i].bombCtrl.SetBomb(true, "", "bomp_remove");
                    if (waters[i] == 5002)
                    {
                        waters[i] = 0;
                        _needDisWater = true;
                    }

                    break;
            }

            if (removeOne && waterItems[i] == WaterItem.BreakIce) continue;

            waterItems[i] = WaterItem.None;
        }

        //移除炸弹 (0表示爆炸后的状态)
        for (var i = 0; i < bombCounts.Count; i++) bombCounts[i] = 0;

        //清理空水块
        if (_needDisWater)
            RemoveItem();

        //藤曼底座
        if (isFreeze)
        {
            AudioKit.PlaySound("resources://Audio/ThornBase");
            freezeSpine.AnimationState.SetAnimation(0, "attack", false);
        }

        //限制瓶
        if (limitColor != 0 && !isFinish)
            if (limitColor > 0 && limitColor < (int)ECombimeAnim.IDLE_MAX)
            {
                limitColorSpine.AnimationState.SetAnimation(0,
                    GameEnum.GetDescription((ECombimeAnim)limitColor), false);
                limitColor = 0;
            }

        // 帘子瓶
        if (LevelManager.Instance.curtainDict.ContainsKey(this))
        {
            curtainCtrl.ClearCurtain();
            curtainHight = 0;
        }

        // 纯黑瓶
        if (BlackWaterGoPar)
        {
            BlackWaterGoPar.SetActive(false);
            IsBlackBottle = false;
        }

        isFreeze = false;
        isNearHide = false;
        isClearHide = false;

        StartCoroutine(CoroutinePlayNearHide(true));

        SetBottleColor(false);
        CheckFinish();
    }

    #endregion

    #region 瓶子完成相关

    /// <summary>
    ///     判断是否完成
    /// </summary>
    /// <param name="isChange"></param>
    public void CheckFinish(bool isChange = false)
    {
        if (!isNearHide && !isClearHide && !isFinish
            && maxNum == 4 && waters.Count == maxNum)
        {
            var topColor = waters[topIdx];
            for (var i = topIdx - 1; i >= 0; i--)
                if (waters[i] != topColor || waterItems[i] == WaterItem.Ice)
                    return;

            if (LevelManager.Instance.clearList.Count > 0)
                LevelManager.Instance.clearList.Remove(topColor);

            OnFinish(topColor);
        }
    }

    /// <summary>
    ///     完成后的处理
    /// </summary>
    public void OnFinish(int finishColor)
    {
        // 帘子机制
        LevelManager.Instance.CurtainUpdate();
        isFinish = true;
        //———标记———
        //原debuff状态逻辑是在表现动画回调修改的(现改成触发时直接修改)
        foreach (var item in LevelManager.Instance.nowBottles)
        {
            item.CheckUnlockHide(finishColor);
            item.CheckNearHide(bottleIdx);
        }

        StartCoroutine(ShowBreakIce());

        for (var i = 0; i < waterItems.Count; i++)
            if (waterItems[i] == WaterItem.Bomb || waterItems[i] == WaterItem.FlyBomb)
            {
                waterItems[i] = WaterItem.None;
                if (bombCounts.Count > i) bombCounts[i] = 100;
            }
        
        //标记——完成后不需要对自身更新水块机制状态
        //CheckWaterItem();
        StartCoroutine(ShowFinish());
        LevelManager.Instance.FinishClear();
    }

    /// <summary>
    ///     完成动画
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowFinish()
    {
        var trackEntry = finishSpine.AnimationState.SetAnimation(0, "animation", false);
        trackEntry.Complete += trackEntry =>
        {
            if ((ItemType)GetMoveOutTop() == ItemType.RainBowWater)
            {
                waters.Clear();
                isFinish = false;
                finishSpine.Hide();
                SetBottleColor();
                RemoveItem();
            }

            if ((ItemType)GetMoveOutTop() == ItemType.FlashWater)
                if (!LevelManager.Instance.isFlashWaterBottleAdded)
                {
                    LevelManager.Instance.isFlashWaterBottleAdded = true;
                    LevelManager.Instance.AddBottle(true);
                }
        };

        //等待水倒进去
        yield return new WaitForSeconds(0.8f);
        AudioKit.PlaySound("resources://Audio/Finish");
        StartCoroutine(PlayBottleCapSound());

        if (isFreeze)
        {
            AudioKit.PlaySound("resources://Audio/ThornBase");
            freezeSpine.AnimationState.SetAnimation(0, "attack", false);
        }

        //yield return new WaitForSeconds(0.2f);
        finishGo.SetActive(isFinish);

        //等待瓶子合成动画完成
        yield return new WaitForSeconds(1f);

        if (limitColor != 0)
            if (limitColor > 0 && limitColor < (int)ECombimeAnim.IDLE_MAX)
                limitColorSpine.AnimationState.SetAnimation(0,
                    GameEnum.GetDescription((ECombimeAnim)limitColor), false);
        // 纯黑瓶
        if (IsBlackBottle)
        {
            IsBlackBottle = false;
            BlackWaterGoPar.SetActive(false);
        }
        
        bubbleSpine.gameObject.SetActive(true);
        var trackEntry1 = bubbleSpine.AnimationState.GetCurrent(0); // 获取轨道0上的当前动画条目
        if (trackEntry1 != null)
        {
            //trackEntry.TimeScale = 1f;
            bubbleSpine.Initialize(true);
        }
        else
        {
            // 如果当前没有动画，直接设置动画
            bubbleSpine.AnimationState.SetAnimation(0, "maopao", false);
        }

    }

    /// <summary>
    ///     播放瓶盖声音
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayBottleCapSound()
    {
        yield return new WaitForSeconds(1f);
        AudioKit.PlaySound("resources://Audio/BottleCap");

    }

    #endregion

    #region obsolete

    /// <summary>
    ///     取得倒出水的对应WaterItem
    /// </summary>
    /// <returns></returns>
    public WaterItem GetMoveOutItemTop()
    {
        if (topIdx < 0) return WaterItem.None;

        return waterItems[topIdx];
    }

    public IEnumerator FinishHide()
    {
        isPlayAnim = true;
        yield return new WaitForSeconds(1);
        //Debug.Log("fx " + waters.Count + " " + name);
        CheckItem();
        SetBottleColor();
        CheckFinish();
        foreach (var item in waterImg) item.waterImg.fillAmount = 1;

        isPlayAnim = false;
    }

    #endregion
}