using UnityEngine;
using UnityEngine.UI;
using QFramework;
using System;
using GameDefine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine.U2D;
using TMPro;


namespace QFramework.Example
{
    public class UIGameNodeData : UIPanelData
    {
        public GlobalMechanism GlobalMechanism;
        public int BeginSetp = 0;
        public int CanUseSetps = 0;
    }

    public partial class UIGameNode : UIPanel, IController
    {
        [SerializeField]
        private MagicCtrl magicCtrl;
        private const string ITEM_ENTRANCE_EFFECT_PATH = "Prefab/ItemEntranceEffect";
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
        [SerializeField] private Image mImgProgressBar;
        [SerializeField] private TextMeshProUGUI mStoryTxt;
        #endregion

        [Header("付费道具解锁UI")]
        [SerializeField] private TextMeshProUGUI mTxtAddHalfBottle;
        [SerializeField] private TextMeshProUGUI mTxtAddBottle;
        [SerializeField] private TextMeshProUGUI mTxtStepBack;
        [SerializeField] private TextMeshProUGUI mTxtRemoveHide;
        [SerializeField] private TextMeshProUGUI mTxtRemoveAll;


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
            #region 全局机制--魔法猫咪
            if (mData.GlobalMechanism == GlobalMechanism.WhiteMagicCar || mData.GlobalMechanism == GlobalMechanism.BlackMagicCar)
            {          
                magicCtrl.Init(mData.GlobalMechanism);
            }
            #endregion
            // please add init code here
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            stageModel = this.GetModel<StageModel>();

            LoadRes();
            BindBtn();
            RegisterEvent();
            ConsumeTakeItems();

            AutoUseAllItems();
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

        #region UI初始化

        /// <summary>
        /// 前五关故事UI
        /// </summary>
        private void InitStoryUI()
        {
            int _curLevel = this.GetUtility<SaveDataUtility>().GetCurrentLevel();
            if (_curLevel > GameDefine.GameConst.NEWBIE_LEVEL_COUNT)
                return;

            if (!g_Star_MagicBook_Guide.activeSelf)
            {
                g_Star_MagicBook_Guide.Show();
                mStoryTxt.font = LevelManager.Instance.greenFont;

                mImgProgressBar.fillAmount = (float)(_curLevel) / GameConst.NEWBIE_LEVEL_COUNT;
                mStoryTxt.text = $"Story {_curLevel}/{GameConst.NEWBIE_LEVEL_COUNT}";
            }
            else
            {
                var fillamount = (float)(_curLevel) / GameConst.NEWBIE_LEVEL_COUNT;
                mImgProgressBar.DOFillAmount(fillamount, 1f);
                mStoryTxt.text = $"Story {_curLevel}/{GameConst.NEWBIE_LEVEL_COUNT}";
            }
        }

        /// <summary>
        /// 修改难度UI
        /// </summary>
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

        /// <summary>
        /// 困难关卡弹窗UI
        /// </summary>
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
        /// 段位UI
        /// </summary>
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

        /// <summary>
        /// 扣除携带道具数量
        /// </summary>
        private void ConsumeTakeItems()
        {
            var takeItems = LevelManager.Instance.takeItem;
            var itemIds = new[] 
            {
                NormalRewardsType.S_AddOneHalfBottle,
                NormalRewardsType.S_RemoveOneBottleHideWater,
                NormalRewardsType.S_RemoveOneDebuffBottle
            };

            for (int i = 0; i < itemIds.Length; i++)
            {
                string _sign = GameEnum.GetDescription(itemIds[i]);

                bool _isTakeItem = (takeItems.Contains((int)itemIds[i]) && (stageModel.ItemDic[(int)itemIds[i]] > 0));
                if (_isTakeItem && CountDownTimerManager.Instance.IsTimerFinished(_sign))
                    stageModel.ReduceItem((int)itemIds[i], 1);
            }
        }

        #endregion

        #region 付费道具相关

