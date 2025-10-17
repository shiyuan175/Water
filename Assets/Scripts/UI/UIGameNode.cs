using UnityEngine;
using UnityEngine.UI;
using QFramework;
using System;
using GameDefine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine.U2D;


namespace QFramework.Example
{
    public class UIGameNodeData : UIPanelData
    {
    }

    public partial class UIGameNode : UIPanel, IController
    {
        private const string CLEAR_BWATER_PARTICLE_PATH = "Prefab/BlackMaskItem";
        private const int GET_THE_LAST_NUMBER_OF_LEVEL = 10;

        [Header("关卡难度UI")]
        #region 关卡难度UI
        
        [SerializeField] private Sprite[] imgBtnItemBgSprites;
        [SerializeField] private Sprite[] imgTopBgSprites;
        [SerializeField] private Sprite[] imgBottomSpirtes;
        [SerializeField] private Sprite[] imgLevelSprites;
        [SerializeField] private Sprite[] imgBtnReturnSprites;
        [SerializeField] private Image[] imgBtnItemBg;
        [SerializeField] private Image imgBtnReturn;
        [SerializeField] private Image imgTopBg;
        [SerializeField] private Image imgBottom;
        [SerializeField] private Image imgLevel;

        #endregion

        [Header("前五关故事引导UI")]
        #region 前五关故事引导UI
        [SerializeField] private GameObject g_Star_MagicBook_Guide;
        [SerializeField] private Sprite s_StarSprite;
        [SerializeField] private Image[] i_StarFrames;
        #endregion

        private ResLoader mResLoader;
        private StageModel stageModel;
        private SpriteAtlas mRankLevelSpriteAtlas;

        private int mCacheRankSpriteIndex;

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UIGameNodeData ?? new UIGameNodeData();
            // please add init code here
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            stageModel = this.GetModel<StageModel>();

            LoadRes();
            BindBtn();
            RegisterEvent();
            SetTakeItem();
        }

        protected override void OnShow()
        {
            InitStoryUI();
            InitRankLevel();
            InitLevelUI();
            InitItemUI();
            SetItem();
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            stageModel = null;
            BtnStepBack.onClick.RemoveAllListeners();
            BtnRemoveHide.onClick.RemoveAllListeners();
            BtnAddBottle.onClick.RemoveAllListeners();
            BtnHalfBottle.onClick.RemoveAllListeners();
            BtnRemoveAll.onClick.RemoveAllListeners();
            BtnReturn.onClick.RemoveAllListeners();
            BtnItem1.onClick.RemoveAllListeners();
            BtnItem2.onClick.RemoveAllListeners();
            BtnItem3.onClick.RemoveAllListeners();

            if (mResLoader != null)
            {
                mResLoader.Recycle2Cache();
                mResLoader = null;
                mRankLevelSpriteAtlas = null;
            }
        }

        private void LoadRes()
        {
            if (this.GetUtility<SaveDataUtility>().GetCurrentLevel() >= GameDefine.GameConst.IN_GAME_RANK_BEGIN_LEVEL)
            {
                mResLoader = ResLoader.Allocate();
                mRankLevelSpriteAtlas = mResLoader.LoadSync<SpriteAtlas>
                    (ABResourceDefine.RANK_LEVEL_ATLAS_BUNDLENAME, ABResourceDefine.RANK_LEVEL_ATLAS_ASSETNAME);
            }
        }

        private void BindBtn()
        {
            BtnReturn.onClick.AddListener(() =>
            {
                UIKit.OpenPanel<UIRetry>();
            });

            BtnItem1.onClick.AddListener(() =>
            {
                UseItem(6, BtnItem1);
            });
            BtnItem2.onClick.AddListener(() =>
            {
                UseItem(7, BtnItem2);
            });
            BtnItem3.onClick.AddListener(() =>
            {
                LevelManager.Instance.ShowItemSelect();
                GameCtrl.Instance.SeletedItem(bottele => { UseItem(8, BtnItem3, bottele); });
            });

            BtnRemoveAll.onClick.AddListener(BtnRemoveAllOnClick);
            BtnAddBottle.onClick.AddListener(BtnAddBottleOnClick);
            BtnHalfBottle.onClick.AddListener(BtnHalfBottleOnClick);
            BtnRemoveHide.onClick.AddListener(BtnRemoveHideOnClick);
            BtnStepBack.onClick.AddListener(BtnSetpBackOnClick);
        }

