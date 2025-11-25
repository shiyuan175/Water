using DG.Tweening;
using GameDefine;
using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace QFramework.Example
{
    public class UIBeginData : UIPanelData
    {
    }
    public partial class UIBegin : UIPanel, ICanRegisterEvent, ICanGetUtility, ICanGetModel
    {
        public ParticleTargetMoveCtrl coinFx, starFx;
        [SerializeField] private Sprite[] btnStartSprites;

        private const int BPIndex = 1;

        #region BottomMenuSetting
        [Header("底部菜单按钮UI")]
        [SerializeField] private List<GameObject> Panels;
        [SerializeField] private List<LayoutElement> mLayoutElements_BgFrame;
        [SerializeField] private List<Button> mMenuBtn;
        [SerializeField] private RectTransform mSelected;
        private GameObject HomeNode => Panels[2];
        private List<LayoutElement> mLayoutElements_MenuBtn;
        private List<RectTransform> mImgsRect;
        // 初始位置
        private float mInitPosY;
        // 当前选中索引(-1表示未选中)
        private int mCurSelectIndex = -1;
       
        #endregion

        [SerializeField] private GameObject[] mSceneUnlockPanels;

        private StageModel stageModel;
        private SaveDataUtility saveData;
        private SceneUnlockModel mSceneUnlockModel;
        private VolcanicActivity mVolcanicActivity;
        private RocketActivity mRocketActivity;
        private HighTowerActivity mHighTowerActivity;
        private MagicStreakActivity mMagicStreakActivity;
        private TierRankActivity mTierRankActivity;
        private TurnTableADActivity mTurnTableADActivity;
        private SepecialOfferADActivity mSepecialOfferADActivity;
        private PrograssGiftADActivity mPrograssGiftADActivity;
        private DuobleGiftAdActivity mDoubleGiftADAcitvity;
        private BannerActivity mBannerActivity;
        private GameObject mCurBannerActivity;


        
        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UIBeginData ?? new UIBeginData();
            // please add init code here
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            //真机模式下，AssetBundle 加载资源后需要关联材质
            //TxtImgprogress.font.material.shader = Shader.Find(TxtImgprogress.font.material.shader.name);
            stageModel = this.GetModel<StageModel>();
            saveData = this.GetUtility<SaveDataUtility>();
            mSceneUnlockModel = this.GetModel<SceneUnlockModel>();
            mVolcanicActivity = GameActivityManager.Instance.GetActivity<VolcanicActivity>();
            mRocketActivity = GameActivityManager.Instance.GetActivity<RocketActivity>();
            mHighTowerActivity = GameActivityManager.Instance.GetActivity<HighTowerActivity>();
            mMagicStreakActivity = GameActivityManager.Instance.GetActivity<MagicStreakActivity>();
            mTierRankActivity = GameActivityManager.Instance.GetActivity<TierRankActivity>();
            mTurnTableADActivity = GameActivityManager.Instance.GetActivity<TurnTableADActivity>();
            mSepecialOfferADActivity = GameActivityManager.Instance.GetActivity<SepecialOfferADActivity>();
            mPrograssGiftADActivity = GameActivityManager.Instance.GetActivity<PrograssGiftADActivity>();
            mDoubleGiftADAcitvity = GameActivityManager.Instance.GetActivity<DuobleGiftAdActivity>();

            LevelManager.Instance.InitBottle();

            LoaderRes();
            BindBtn();
            InitBottomMenu();
            RegisterEvent();
            InitSceneUI();
            ShowActivityState();
            InitActivityState();

            GameUtilityManager.Instance.RegisterTask(this, UpdateUI);

            if (saveData.GetCurrentLevel() <= GameConst.NEWBIE_LEVEL_COUNT)
            {
                BottomMenuNode.Hide();
                HomeNode.Hide();
            }
        }

        protected override void OnShow()
        {
            //BindBtn();
            //RegisterEvent();
            //InitSceneUI();
            //InitActivityState();
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            
        }

        private void BindBtn()
        {
            BtnStart.onClick.RemoveAllListeners();
            BtnStart.onClick.AddListener(TryStartGame);

            BtnHeart.onClick.RemoveAllListeners();
            BtnHeart.onClick.AddListener(() =>
            {
                UIKit.OpenPanel<UIMoreLife>();
            });

            BtnArea.onClick.RemoveAllListeners();
            BtnArea.onClick.AddListener(() =>
            {
                string _sceneName;
                if(mSceneUnlockModel.SceneIndex >= GameConst.SceneUnlock.Count)
                {
                    int maxKey = GameConst.SceneUnlock.Keys.Max();
                    _sceneName = GameConst.SceneUnlock[maxKey];
                }
                else
                {
                    _sceneName = GameConst.SceneUnlock[mSceneUnlockModel.SceneIndex];
                }
                this.gameObject.Hide();
                UIKit.OpenPanel(_sceneName);
            });

            BtnHead.onClick.RemoveAllListeners();
            BtnHead.onClick.AddListener(() =>
            {
                UIKit.OpenPanel("UIPersonal");
            });

            BtnCoin.onClick.RemoveAllListeners();
            BtnCoin.onClick.AddListener(() =>
            {
                //跳转商店
                InitBeginMenuButton(0);
            });

            BtnVANode.onClick.RemoveAllListeners();
            BtnVANode.onClick.AddListener(() =>
            {
                if (!mVolcanicActivity.VAActivateState)
                    UIKit.OpenPanel<UIVolcanicActivityEntrance>();
                else
                    UIKit.OpenPanel<UIVolcanicActivity>();
            });

            BtnRANode.onClick.RemoveAllListeners();
            BtnRANode.onClick.AddListener(() =>
            {
                if (!GameUtils.DoesCountDownKeyExist(GameConst.ROCKET_ACTIVITY_SIGN))
                    UIKit.OpenPanel<UIRocketActivityEntrance>();
                else
                    UIKit.OpenPanel<UIRocketActivity>();
            });

            BtnHTANode.onClick.RemoveAllListeners();
            BtnHTANode.onClick.AddListener(() =>
            {
                if (!GameUtils.DoesCountDownKeyExist(GameConst.HIGH_TOWER_ACTIVITY_SIGN))
                    UIKit.OpenPanel<UIHighTowerActivityEntrance>();
                else
                    UIKit.OpenPanel<UIHighTowerActivity>();
            });

            BtnMSANode.onClick.RemoveAllListeners();
            BtnMSANode.onClick.AddListener(() =>
            {
                if (!GameUtils.DoesCountDownKeyExist(GameConst.MAGIC_STREAK_ACTIVITY_SIGN))
                    UIKit.OpenPanel<UIMagicStreakActivityEntrance>();
                else
                    UIKit.OpenPanel<UIMagicStreakActivity>();
            });

            BtnTTNode.onClick.RemoveAllListeners();
            BtnTTNode.onClick.AddListener(() =>
            {
                UIKit.OpenPanel<UIMallTurntable>();
            });
            BtnBPNode.onClick.RemoveAllListeners();
            BtnBPNode.onClick.AddListener(() =>
            {
                MenuBtnEvent(BPIndex);
            });

            BtnPGNode.onClick.RemoveAllListeners();
            BtnPGNode.onClick.AddListener(() =>
            {
                UIKit.OpenPanel<UIPrograssGiftADActivity>();
            });
            BtnSONode.onClick.RemoveAllListeners();
            BtnSONode.onClick.AddListener(() =>
            {
                UIKit.OpenPanel<UISepecialOfferGift>();
            });

            BtnDGNode.onClick.RemoveAllListeners();
            BtnDGNode.onClick.AddListener(() =>
            {
                UIKit.OpenPanel<UIDoubleGiftADActivity>();
            });

            BtnRemoveADNode.onClick.RemoveAllListeners();
            BtnRemoveADNode.onClick.AddListener(() =>{
                UIKit.OpenPanel<UIRemoveAdADActivity>();
            });
            BtnTRANode.onClick.RemoveAllListeners();
            BtnTRANode.onClick.AddListener(() =>
            {
                var _activityStatus = mTierRankActivity.ActivityStatus;
                var _openRankPanel = false;

                switch (_activityStatus)
                {
                    case SettlementActivityStatus.Inactive:
                        _openRankPanel = false;
                        break;

                    case SettlementActivityStatus.Active:
                        _openRankPanel = true;
                        break;

                    case SettlementActivityStatus.Finished:
                        if (!mTierRankActivity.TRAData.Player.IsRewardSettled)
                            _openRankPanel = true;
                        else
                            _openRankPanel = false;
                        break;
                }
            
                if (_openRankPanel)
                    UIKit.OpenPanel<UITierRankActivity>();
                else
                    UIKit.OpenPanel<UITierRankActivityEntrance>();
            });

            //底部区域按钮监听
            mMenuBtn.ForEach(btn => btn.onClick.RemoveAllListeners());
            for (int i = 0; i < mMenuBtn.Count; i++)
            {
                int _btnIdx = i;
                mMenuBtn[_btnIdx].onClick.AddListener(() =>
                {
                    MenuBtnEvent(_btnIdx);
                });
            }
        }

        private void RegisterEvent()
        {
            this.RegisterEvent<ReturnToMainEvent>(e =>
            {
                LevelManager.Instance.InitBottle();
                BottomMenuNode.Show();
                HomeNode.Show();

                //重启会使Aspect Ratio Fitter 将UI重新布局(等待一帧刷新)
                ActionKit.DelayFrame(1, () =>
                {
                    mImgsRect[2].DOAnchorPosY(mInitPosY + 60f, 0);
                }).Start(this);

                SetStartLevel();
                if (e.PassLevel)
                {
                    StartCoroutine(ShowFx());
                    ShowActivityState();

                    if (mSceneUnlockModel.SceneIndex == 0 && mSceneUnlockModel.SceneUnlockUnitIndex == 0)
                        UIKit.OpenPanel<SceneUnlockGuide>(UILevel.PopUI);
                }

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<VitalityChangeEvent>(e =>
            {
                SetVitality();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<UnlockSceneBackEvent>(e =>
            {
                this.gameObject.Show();
                mImgsRect[2].DOAnchorPosY(mInitPosY + 60f, 0);
                SetScene();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<GameStartEvent>(e =>
            {
                LevelManager.Instance.StartGame(saveData.GetCurrentLevel());
                UIKit.OpenPanel<UIGameNode>(new UIGameNodeData { globalMechanism = LevelManager.Instance.globalMechanism });
                
                BottomMenuNode.Hide();
                HomeNode.Hide();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<AvatarEvent>(e =>
            {
                BtnHead.GetComponent<Image>().sprite = AvatarManager.Instance.GetAvatarSprite(true, e.AvatarId);
                ImgHeadFrame.sprite = AvatarManager.Instance.GetAvatarSprite(false, e.AvatarFrameId);
            }).UnRegisterWhenGameObjectDestroyed(this);

            this.RegisterEvent<OnActivityStatusChanged>(e =>
            {
                ActivityStatusChangeEvent(e);

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            StringEventSystem.Global.Register(GameConst.OPEN_SHOP_PANEL_EVENT, () =>
            {
                InitBeginMenuButton(0);

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            StringEventSystem.Global.Register(GameConst.START_POTION_ACTIVITY, () =>
            {
                RegisterBannerActivity();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            StringEventSystem.Global.Register(GameConst.COIN_CHANGE, () =>
            {
                SetCoin();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
      
            StringEventSystem.Global.Register(GameConst.UNLOCK_NEW_SCENES, () =>
            {
                SetScene();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            StringEventSystem.Global.Register(GameConst.SCENE_UNLOCK_GUIDE_STEP1, () =>
            {
                mMenuBtn.Last().onClick.Invoke();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void TryStartGame()
        {
            if (mTierRankActivity != null)
            {
                var _activityStatus = mTierRankActivity.ActivityStatus;
                if (_activityStatus == SettlementActivityStatus.Inactive)
                {
                    UIKit.OpenPanel<UITierRankActivityEntrance>();
                    return;
                }

                if (_activityStatus == SettlementActivityStatus.Finished
                    && !mTierRankActivity.TRAData.Player.IsRewardSettled)
                {
                    UIKit.OpenPanel<UITierRankActivity>();
                    return;
                }
            }
            
            UIKit.OpenPanel<UIBeginSelect>();
        }

        #region 底部菜单栏按钮切换

        private void MenuBtnEvent(int index)
        {
            if (mCurSelectIndex == index) return;

            ChangePanel(index);

            // 还原上一个
            if (mCurSelectIndex >= 0 && mCurSelectIndex < mImgsRect.Count)
            {
                var _prevImg = mImgsRect[mCurSelectIndex];
                _prevImg.DOScale(0.8f, 0.1f);
                _prevImg.DOAnchorPosY(mInitPosY, 0.1f);
                mLayoutElements_BgFrame[mCurSelectIndex].flexibleWidth = 1f;
                mLayoutElements_MenuBtn[mCurSelectIndex].flexibleWidth = 1f;
            }

            // 当前点击动画
            var _curImg = mImgsRect[index];
            _curImg.localScale = Vector3.one * 0.5f;
            _curImg.DOScale(1f, 0.1f);
            _curImg.DOAnchorPosY(mInitPosY + 60f, 0.1f); 
            mLayoutElements_BgFrame[index].flexibleWidth = 1.2f;
            mLayoutElements_MenuBtn[index].flexibleWidth = 1.2f;

            //延迟一帧等待 Layout 刷新,然后更新选中块
            ActionKit.DelayFrame(1, () =>
            {
                var _rect = mLayoutElements_BgFrame[index].GetComponent<RectTransform>();
                mSelected.DOMove(_rect.position, 0.2f);
            }).Start(this);
            mCurSelectIndex = index;
        }

        private void InitBottomMenu()
        {
            mImgsRect = mMenuBtn.Select(btn => btn.transform.GetChild(0).GetComponent<RectTransform>())
            .ToList();
            mLayoutElements_MenuBtn = mMenuBtn.Select(btn => btn.GetComponent<LayoutElement>())
            .ToList();
            mInitPosY = mImgsRect[0].anchoredPosition.y;

            //初始化选择主城按钮(索引为2)
            InitBeginMenuButton(2);

            Canvas.ForceUpdateCanvases();
            foreach (var rt in mImgsRect)
            {
                rt.GetComponent<AspectRatioFitter>().Disable();
                var temp = rt.rect.size;
                rt.anchorMin = Vector2.one * 0.5f;
                rt.anchorMax = Vector2.one * 0.5f;
                rt.sizeDelta = temp;
            }

            var _rect = mLayoutElements_BgFrame[2].GetComponent<RectTransform>();
            mSelected.anchorMin = _rect.anchorMin;
            mSelected.anchorMax = _rect.anchorMax;
            mSelected.pivot = _rect.pivot;
            mSelected.position = _rect.position;
            mSelected.sizeDelta = _rect.sizeDelta;
        }

        /// <summary>
        /// 菜单按钮点击切换界面
        /// </summary>
        /// <param name="index"></param>
        private void ChangePanel(int index)
        {
            for (int i = 0; i < Panels.Count; i++)
            {
                if (i == index)
                    Panels[i].Show();
                else
                    Panels[i].Hide();
            }
        }

        /// <summary>
        /// 显示并初始化底部菜单按钮
        /// </summary>
        private void InitBeginMenuButton(int index = -1)
        {
            BottomMenuNode.Show();
            //有参传入，初始按钮点击(切换对应界面)
            if (index > -1)
                mMenuBtn[index].onClick.Invoke();
        }

        #endregion

        #region 字体/金币/体力/星星/头像/建筑 初始化更新
        
        private void LoaderRes()
        {
            TxtImgprogress.font = LevelManager.Instance.blueFont;
            TxtImgprogress.font.material.shader = Shader.Find(TxtImgprogress.font.material.shader.name);
            TxtStartLevel.font = LevelManager.Instance.redFont;
            TxtArea.font = LevelManager.Instance.redFont;
            TxtStraightWin_Red.font = LevelManager.Instance.redFont;
            TxtDoubleBuffCountDown_Red.font = LevelManager.Instance.redFont;
        }

        private void InitSceneUI()
        {
            //Init Avatar
            BtnHead.GetComponent<Image>().sprite = AvatarManager.Instance.GetAvatarSprite(true);
            ImgHeadFrame.sprite = AvatarManager.Instance.GetAvatarSprite(false);
      
            SetVitality();
            SetCoin();
            SetStar();
            SetScene();
            SetStartLevel();
        }
    
        private void SetCoin()
        {
            TxtCoin.text = CoinManager.Instance.Coin.ToString();
        }

        private void SetStar()
        {
            TxtStar.text = stageModel.RemainingStars.ToString();
        }

        private void SetVitality()
        {
            TxtHeart.text = HealthManager.Instance.UnLimitHp ? "∞" : HealthManager.Instance.NowHp.ToString();

            if (!HealthManager.Instance.UnLimitHp && HealthManager.Instance.IsMaxHp)
                TxtTime.text = "FULL";
        }

        private void SetStartLevel()
        {
            int currentLevel = saveData.GetCurrentLevel();
            string appendString="";
            // 设置图案和附加文本
            if (currentLevel > GameConst.LEVEL_TYPE_LAST_DIGIT)
            {
                switch (currentLevel% GameConst.LEVEL_TYPE_LAST_DIGIT)
                {
                    case 4:
                        appendString = "Hard Level";
                        BtnStart.transform.GetComponent<Image>().sprite = btnStartSprites[1];
                        break;
                    case 9:
                        appendString = "Super Hard";
                        BtnStart.transform.GetComponent<Image>().sprite = btnStartSprites[2];
                        break;
                    default:
                        appendString = "";
                        BtnStart.transform.GetComponent<Image>().sprite = btnStartSprites[0];
                        break;  
                }
            }
            TxtStartLevel.text = $"Level {currentLevel}"+$"<br><size=45>{appendString}</size>";

            TxtStraightWin_Red.text = $"{stageModel.InGameRankStreakWinNum} Straight";
        }

        /// <summary>
        /// 金币和星星的飞行粒子效果
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShowFx()
        {
            CoinManager.Instance.AddCoin((int)(GameConst.WIN_COINS * stageModel.GoldCoinsMultiple));
            RewardUIManager.Instance.PopupCoinText(GameConst.WIN_COINS * stageModel.GoldCoinsMultiple);
            coinFx.Play(10);
            yield return new WaitForSeconds(0.5f);
            starFx.Play(10);
            yield return new WaitForSeconds(0.5f);
            SetStar();
        }

        /// <summary>
        /// 更新主页场景建筑
        /// </summary>
        private void SetScene()
        {
            var _unitUnlockProgress = mSceneUnlockModel.SceneUnlockUnitIndex;
            var _sceneIndex = mSceneUnlockModel.SceneIndex;
            //注释代码是最后一个场景的判定,可作为安全代码保留
            //bool isLastScene = _sceneIndex >= GameConst.SceneUnlock.Count;
            //if (isLastScene)
            //    _sceneIndex = GameConst.SceneUnlock.Keys.Max();

            var _unitCount = mSceneUnlockPanels[_sceneIndex].GetComponent<SceneUnlockCtrl>().UnitCount;

            //if (isLastScene)
            //    _unitUnlockProgress = _unitCount;

            for (int i = 0; i < mSceneUnlockPanels.Length; i++)
            {
                mSceneUnlockPanels[i].Hide();
                if (i == _sceneIndex)
                    mSceneUnlockPanels[i].Show();
            }

            mSceneUnlockPanels[_sceneIndex].GetComponent<SceneUnlockCtrl>().UpdateUnitSprite(_unitUnlockProgress);

            SetStar();

            ImgProgress.fillAmount = (float)_unitUnlockProgress / _unitCount;
            TxtImgprogress.text = $"{_unitUnlockProgress} / {_unitCount}";

            //首套场景解锁完成
            if (_sceneIndex == 0 && _unitUnlockProgress == _unitCount)
            {
                UIKit.OpenPanel<PlotUnlockGuide>(UILevel.PopUI);
            }
        }

        private void UpdateUI()
        {
            if (HomeNode.activeSelf)
            {
                if (HealthManager.Instance.UnLimitHp || !HealthManager.Instance.IsMaxHp)
                {
                    TxtTime.text = HealthManager.Instance.UnLimitHp ?
                        HealthManager.Instance.UnLimitHpTimeStr :
                        HealthManager.Instance.RecoverTimerStr;
                }

                if (CountDownTimerManager.Instance.IsTimerFinished(GameEnum.GetDescription(SpecialRewardsType.UnlimitedDoubleBuff)))
                {
                    AnimStartFlash.Hide();
                    ImgDoubleBuff.Hide();
                    ImgDoubleBuffCountDown.Hide();
                }
                else
                {
                    AnimStartFlash.Show();
                    ImgDoubleBuff.Show();
                    ImgDoubleBuffCountDown.Show();
                    TxtDoubleBuffCountDown_Red.text = CountDownTimerManager.Instance.GetRemainingTimeText(GameEnum.GetDescription(SpecialRewardsType.UnlimitedDoubleBuff));
                }

                if (mVolcanicActivity is not null)
                {
                    switch (mVolcanicActivity.ActivityStatus)
                    {
                        case GameActivityStatus.Active:
                            TxtVolcanicActivity.text = mVolcanicActivity.GetActivityReamingTime();
                            break;

                        case GameActivityStatus.CoolingDown:
                            TxtVolcanicActivity.text = mVolcanicActivity.GetCooldownReamingTime();
                            break;
                    }
                }

                if (mHighTowerActivity is not null)
                {
                    if (mHighTowerActivity.ActivityStatus is GameActivityStatus.Active) 
                        TxtHighTowerActivity.text = mHighTowerActivity.GetActivityReamingTime();
                }

                if (mMagicStreakActivity is not null)
                {
                    if (mMagicStreakActivity.ActivityStatus is SettlementActivityStatus.Active)
                        TxtMagicStreakActivity.text = mMagicStreakActivity.GetActivityReamingTime();
                }

                if (mTierRankActivity is not null)
                {
                    if (mTierRankActivity.ActivityStatus is SettlementActivityStatus.Active)
                        TxtTierRankActivity.text = mTierRankActivity.GetHalfOneHourTierRankTime();
                }

                if (mRocketActivity is not null)
                {
                    if (mRocketActivity.ActivityStatus is GameActivityStatus.Active)
                        TxtRocketActivity.text = mRocketActivity.GetActivityReamingTime();
                }

              /* if (mPrograssGiftADActivity is not null)
               {
                    if (mPrograssGiftADActivity.ActivityStatus is GameActivityStatus.Active)
                        TxtPGActivity.text = mPrograssGiftADActivity.GetActivityReamingTime();
               }

               if(mSepecialOfferADActivity is not null)
               {
                    if (mSepecialOfferADActivity.ActivityStatus is GameActivityStatus.Active)
                        TxtSOActivity.text = mSepecialOfferADActivity.GetActivityReamingTime();
               }
               if(mDoubleGiftADAcitvity is not null)
               {
                    if (mDoubleGiftADAcitvity.ActivityStatus is GameActivityStatus.Active)
                        TxtDGActivity.text = mDoubleGiftADAcitvity.GetActivityReamingTime();
               }    */
            }
        }
        #endregion

        #region 活动模块
        private void ShowActivityState()
        {
            var _curLevel = saveData.GetCurrentLevel();
            //各活动在主页显示的条件(到达第几关在主页显示，部分活动结束时需隐藏自身)
            //活动未注册时管理显示，注册后由活动自身管理
            if (_curLevel >= 7 && mVolcanicActivity is null)
                BtnVANode.Show();

            if (_curLevel >= 16 && mRocketActivity is null)
                BtnRANode.Show();

            if (_curLevel >= GameConst.TRA_BEGIN_LEVEL)
                BtnTRANode.Show();

            if (_curLevel >= GameConst.TT_AD_BEGIN_LEVEL)
                BtnTTNode.Show();

            if (_curLevel >= 26)
                BtnMSANode.Show();

            if (_curLevel >= 46 && mHighTowerActivity is null)
                BtnHTANode.Show();

            if (_curLevel >= GameConst.SO_AD_BEGIN_LEVEL && !this.GetModel<SepecialOfferADActivityModel>().IsBuy)
                BtnSONode.Show();

            if (_curLevel >= GameConst.BP_AD_BEGIN_LEVEL)
            {
                BtnBPNode.Show();
                Btn_Bp.interactable = true;
                ImgLock.Hide();
            }
                

            if (_curLevel >= GameConst.DG_AD_BEGIN_LEVEL && (!this.GetModel<DoubleGiftADActivityModel>().IsBuy||! this.GetModel<DoubleGiftADActivityModel>().GiftIsGot))
                BtnDGNode.Show();

            if (_curLevel >= GameConst.PG_AD_BEGIN_LEVEL)
                BtnPGNode.Show();

            if (_curLevel >= GameConst.REMOVE_AD_BEGIN_LEVEL &&!this.GetModel<RemoveADACtivityModel>().IsBuy)
                BtnRemoveADNode.Show();
            
        }

        private void InitActivityState()
        {
            UpdateVAState();
            UpdateRAState();
            UpdateHTAState();
            UpdateMSAState();
            UptateTRAState();

            //横幅活动
            if (saveData.GetCurrentLevel() >= GameConst.WIN_STREAK_BEGIN_LEVEL)
            {
                RegisterBannerActivity();
            }
        }

        /// <summary>
        /// 活动状态变更事件
        /// </summary>
        private void ActivityStatusChangeEvent(OnActivityStatusChanged eventData)
        {
            //Debug.Log("收到事件、状态：" + eventData.Status);
            var _activity = eventData.Sender;
            if (_activity is VolcanicActivity)
            {
                mVolcanicActivity ??= _activity as VolcanicActivity;
                UpdateVAState();
            }

            else if (_activity is RocketActivity)
            {
                mRocketActivity ??= _activity as RocketActivity;
                UpdateRAState();
            }

            else if (_activity is HighTowerActivity)
            {
                mHighTowerActivity ??= _activity as HighTowerActivity;
                UpdateHTAState();
            }

            else if (_activity is MagicStreakActivity)
            {
                mMagicStreakActivity ??= _activity as MagicStreakActivity;
                UpdateMSAState();
            }

            else if(_activity is TurnTableADActivity)
            {
                mTurnTableADActivity ??= _activity as TurnTableADActivity;
                UpdateTTState();
            }
            else if (_activity is PrograssGiftADActivity)
            {
                mPrograssGiftADActivity ??= _activity as PrograssGiftADActivity;
                UpdatePGState();
            }
            else if (_activity is SepecialOfferADActivity)
            {
                mSepecialOfferADActivity ??= _activity as SepecialOfferADActivity;
                UpdateSOState();
            }
            else if (_activity is DuobleGiftAdActivity)
            {
                mDoubleGiftADAcitvity ??= _activity as DuobleGiftAdActivity;
                UpdateDGState();
            }

            else if (_activity is TierRankActivity)
            {
                mTierRankActivity ??= _activity as TierRankActivity;
                UptateTRAState();
            }


            else if (_activity is BannerActivity)
            {
                if (eventData.Status is GameActivityStatus.Active)
                {
                    if (mCurBannerActivity == null)
                    {
                        var potionNode = Resources.Load<GameObject>("Prefab/BannerActivityNode");
                        mCurBannerActivity = Instantiate(potionNode, HomeNode.transform);
                    }
                }
            }
          
            //...Other Activity
        }
        
        private void UpdateVAState()
        {
            if (mVolcanicActivity is null)
            {
                BtnVANode.interactable = false;
                TxtVolcanicActivity.text = "LV15";
                return;
            }

            ChangeActivityIcon(BtnVANode.gameObject);
            BtnVANode.interactable = true;
            switch (mVolcanicActivity.ActivityStatus)
            {
                case GameActivityStatus.Inactive:
                    BtnVANode.Show();
                    TxtVolcanicActivity.text = "START";
                    break;

                case GameActivityStatus.Active:
                    BtnVANode.Show();
                    TxtVolcanicActivity.text = mVolcanicActivity.GetActivityReamingTime();
                    break;
                    
                case GameActivityStatus.CoolingDown:
                    BtnVANode.Show();
                    TxtVolcanicActivity.text = mVolcanicActivity.GetCooldownReamingTime();
                    break;

                case GameActivityStatus.WaitStart:
                    BtnVANode.Hide();
                    break;
            }
        }

        private void UpdateRAState()
        {
            if (mRocketActivity is null)
            {
                BtnRANode.interactable = false;
                TxtRocketActivity.text = "LV25";
                return;
            }

            ChangeActivityIcon(BtnRANode.gameObject);
            BtnRANode.interactable = true;

            switch (mRocketActivity.ActivityStatus)
            {
                case GameActivityStatus.Inactive:
                    BtnRANode.Show();
                    TxtRocketActivity.text = "START";
                    break;

                case GameActivityStatus.Active:
                    BtnRANode.Show();
                    TxtRocketActivity.text = mRocketActivity.GetActivityReamingTime();
                    break;

                case GameActivityStatus.CoolingDown:
                    BtnRANode.Hide();
                    break;
            }
        }

        private void UpdateHTAState()
        {
            if (mHighTowerActivity is null)
            {
                BtnHTANode.interactable = false;
                TxtHighTowerActivity.text = "LV65";
                return;
            }

            ChangeActivityIcon(BtnHTANode.gameObject);
            BtnHTANode.interactable = true;

            switch (mHighTowerActivity.ActivityStatus)
            {
                case GameActivityStatus.Inactive:
                    BtnHTANode.Show();
                    TxtHighTowerActivity.text = "START";
                    break;

                case GameActivityStatus.Active:
                    BtnHTANode.Show();
                    TxtHighTowerActivity.text = mHighTowerActivity.GetActivityReamingTime();
                    break;

                default:
                    BtnHTANode.Hide();
                    break;
            }
        }

        private void UpdateMSAState()
        {
            if (mMagicStreakActivity is null)
            {
                BtnMSANode.interactable = false;
                TxtMagicStreakActivity.text = "LV45";
                return;
            }

            ChangeActivityIcon(BtnMSANode.gameObject);
            BtnMSANode.interactable = true;
            TxtMagicStreakActivity.text = mMagicStreakActivity.ActivityStatus switch
            {
                SettlementActivityStatus.Inactive => "START",
                SettlementActivityStatus.Active => mMagicStreakActivity.GetActivityReamingTime(),
                _ => "Finished"
            };
        }

        private void UptateTRAState()
        {
            if (mTierRankActivity is not null)
            {
                if (mTierRankActivity.ActivityStatus == SettlementActivityStatus.Locked)
                {
                    BtnTRANode.interactable = false;
                    TxtTierRankActivity.text = "Next Day";
                    return;
                }

                ChangeActivityIcon(BtnTRANode.gameObject);
                BtnTRANode.interactable = true;
                //更换横幅精灵
              
                TxtTierRankActivity.text = mTierRankActivity.ActivityStatus switch
                {
                    SettlementActivityStatus.Inactive => "START",
                    SettlementActivityStatus.Active => mTierRankActivity.GetHalfOneHourTierRankTime(),
                    _ => "Finished"
                };
            }
        }
        
        private void UpdateTTState()
        {  
            TxtTTActivity.text = mTurnTableADActivity.ActivityStatus switch
            {
                GameActivityStatus.Active => "Active",
                _ => "Finished"
            };
        }
        private void UpdatePGState()
        {
            TxtPGActivity.text = mPrograssGiftADActivity.ActivityStatus switch
            {
                GameActivityStatus.Active => "Active",
                _ => "Finished"
            };
        }
       private void UpdateSOState()
       {
            TxtSOActivity.text = mSepecialOfferADActivity.ActivityStatus switch
            {
                GameActivityStatus.Active => "Active",
                _ => "Finished"
            };
        }
        private void UpdateDGState()
        {
            TxtDGActivity.text = mDoubleGiftADAcitvity.ActivityStatus switch
            {
                GameActivityStatus.Active => "Active",
                _ => "Finished"
            };
        }


        private void ChangeActivityIcon(GameObject activity)
        {
            var _gameActivityStateCtrl = activity.GetComponent<GameActivityStateCtrl>();
            _gameActivityStateCtrl.ChangeIcon();
        }

        /// <summary>
        /// 横幅连胜活动
        /// </summary>
        private void RegisterBannerActivity()
        {
            //Debug.Log("实例活动");
            //CountDownTimerManager.Instance.ResetTimer(GameConst.POTION_ACTIVITY_SIGN, 10);
            CountDownTimerManager.Instance.StartTimer(GameConst.POTION_ACTIVITY_SIGN, 1440f);
            var potionActivityModel = this.GetModel<PotionActivityModel>();
            if (!CountDownTimerManager.Instance.IsTimerFinished(GameConst.POTION_ACTIVITY_SIGN))
            {
                if (!potionActivityModel.PotionActivityProgressEnd)
                {
                    var potionNode = Resources.Load("Prefab/PotionActivityNode");
                    var node = Instantiate(potionNode, HomeNode.transform);
                    //Debug.Log("剩余时长：" + CountDownTimerManager.Instance.GetRemainingTimeText(GameConst.POTION_ACTIVITY_SIGN));
                }
            }
            else
            {
                GameActivityManager.Instance.RegisterActivity<BannerActivity>();
                mBannerActivity = GameActivityManager.Instance.GetActivity<BannerActivity>();
                if (mBannerActivity.ActivityStatus == GameActivityStatus.Active)
                {
                    if (mCurBannerActivity == null)
                    {
                        var potionNode = Resources.Load<GameObject>("Prefab/BannerActivityNode");
                        mCurBannerActivity = Instantiate(potionNode, HomeNode.transform);
                    }
                }
            }
        }

        #endregion


        private void InitUI()
        {
            if(saveData.GetCurrentLevel()>=GameConst.BP_AD_BEGIN_LEVEL)
            {
                Btn_Bp.interactable = true;
                ImgLock.Hide();
            }
            else
            {
                Btn_Bp.interactable = false;
                ImgLock.Show();
            }
            
        }
    }
    
}