        /// <summary>
        /// 显示道具图标
        /// </summary>
        private void InitItemUI()
        {
            int level = this.GetUtility<SaveDataUtility>().GetCurrentLevel();

            if (level >= (int)GameDefine.UIGuideLevel.UIGuideLevelAddBottle)
                UnLockItem(NormalRewardsType.AddOneBottle);
            else
            {
                mTxtAddBottle.font = LevelManager.Instance.redFont;
                mTxtAddBottle.text = $"{(int)UIGuideLevel.UIGuideLevelAddBottle}";
            }

            if (level >= (int)GameDefine.UIGuideLevel.UIGuideLevelHalfBottle)
                UnLockItem(NormalRewardsType.AddHalfBottle);
            else
            {
                mTxtAddHalfBottle.font = LevelManager.Instance.redFont;
                mTxtAddHalfBottle.text = $"{(int)UIGuideLevel.UIGuideLevelHalfBottle}";
            }

            if (level > (int)GameDefine.UIGuideLevel.UIGuideLevelRemoveHide)
                UnLockItem(NormalRewardsType.RemoveHide);
            else
            {
                mTxtRemoveHide.font = LevelManager.Instance.redFont;
                mTxtRemoveHide.text = $"{(int)UIGuideLevel.UIGuideLevelRemoveHide}";
            }

            if (level > (int)GameDefine.UIGuideLevel.UIGuideLevelRemoveAll)
                UnLockItem(NormalRewardsType.RemoveAll);
            else
            {
                mTxtRemoveAll.font = LevelManager.Instance.redFont;
                mTxtRemoveAll.text = $"{(int)UIGuideLevel.UIGuideLevelRemoveAll}";
            }

            if (level >= (int)GameDefine.UIGuideLevel.UIGuideLevelStepBack)
                UnLockItem(NormalRewardsType.StepBack);
            else
            {
                mTxtStepBack.font = LevelManager.Instance.redFont;
                mTxtStepBack.text = $"{(int)UIGuideLevel.UIGuideLevelStepBack}";
            }
        }

        /// <summary>
        /// 道具解锁
        /// </summary>
        /// <param name="PropType"></param>
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

        /// <summary>
        /// 下方道具栏UI更新
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
        #endregion

        #region 时序动作
        // 异步队列
        private readonly Queue<Action<Action>> mActionQueue = new();
        private bool mIsRunning = false;

        private void EnqueueAction(Action<Action> action)
        {
            mActionQueue.Enqueue(action);
            TryRunNext();
        }

        private void TryRunNext()
        {
            if (mIsRunning || mActionQueue.Count == 0) return;

            mIsRunning = true;
            var action = mActionQueue.Dequeue();
            action.Invoke(() =>
            {
                mIsRunning = false;
                TryRunNext();
                //ActionKit.Delay(0.3f, () =>
                //{
                   
                //}).Start(this);
            });
        }
        
        private void AutoUseAllItems()
        {
            var level = this.GetUtility<SaveDataUtility>().GetCurrentLevel();

            if (level >= (int)GameDefine.UnLockMechanism.RemoveHideWinStreakLevel
                && stageModel.RemoveHideStreakWinNum >= GameConst.TEN_CONTINUE_WIN_NUM)
            {
                EnqueueAction(_nextItem =>
                {
                    //Debug.Log("连胜去黑生效");
                    StreaWinClearBWater(_nextItem);
                });
            }

            if (LevelManager.Instance.takeItem.Contains((int)NormalRewardsType.S_RemoveOneBottleHideWater)
                && level >= (int)UnLockMechanism.S_RemoveOneBottleHideWater)
            {
                EnqueueAction(_nextItem =>
                {
                    //Debug.Log("去黑道具生效");
                    RemoveOneBottleHideWater(_nextItem);
                });
            }

            if (LevelManager.Instance.takeItem.Contains((int)NormalRewardsType.S_RemoveOneDebuffBottle)
                && level >= (int)UnLockMechanism.S_RemoveOneDebuffBottle)
            {
                EnqueueAction(_nextItem =>
                {
                    //Debug.Log("去Debuff道具生效");
                    RemoveOneDebuffBottle(_nextItem);
                });
            }

            if (LevelManager.Instance.takeItem.Contains((int)NormalRewardsType.S_AddOneHalfBottle)
                && level >= (int)UnLockMechanism.S_AddOneHalfBottle)
            {
                EnqueueAction(_nextItem =>
                {
                    //Debug.Log("增加瓶子生效");
                    AddOneHalfBottle(_nextItem);
                });
            }
        }

        #endregion

        #region 携带道具相关

        /// <summary>
        /// 祛除瓶中所有黑水
        /// </summary>
        /// <param name="useItem">是否由道具生效</param>
        /// <param name="action"></param>
        private void ClearBottleBlackWater(bool useItem, Action action = null)
        {
            if (LevelManager.Instance.hideBottleList.Count > 0)
            {
                //剔除魔法布和陶瓷瓶的瓶子
                var _tempList = new List<BottleCtrl>(LevelManager.Instance.hideBottleList);
                _tempList.RemoveAll(item => item.isClearHide || item.isNearHide);

                if (_tempList.Count == 0)
                {
                    action?.Invoke();
                    return;
                }

                int _removeCount = useItem ? 1 : _tempList.Count / 2;
                //特判处理(只有一个黑水瓶)
                _removeCount = Math.Min(_removeCount, _tempList.Count);

                while (_tempList.Count > _removeCount)
                {
                    int randIndex = UnityEngine.Random.Range(0, _tempList.Count);
                    _tempList.RemoveAt(randIndex);
                }

                foreach (var item in _tempList)
                {
                    LevelManager.Instance.hideBottleList.Remove(item);
                    item.StarSetHideShow();
                }
                action?.Invoke();
            }
            else
                action?.Invoke();
        }