        private void RegisterEvent()
        {
            this.RegisterEvent<RefreshItemEvent>(e =>
            {
                SetItem();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<UnLockItem>(e =>
            {
                UnLockItem(e.PropType);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<LevelStartEvent>(eventId =>
            {
                TxtLevel.text = LevelManager.Instance.levelId.ToString();
                InitStoryUI();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            StringEventSystem.Global.Register(GameConst.STREAK_WIN_REMOVE_HIDE, () =>
            {
                ClearBottleBlackWater(false);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            StringEventSystem.Global.Register(GameConst.VICTORY_EVENT, () =>
            {
                int level = this.GetUtility<SaveDataUtility>().GetCurrentLevel();
                //原是第八关才显示段位(5~7关直接返回)
                //现在是第六关显示，第五关通过时会触发返回
                if (level - 1 < GameConst.IN_GAME_RANK_BEGIN_LEVEL)
                {
                    OpenUIVictory();
                    return;
                }

                var _tempWin = stageModel.InGameRankStreakWinNum;

                //飞星效果
                var curRankIndex = Mathf.Min(8, Mathf.Max(0, (_tempWin - 1) / 5));
                FlightEffects.Show();
                FlightEffects.DOMove(ImgRankLevel.transform.position, 1f)
                .OnComplete(() =>
                {
                    TxtRankLevel.text = _tempWin.ToString();

                    //段位无晋升
                    if (curRankIndex <= mCacheRankSpriteIndex)
                    {
                        OpenUIVictory();
                        return;
                    }

                    ImgRankSprite_mid.sprite = mRankLevelSpriteAtlas.GetSprite(GameUtils.GetAtlasSpriteName(mCacheRankSpriteIndex));
                    ImgRankSprite_mid.SetNativeSize();
                    SpineRankPromotion.Show();

                    SpineRankPromotion.AnimationState.SetAnimation(0, "animation", false);

                    ActionKit.Delay(0.5f, () =>
                    {
                        ImgRankSprite_mid.sprite =
                        mRankLevelSpriteAtlas.GetSprite(GameUtils.GetAtlasSpriteName(curRankIndex));
                        ImgRankLevel.sprite =
                        mRankLevelSpriteAtlas.GetSprite(GameUtils.GetAtlasSpriteName(curRankIndex));

                        ImgRankSprite_mid.SetNativeSize();
                    }).Start(this);

                    SpineRankPromotion.AnimationState.Complete += (trackEntry) =>
                    {
                        SpineRankPromotion.Hide();

                        if (stageModel.CompareWithHistoryBestRank(curRankIndex))
                        {
                            //Debug.Log("首次晋升段位");
                            CoinManager.Instance.AddCoin(300);
                            RewardUIManager.Instance.PlayRewardAnim(300, true, null);

                            ActionKit.Delay(1.5f, () =>
                            {
                                OpenUIVictory();
                            }).Start(this);
                            return;
                        }
                        //奖励已经领取
                        OpenUIVictory();
                    };
                });

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void InitStoryUI()
        {
            int _curLevel = this.GetUtility<SaveDataUtility>().GetCurrentLevel();
            if (_curLevel > GameDefine.GameConst.NEWBIE_LEVEL_COUNT)
                return;

            g_Star_MagicBook_Guide.Show();
            //不对，关卡起始是1，
            //1是不亮，2亮一个，3亮2个，4亮3个，5亮4个，数组长4
            var _temp = Mathf.Clamp(_curLevel - 1, 0, i_StarFrames.Length);
            for (int i = 0; i < _temp; i++)
            {
                i_StarFrames[i].sprite = s_StarSprite;
            }
        }

        private void InitLevelUI()
        {
            int level = this.GetUtility<SaveDataUtility>().GetCurrentLevel();
            TxtLevel.text = level.ToString();

            if (level <= 5)
                BtnReturn.Hide();

            if (level < GET_THE_LAST_NUMBER_OF_LEVEL)
                return;
            int _index = 0;
            switch (level % GET_THE_LAST_NUMBER_OF_LEVEL)
            {
                case (int)LevelHardType.Hard:
                    _index = 1;
                    break;

                case (int)LevelHardType.VeryHand:
                    _index = 2;
                    break;

                    // t初始化为0，所以没有用Defailt取0
            }
            if (_index != 0)
                SetTextTip();
            // 换按钮的背景颜色
            /*foreach (var i in imgBtnItemBg)
                i.sprite = imgBtnItemBgSprites[_index];*/
            imgTopBg.sprite = imgTopBgSprites[_index];
            imgLevel.sprite = imgLevelSprites[_index];
            imgBottom.sprite = imgBottomSpirtes[_index];
            imgBtnReturn.sprite = imgBtnReturnSprites[_index];

        }

        private void SetTextTip()
        {
            // 设置动画
            LevelTipPanel.Show();
            float _durationTime = 1.5f;
            CanvasGroup _canvasGroup = LevelTipPanel.GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;
            _canvasGroup.DOFade(1f, _durationTime)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    _canvasGroup.DOFade(0f, _durationTime)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        LevelTipPanel.Hide();
                    });

                });

            // 设置文本 5-20的偏转值
            TextLevelTip.text = UnityEngine.Random.Range(50, 70).ToString() + "% of players were defeated at this level";


        }

        /// <summary>
        /// 显示道具图标
        /// </summary>
        private void InitItemUI()
        {
            int level = this.GetUtility<SaveDataUtility>().GetCurrentLevel();

            if (level > (int)GameDefine.UIGuideLevel.UIGuideLevelAddBottle)
                UnLockItem(NormalRewardsType.AddOneBottle);

            if (level > (int)GameDefine.UIGuideLevel.UIGuideLevelHalfBottle)
                UnLockItem(NormalRewardsType.AddHalfBottle);

            if (level > (int)GameDefine.UIGuideLevel.UIGuideLevelRemoveHide)
                UnLockItem(NormalRewardsType.RemoveHide);

            if (level > (int)GameDefine.UIGuideLevel.UIGuideLevelRemoveAll)
                UnLockItem(NormalRewardsType.RemoveAll);

            if (level > (int)GameDefine.UIGuideLevel.UIGuideLevelStepBack)
                UnLockItem(NormalRewardsType.StepBack);

            if (level >= (int)GameDefine.UnLockMechanism.EnterLevelSelectProps)
                BtnItemBg.Show();
        }

        private void InitRankLevel()
        {
            int level = this.GetUtility<SaveDataUtility>().GetCurrentLevel();

            if (level == GameConst.IN_GAME_RANK_BEGIN_LEVEL)
            {
                ImgRankLevel_Label.Show();
                ActionKit.Delay(5f, () =>
                {
                    ImgRankLevel_Label.Hide();

                }).Start(this);
            }
            else ImgRankLevel_Label.Hide();

            if (level >= GameConst.IN_GAME_RANK_BEGIN_LEVEL)
            {
                ImgRankLevel.Show();
                var _tempWin = stageModel.InGameRankStreakWinNum;
                TxtRankLevel.text = _tempWin.ToString();
                //5次连胜晋升一个段位,总段位数9(起始0)
                mCacheRankSpriteIndex = Mathf.Min(8, Mathf.Max(0, (_tempWin - 1) / 5));
                ImgRankLevel.sprite = mRankLevelSpriteAtlas.GetSprite(GameUtils.GetAtlasSpriteName(mCacheRankSpriteIndex));
            }

            else ImgRankLevel.Hide();
        }

        private void UnLockItem(NormalRewardsType PropType)
        {
            Transform transform = null;
            switch (PropType)
            {
                case NormalRewardsType.StepBack:
                    transform = BtnStepBack.transform;
                    break;

                case NormalRewardsType.RemoveHide:
                    transform = BtnRemoveHide.transform;
                    break;

                case NormalRewardsType.AddHalfBottle:
                    transform = BtnHalfBottle.transform;
                    break;

                case NormalRewardsType.AddOneBottle:
                    transform = BtnAddBottle.transform;
                    break;

                case NormalRewardsType.RemoveAll:
                    transform = BtnRemoveAll.transform;
                    break;
            }

            transform.Find("ImgItem").Show();
            transform.Find("ImgLock").Hide();
            transform.GetComponent<Button>().interactable = true;
            transform.Find("ImgItem").GetComponent<Image>().color = Color.white;
        }

        #region 道具相关
        private void BtnSetpBackOnClick()
        {
            if (!LevelManager.Instance.isPlayFxAnim && GameCtrl.Instance.IsPouring)
            {
                if (stageModel.ItemDic[1] <= 0)
                {
                    UIBuyItemData data = new UIBuyItemData() { item = 1 };
                    UIKit.OpenPanel<UIBuyItem>(data);
                    return;
                }
                if (LevelManager.Instance.ReturnLast())
                    stageModel.ReduceItem(1, 1);
            }
        }
        private void BtnRemoveHideOnClick()
        {
            if (!LevelManager.Instance.isPlayFxAnim && GameCtrl.Instance.IsPouring)
            {
                if (stageModel.ItemDic[2] <= 0)
                {
                    UIBuyItemData data = new UIBuyItemData() { item = 2 };
                    UIKit.OpenPanel<UIBuyItem>(data);
                    return;
                }
                //判断是否有黑水瓶
                if (LevelManager.Instance.hideBottleList.Count != 0)
                {
                    LevelManager.Instance.RemoveHide(() =>
                    {
                        stageModel.ReduceItem(2, 1);
                    });
                }
            }
        }
        private void BtnAddBottleOnClick()
        {
            if (!LevelManager.Instance.isPlayFxAnim && GameCtrl.Instance.IsPouring)
            {
                if (stageModel.ItemDic[3] <= 0)
                {
                    UIBuyItemData data = new UIBuyItemData() { item = 3 };
                    UIKit.OpenPanel<UIBuyItem>(data);
                    return;
                }
                LevelManager.Instance.AddBottle(false, () =>
                {
                    stageModel.ReduceItem(3, 1);
                });
            }
        }
        private void BtnHalfBottleOnClick()
        {
            if (!LevelManager.Instance.isPlayFxAnim && GameCtrl.Instance.IsPouring)
            {
                if (stageModel.ItemDic[4] <= 0)
                {
                    UIBuyItemData data = new UIBuyItemData() { item = 4 };
                    UIKit.OpenPanel<UIBuyItem>(data);
                    return;
                }
                LevelManager.Instance.AddBottle(true, () =>
                {
                    stageModel.ReduceItem(4, 1);
                });
            }
        }
        private void BtnRemoveAllOnClick()
        {
            if (!LevelManager.Instance.isPlayFxAnim && GameCtrl.Instance.IsPouring)
            {
                if (stageModel.ItemDic[5] <= 0)
                {
                    UIBuyItemData data = new UIBuyItemData() { item = 5 };
                    UIKit.OpenPanel<UIBuyItem>(data);
                    return;
                }
                if (LevelManager.Instance.CheckAllDebuff())
                {
                    LevelManager.Instance.RemoveAll(() =>
                    {
                        //清空操作记录的障碍(避免回退恢复)
                        foreach (var bottle in LevelManager.Instance.nowBottles)
                        {
                            foreach (var record in bottle.moveRecords)
                            {
                                record.isFreeze = false;
                                record.isClearHide = false;
                                record.isNearHide = false;
                                record.limitColor = 0;

                                for (int i = 0; i < record.hideWaters.Count; i++)
                                {
                                    record.hideWaters[i] = false;
                                }
                                for (int i = 0; i < record.waterItems.Count; i++)
                                {
                                    record.waterItems[i] = WaterItem.None;
                                }

                                for (int i = 0; i < record.bombCount.Count; i++)
                                {
                                    record.bombCount[i] = 0;
                                }
                            }
                        }
                    });
                    stageModel.ReduceItem(5, 1);
                }
            }
        }

        /// <summary>
        /// 使用携带道具
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="itemObj"></param>
        /// <param name="botter">作用与哪个瓶子(打乱水块道具传入)</param>
        void UseItem(int itemID, Button itemObj, BottleCtrl botter = null)
        {
            switch (itemID)
            {
                case 6:
                    LevelManager.Instance.AddBottle(true, () =>
                    {
                        TxtItem1.text = "0";
                    });
                    break;

                case 7:
                    if (!(LevelManager.Instance.hideBottleList.Count > 0))
                        return;
                    ClearBottleBlackWater(true, () =>
                    {
                        TxtItem2.text = "0";
                    });
                    break;

                case 8:
                    // 索引列表用于随机洗牌
                    List<int> _indices = Enumerable.Range(0, botter.waters.Count).ToList();
                    do
                    {
                        for (int i = 0; i < _indices.Count; i++)
                        {
                            int randIndex = UnityEngine.Random.Range(i, _indices.Count);
                            (_indices[i], _indices[randIndex]) = (_indices[randIndex], _indices[i]);
                        }
                    }
                    while (Enumerable.SequenceEqual(_indices.Select(i => botter.waters[i]), botter.waters));

                    List<int> _newWaters = new List<int>();
                    List<bool> _newHideWater = new List<bool>();
                    List<WaterItem> _newWaterItems = new List<WaterItem>();
                    List<int> _newBombs = new List<int>();

                    foreach (int idx in _indices)
                    {
                        _newWaters.Add(botter.waters[idx]);
                        _newHideWater.Add(botter.hideWaters[idx]);
                        _newWaterItems.Add(botter.waterItems[idx]);
                        _newBombs.Add(botter.bombCounts[idx]);
                    }
                    // 替换原列表
                    botter.waters = _newWaters;
                    botter.hideWaters = _newHideWater;
                    botter.waterItems = _newWaterItems;
                    botter.bombCounts = _newBombs;

                    //修改水块颜色和切换道具位置
                    for (int i = 0; i < botter.waters.Count; i++)
                    {
                        var useColor = botter.waters[i] - 1;
                        if (useColor < 1000)
                            botter.waterImg[i].SetColorState(ItemType.UseColor, LevelManager.Instance.waterColor[useColor], i == botter.topIdx);
                        else
                            botter.waterImg[i].SetColorState((ItemType)botter.waters[i], LevelManager.Instance.ItemColor, i == botter.topIdx);
                    }

                    //修改水面位置，修改水面颜色并播放水面动画
                    botter.SetNowSpinePos(botter.waters.Count);
                    botter.PlaySpineWaitAnim();
                    botter.CheckWaterItem();
                    botter.UpdateBomb();
                    botter.SetHideShow(true);
                    LevelManager.Instance.HideItemSelect();

                    TxtItem3.text = "0";
                    //Debug.Log("打乱顺序成功");
                    break;
            }

            //if (!CheckHaveItem(itemID))//调整为仅使用一次
            itemObj.interactable = false;
        }

        /// <summary>
        /// 祛除瓶中所有黑水
        /// </summary>
        /// <param name="useItem">是否由道具生效</param>
        /// <param name="action">回调(道具使用时传入)</param>
        private void ClearBottleBlackWater(bool useItem, Action action = null)
        {
            if (LevelManager.Instance.hideBottleList.Count > 0)
            {
                //剔除魔法布和陶瓷瓶的瓶子
                //即使列表为0也往下执行(需要播放动画)
                var _tempList = new List<BottleCtrl>(LevelManager.Instance.hideBottleList);
                _tempList.RemoveAll(item => item.isClearHide || item.isNearHide);

                int _removeCount = useItem ? 1 : _tempList.Count / 2;
                //特判处理(只有一个黑水瓶)
                _removeCount = Math.Min(_removeCount, _tempList.Count);

                while (_tempList.Count > _removeCount)
                {
                    int randIndex = UnityEngine.Random.Range(0, _tempList.Count);
                    _tempList.RemoveAt(randIndex);
                }

                if (useItem)
                    useItemClearBWater(_tempList, action);
                else
                    StreaWinClearBWater(_tempList, action);
            }
        }

        private void StreaWinClearBWater(List<BottleCtrl> tempList, Action action)
        {
            var _particle = Resources.Load(CLEAR_BWATER_PARTICLE_PATH);
            var _tempObj = Instantiate(_particle) as GameObject;
            //UIKit.OpenPanel<UIMask>(UILevel.PopUI);//遮罩
            _tempObj.transform.DOLocalMoveY(0, 0.8f);
            _tempObj.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.8f)
            .OnComplete(() =>
            {
                foreach (var item in tempList)
                {
                    item.StarSetHideShow();
                }
                action?.Invoke();
                Destroy(_tempObj);
            });
        }

        private void useItemClearBWater(List<BottleCtrl> tempList, Action action)
        {
            //后续道具弹出的动画，
            //动画结束回调执行去黑

            foreach (var item in tempList)
            {
                //星星去黑效果
                item.StarSetHideShow();

                //原先的去黑效果
                //for (int i = 0; i < item.hideWaters.Count; i++)
                //{
                //    item.hideWaters[i] = false;
                //}
                //item.SetHideShow(true);
            }

            action?.Invoke();
        }

        /// <summary>
        /// 使用携带道具按钮事件
        /// </summary>
        /// 进入游戏/重置关卡调用
        private void SetTakeItem()
        {
            var takeItems = LevelManager.Instance.takeItem;
            var buttons = new[] { BtnItem1, BtnItem2, BtnItem3 };
            var texts = new[] { TxtItem1, TxtItem2, TxtItem3 };
            var itemIds = new[] { 6, 7, 8 };
            bool _showItem = false;
            for (int i = 0; i < itemIds.Length; i++)
            {
                int itemId = itemIds[i];
                var _rewardType = (SpecialRewardsType)itemId;
                string _sign = GameEnum.GetDescription(_rewardType);

                bool active = (takeItems.Contains(itemId) && CheckHaveItem(itemId))
                    || !CountDownTimerManager.Instance.IsTimerFinished(_sign);
                buttons[i].interactable = active;
                texts[i].text = active ? "1" : "0";

                if (active)
                {
                    if (CountDownTimerManager.Instance.IsTimerFinished(GameEnum.GetDescription(SpecialRewardsType.Unlimited_S_ChangeWater)))
                        stageModel.ReduceItem(itemIds[i], 1);
                }
                // 取真
                _showItem = _showItem | active;
            }

            if (!_showItem)
                BtnItemBg.Hide();

        }

        /// <summary>
        /// 下方道具栏道具更新
        /// </summary>
        private void SetItem()
        {
            stageModel = this.GetModel<StageModel>();
            BtnAddStepBack.gameObject.SetActive(stageModel.ItemDic[1] <= 0);
            TxtRefreshNum.text = stageModel.ItemDic[1].ToString();

            BtnAddRemove.gameObject.SetActive(stageModel.ItemDic[2] <= 0);
            TxtRemoveHideNum.text = stageModel.ItemDic[2].ToString();

            BtnAddAddBottle.gameObject.SetActive(stageModel.ItemDic[3] <= 0);
            TxtAddBottleNum.text = stageModel.ItemDic[3].ToString();

            BtnAddHalfBottle.gameObject.SetActive(stageModel.ItemDic[4] <= 0);
            TxtAddHalfBottleNum.text = stageModel.ItemDic[4].ToString();

            BtnAddRemoveBottle.gameObject.SetActive(stageModel.ItemDic[5] <= 0);
            TxtRemoveAllNum.text = stageModel.ItemDic[5].ToString();
        }

        /// <summary>
        /// 检查是否拥有道具
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        private bool CheckHaveItem(int itemID)
        {
            if (stageModel.ItemDic[itemID] > 0)
                return true;
            else return false;
        }

        #endregion

        private void OpenUIVictory()
        {
            ActionKit.Delay(0.5f, () =>
            {
                UIKit.ClosePanel<UIMask>();
                AudioKit.PlaySound("resources://Audio/Victory");
                UIKit.OpenPanel<UIVictory>();
            }).Start(this);
        }
    }
}
