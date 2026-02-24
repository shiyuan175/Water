using System;
using DG.Tweening;
using Game.Water;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Water
{
    public class UIGameNodeData : UIPanelData
    {
        public GlobalMechanism GlobalMechanism;
    }

    public partial class UIGameNode : UIPanel, IController, ICanSendEvent
    {
        private const int GET_THE_LAST_NUMBER_OF_LEVEL = 10;

        [Serializable]
        public struct DifficultyStyle
        {
            public Sprite TopBgSprites;
            public Sprite BottomSpirtes;
            public Sprite LevelTitleSprites;
            public Sprite ResetSprites;
            public Sprite SettingSprites;
        }

        [SerializeField]
        private MagicCtrl magicCtrl;
        public RectTransform CatPosition;

        [SerializeField] private Sprite[] mRankLevelSprites;

        #region 关卡难度UI

        //0简单 1中等 2困难
        [SerializeField] private DifficultyStyle[] mDifficultyStyles;
        [SerializeField] private Image imgTopBg;
        [SerializeField] private Image imgBottom;
        [SerializeField] private Image imgLevel;

        #endregion

        private GameGlobalModel gameGlobalModel;
        private int mCacheRankSpriteIndex;
        private bool mIsOpenUIVictory;

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

        protected override void OnInit(IUIData uiData = null)
        {

        }

        protected override void OnOpen(IUIData uiData = null)
        {
            gameGlobalModel = this.GetModel<GameGlobalModel>();
                
            BindBtn();
            RegisterEvent();
            #region 全局机制--魔法猫咪

            mData = uiData as UIGameNodeData ?? new UIGameNodeData();

            if (mData.GlobalMechanism == GlobalMechanism.WhiteMagicCar || mData.GlobalMechanism == GlobalMechanism.BlackMagicCar)
            {
                magicCtrl.Init(mData.GlobalMechanism);
            }

            #endregion
        }

        protected override void OnShow()
        {
            InitRankLevel();
            InitLevelUI();
            InitItemUI();
            SetItem();
            mIsOpenUIVictory = false;
            var globalMechanism = LevelManager.Instance.globalMechanism;
            if (globalMechanism == GlobalMechanism.WhiteMagicCar || globalMechanism == GlobalMechanism.BlackMagicCar)
            {
                magicCtrl.Show();
          
                magicCtrl.Init(globalMechanism);
            }
            else
            {
                magicCtrl.Hide();
            }

        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            gameGlobalModel = null;
        }

        private void BindBtn()
        {
            BtnRemoveAll.onClick.AddListener(BtnRemoveAllOnClick);
            
            BtnAddBottle.onClick.AddListener(BtnAddBottleOnClick);
            
            BtnHalfBottle.onClick.AddListener(BtnHalfBottleOnClick);
        
            BtnRemoveHide.onClick.AddListener(BtnRemoveHideOnClick);
            
            BtnStepBack.onClick.AddListener(BtnSetpBackOnClick);

            BtnReset.onClick.AddListener(BtnResetOnClick);
            BtnSetting.onClick.AddListener(() => 
            { 
                UIKit.OpenPanel<UISetting>(UILevel.PopUI);
            });
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
                SetItem();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<LevelStartEvent>(eventId =>
            {
                TxtLevel.text = LevelManager.Instance.levelId.ToString();
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

                var _tempWin = gameGlobalModel.InGameRankStreakWinNum;

                //飞星效果
                var curRankIndex = Mathf.Min(8, Mathf.Max(0, (_tempWin - 1) / 5));
                
                FlightEffects.Show();
                Vector3 originalPos = FlightEffects.transform.position;
                FlightEffects.DOMove(ImgRankLevel.transform.position, 1f) 
                .OnComplete(() =>
                {
                    TxtRankLevel.text = _tempWin.ToString();
                    // 回到原位
                    DOVirtual.DelayedCall(0.5f, () =>
                    {
                        FlightEffects.transform.position = originalPos;
                        FlightEffects.Hide(); // 可选：隐藏效果
                    });
                    //段位无晋升
                    if (curRankIndex <= mCacheRankSpriteIndex)
                    {
                        OpenUIVictory();
                        return;
                    }

                    ImgRankSprite_mid.sprite = mRankLevelSprites[mCacheRankSpriteIndex];
                    ImgRankSprite_mid.SetNativeSize();
                    SpineRankPromotion.Show();

                    SpineRankPromotion.AnimationState.SetAnimation(0, "animation", false);

                    ActionKit.Delay(0.5f, () =>
                    {
                        ImgRankSprite_mid.sprite =
                        mRankLevelSprites[curRankIndex];
                        ImgRankLevel.sprite =
                        mRankLevelSprites[curRankIndex];

                        ImgRankSprite_mid.SetNativeSize();
                    }).Start(this);

                    SpineRankPromotion.AnimationState.Complete += (trackEntry) =>
                    {
                        SpineRankPromotion.Hide();
                        
                        OpenUIVictory();
                    };
                });

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        #region UI初始化
        
        /// <summary>
        /// 修改难度UI
        /// </summary>
        private void InitLevelUI()
        {
            int level = this.GetUtility<SaveDataUtility>().GetCurrentLevel();
            TxtLevel.text = level.ToString();
            BtnReset.gameObject.SetActive(level > 5);

            if (level < GET_THE_LAST_NUMBER_OF_LEVEL) return;

            int _index = (level % GET_THE_LAST_NUMBER_OF_LEVEL) switch
            {
                (int)LevelHardType.Hard => 1,
                (int)LevelHardType.VeryHand => 2,
                _ => 0
            };

            if (_index != 0)
                SetTextTip();

            imgTopBg.sprite = mDifficultyStyles[_index].TopBgSprites;
            imgLevel.sprite = mDifficultyStyles[_index].LevelTitleSprites;
            imgBottom.sprite = mDifficultyStyles[_index].BottomSpirtes;
            BtnReset.image.sprite = mDifficultyStyles[_index].ResetSprites;
            BtnSetting.image.sprite = mDifficultyStyles[_index].SettingSprites;
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
                var _tempWin = gameGlobalModel.InGameRankStreakWinNum;
                TxtRankLevel.text = _tempWin.ToString();
                //5次连胜晋升一个段位,总段位数9(起始0)
                mCacheRankSpriteIndex = Mathf.Min(8, Mathf.Max(0, (_tempWin - 1) / 5));
                ImgRankLevel.sprite = mRankLevelSprites[mCacheRankSpriteIndex];
            }

            else ImgRankLevel.Hide();
        }

        #endregion

        #region 付费道具相关

        /// <summary>
        /// 显示道具图标
        /// </summary>
        private void InitItemUI()
        {
            int level = this.GetUtility<SaveDataUtility>().GetCurrentLevel();

            if (level > (int)GameConst.NEWBIE_LEVEL_COUNT)
                UnLockItem(NormalRewardsType.AddHalfBottle);

            if (level > (int)GameConst.NEWBIE_LEVEL_COUNT)
                UnLockItem(NormalRewardsType.AddOneBottle);

            if (level > (int)GameConst.NEWBIE_LEVEL_COUNT)
                UnLockItem(NormalRewardsType.StepBack);

            if (level > (int)UIGuideLevel.UIGuideLevelRemoveHide)
                UnLockItem(NormalRewardsType.RemoveHide);
            else if (level > (int)GameConst.NEWBIE_LEVEL_COUNT)
            {
                BtnRemoveHide.transform.Find("ImgLock").Show();
            }

            if (level > (int)UIGuideLevel.UIGuideLevelRemoveAll)
                UnLockItem(NormalRewardsType.RemoveAll);
            else if (level > (int)GameConst.NEWBIE_LEVEL_COUNT)
            {
                BtnRemoveAll.transform.Find("ImgLock").Show();
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
                case NormalRewardsType.AddHalfBottle:
                    transform = BtnHalfBottle.transform;
                    break;

                case NormalRewardsType.AddOneBottle:
                    transform = BtnAddBottle.transform;
                    break;

                case NormalRewardsType.StepBack:
                    transform = BtnStepBack.transform;
                    break;

                case NormalRewardsType.RemoveHide:
                    transform = BtnRemoveHide.transform;
                    transform.Find("ImgLock").Hide();
                    break;

                case NormalRewardsType.RemoveAll:
                    transform = BtnRemoveAll.transform;
                    transform.Find("ImgLock").Hide();
                    break;
            }

            transform.Find("ImgItem").Show();
            transform.Find("ItemNumBg").Show();
            transform.GetComponent<Button>().interactable = true;
            transform.Find("ImgItem").GetComponent<Image>().color = Color.white;
        }

        /// <summary>
        /// 下方道具栏UI更新
        /// </summary>
        private void SetItem()
        {
            var level = this.GetUtility<SaveDataUtility>().GetCurrentLevel();
            gameGlobalModel = this.GetModel<GameGlobalModel>();
            BtnAddStepBack.gameObject.SetActive(gameGlobalModel.ItemDic[1] <= 0 && BtnStepBack.transform.Find("ItemNumBg").gameObject.activeSelf);
            TxtRefreshNum.text = gameGlobalModel.ItemDic[1].ToString();

            BtnAddRemove.gameObject.SetActive(gameGlobalModel.ItemDic[2] <= 0 && BtnRemoveHide.transform.Find("ItemNumBg").gameObject.activeSelf);
            TxtRemoveHideNum.text = gameGlobalModel.ItemDic[2].ToString();

            BtnAddAddBottle.gameObject.SetActive(gameGlobalModel.ItemDic[3] <= 0 && BtnAddBottle.transform.Find("ItemNumBg").gameObject.activeSelf);
            TxtAddBottleNum.text = gameGlobalModel.ItemDic[3].ToString();

            BtnAddHalfBottle.gameObject.SetActive(gameGlobalModel.ItemDic[4] <= 0 && BtnHalfBottle.transform.Find("ItemNumBg").gameObject.activeSelf);
            TxtAddHalfBottleNum.text = gameGlobalModel.ItemDic[4].ToString();

            BtnAddRemoveBottle.gameObject.SetActive(gameGlobalModel.ItemDic[5] <= 0 && BtnRemoveAll.transform.Find("ItemNumBg").gameObject.activeSelf);
            TxtRemoveAllNum.text = gameGlobalModel.ItemDic[5].ToString();
        }

        private void BtnSetpBackOnClick()
        {
            if (!LevelManager.Instance.isPlayFxAnim && GameCtrl.Instance.IsPouring)
            {
                if (gameGlobalModel.ItemDic[1] <= 0)
                {
                    TopOnADManager.Instance.ShowVideoAd(() => { gameGlobalModel.AddItem(1, 1); }, null);
#if UNITY_EDITOR
                    Debug.Log("模拟广告");
                    gameGlobalModel.AddItem(1, 5);
#endif
                    return;
                }
                if (LevelManager.Instance.ReturnLast())
                    gameGlobalModel.ReduceItem(1, 1);
            }
        }
        private void BtnRemoveHideOnClick()
        {
            if (!LevelManager.Instance.isPlayFxAnim && GameCtrl.Instance.IsPouring)
            {
                if (gameGlobalModel.ItemDic[2] <= 0)
                {
                    /*UIBuyItemData data = new UIBuyItemData() { item = 2 };
                    UIKit.OpenPanel<UIBuyItem>(data);*/
                    TopOnADManager.Instance.ShowVideoAd(() => { gameGlobalModel.AddItem(2, 1); }, null);
#if UNITY_EDITOR
                    Debug.Log("模拟广告");
                    gameGlobalModel.AddItem(2, 1);
#endif
                    return;
                }

                //判断是否有黑水瓶
                if (LevelManager.Instance.hideBottleList.Count != 0)
                {
                    LevelManager.Instance.RemoveHide(() =>
                    {
                        gameGlobalModel.ReduceItem(2, 1);
                    });
                }
            }
        }
        private void BtnAddBottleOnClick()
        {
            if (!LevelManager.Instance.isPlayFxAnim && GameCtrl.Instance.IsPouring)
            {
                if (gameGlobalModel.ItemDic[3] <= 0)
                {
                    TopOnADManager.Instance.ShowVideoAd(() => { gameGlobalModel.AddItem(3, 1); }, null);
#if UNITY_EDITOR
                    Debug.Log("模拟广告");
                    gameGlobalModel.AddItem(3, 1);
#endif
                    return;
                }
                LevelManager.Instance.AddBottle(false, () =>
                {
                    gameGlobalModel.ReduceItem(3, 1);
                });
            }
        }
        private void BtnHalfBottleOnClick()
        {
            if (!LevelManager.Instance.isPlayFxAnim && GameCtrl.Instance.IsPouring)
            {
                if (gameGlobalModel.ItemDic[4] <= 0)
                {
                    TopOnADManager.Instance.ShowVideoAd(() => { gameGlobalModel.AddItem(4, 1); }, null);
#if UNITY_EDITOR
                    Debug.Log("模拟广告");
                    gameGlobalModel.AddItem(4, 1);
#endif
                    return;
                }
                LevelManager.Instance.AddBottle(true, () =>
                {
                    gameGlobalModel.ReduceItem(4, 1);
                });
            }
        }
        private void BtnRemoveAllOnClick()
        {
            if (!LevelManager.Instance.isPlayFxAnim && GameCtrl.Instance.IsPouring)
            {
                if (gameGlobalModel.ItemDic[5] <= 0)
                {
                    TopOnADManager.Instance.ShowVideoAd(() => { gameGlobalModel.AddItem(5, 1); }, null);
#if UNITY_EDITOR
                    Debug.Log("模拟广告");
                    gameGlobalModel.AddItem(5, 1);
#endif
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

                                for (int i = 0; i < record.HideWaterTypes.Count; i++)
                                {
                                    record.HideWaterTypes[i] = HideWaterType.None;
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
                    gameGlobalModel.ReduceItem(5, 1);
                }
            }
        }
        #endregion

        private void BtnResetOnClick()
        {
            TopOnADManager.Instance.ShowIntersAd(null, () =>
            {
                StartCoroutine(LevelManager.Instance.AdRewardCoroutine());
            });
#if UNITY_EDITOR
            Debug.Log("模拟广告");
            this.GetModel<GameGlobalModel>().ResetCountinueWinNum();
            LevelManager.Instance.StartGame(this.GetUtility<SaveDataUtility>().GetCurrentLevel());
            if (UIKit.GetPanel<UIMask>())
                UIKit.ClosePanel<UIMask>();
#endif
        }

        private void OpenUIVictory()
        {
            if (!mIsOpenUIVictory)
            {
                ActionKit.Delay(0.5f, () =>
                {
                    if (UIKit.GetPanel<UIMask>())
                        UIKit.ClosePanel<UIMask>();
                    var sound = AudioKit.PlaySound("resources://Audio/Victory",volume: 0.7f);
                    UIKit.OpenPanel<UIVictory>();
                }).Start(this);
            }

            mIsOpenUIVictory = true;
        }
    }
}