        /// <summary>
        /// 连胜去黑
        /// </summary>
        /// <param name="tempList"></param>
        /// <param name="action"></param>
        private void StreaWinClearBWater(Action action)
        {
            PlayParticleEffect(() =>
            {
                ClearBottleBlackWater(false, () =>
                {
                    action?.Invoke();
                });
            });
        }

        /// <summary>
        /// 去除一瓶黑水
        /// </summary>
        /// <param name="onComplete"></param>
        private void RemoveOneBottleHideWater(Action onComplete)
        {
            var _sprite = RewardUIManager.Instance.GetRewardSprite(NormalRewardsType.S_RemoveOneBottleHideWater);
            PlayParticleEffect(() =>
            {
                ClearBottleBlackWater(true, () =>
                {
                    // 队列通知动作完成
                    onComplete?.Invoke();
                });
            }, _sprite);
        }
       
        /// <summary>
        /// 增加一格瓶子
        /// </summary>
        /// <param name="onComplete"></param>
        private void AddOneHalfBottle(Action onComplete)
        {
            var _sprite = RewardUIManager.Instance.GetRewardSprite(NormalRewardsType.S_AddOneHalfBottle); 
            void _changeRainBowWater(Action callback)
            {
                var _tempWater = new List<int>(LevelManager.Instance.clearList);
                //1、移除药水需要消除的颜色
                var changeColors = LevelManager.Instance.nowLevel.changeList.Select(x => x.NeedChangeColor);
                _tempWater = _tempWater.Except(changeColors).ToList();
                //2、移除需要消除多次颜色
                _tempWater = _tempWater.GroupBy(x => x).Where(g => g.Count() == 1).Select(g => g.Key).ToList();
                //3、移除魔法布的颜色 和 限制瓶颜色
                var hideColors = LevelManager.Instance.nowBottles
                    .Where(b => b.isClearHide || b.limitColor > 0)
                    .Select(b => b.isClearHide ? b.unlockClear : b.limitColor);
                _tempWater = _tempWater.Except(hideColors).ToList();

                //4、取随机颜色
                var _colorIdx = _tempWater[UnityEngine.Random.Range(0, _tempWater.Count)];
                LevelManager.Instance.clearList.Remove(_colorIdx);

                //遍历有这个颜色的瓶子，执行方法
                foreach (var bottle in LevelManager.Instance.nowBottles)
                {
                    if (bottle.waters.Contains(_colorIdx))
                    {
                        bottle.ChangeWaterToRainBowWater(_colorIdx);
                    }
                }

                callback?.Invoke();
            }

            PlayParticleEffect(() => _changeRainBowWater(onComplete), _sprite);
        }

        /// <summary>
        /// 移除一个瓶子的负面状态
        /// </summary>
        /// <param name="botter"></param>
        private void RemoveOneDebuffBottle(Action onComplete)
        {
            var _sprite = RewardUIManager.Instance.GetRewardSprite(NormalRewardsType.S_RemoveOneDebuffBottle);
            void _removeDeuff(Action callback)
            {
                var _tempbottle = new List<BottleCtrl>(LevelManager.Instance.nowBottles);
                BottleCtrl _bottle = null;
                while (_tempbottle.Count != 0)
                {
                    var _randomIndex = UnityEngine.Random.Range(0, _tempbottle.Count);
                    _bottle = _tempbottle[_randomIndex];
                    if (_bottle.CheckDebuff())
                        break;
                    else
                        _tempbottle.RemoveAt(_randomIndex);
                }

                _bottle?.SetNormal(true);
                callback?.Invoke();
            }

            PlayParticleEffect(() => _removeDeuff(onComplete), _sprite);
            /*#region 原打乱水块功能
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
            #endregion*/
        }

        /// <summary>
        /// 道具入场动画
        /// </summary>
        /// <param name="action"></param>
        private void PlayParticleEffect(Action action ,Sprite sprite = null)
        {
            var _particle = Resources.Load(ITEM_ENTRANCE_EFFECT_PATH);
            var _tempObj = Instantiate(_particle) as GameObject;

            if (sprite != null)
                _tempObj.GetComponent<SpriteRenderer>().sprite = sprite;

            //UIKit.OpenPanel<UIMask>(UILevel.PopUI);//遮罩
            _tempObj.transform.DOLocalMoveY(0, 1f);
            _tempObj.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 1f)
            .OnComplete(() =>
            {
                action?.Invoke();
                Destroy(_tempObj);
            });
        }

        private void OpenUIVictory()
        {
            ActionKit.Delay(0.5f, () =>
            {
                UIKit.ClosePanel<UIMask>();
                AudioKit.PlaySound("resources://Audio/Victory");
                UIKit.OpenPanel<UIVictory>();
            }).Start(this);
        }
        #endregion
    }
}
