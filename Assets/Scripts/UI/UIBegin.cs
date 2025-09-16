using UnityEngine;
using UnityEngine.UI;
using QFramework;
using GameDefine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using System.Linq;
using System;
using TMPro;

namespace QFramework.Example
{
    public class UIBeginData : UIPanelData
    {
    }
    public partial class UIBegin : UIPanel, ICanRegisterEvent, ICanGetUtility, ICanGetModel
    {
        public ParticleTargetMoveCtrl coinFx, starFx;
        [SerializeField] private Sprite[] btnStartSprites;

        #region BottomMenuSetting
        [SerializeField] private List<Button> bottomMenuBtns;
        [SerializeField] private List<RectTransform> bottomMenuRect;
        [SerializeField] private List<GameObject> Panels;
        [SerializeField] private RectTransform selectedImg;
        private GameObject HomeNode => Panels[2];
        private int nowButton = 2;
        private readonly Vector2 SELECTED = new Vector2(256, 200);  // 选中放大的大小
        private readonly Vector2 NSELECTED = new Vector2(206, 200); // 未选中的大小
        private readonly float minScaleValue = 0.5f;                // 按钮的缩小值(先缩小后放大)
        private readonly float maxScaleValue = 1.2f;                // 按钮的放大值
        private readonly float targetPosY = 80f;                    // 按钮往上抬起的高度
        private readonly float initPosY = 15f;                      // 按钮的初始位置
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

            LevelManager.Instance.InitBottle();

            InitTxtFont();

            int currentLevel = saveData.GetCurrentLevel();
            if (currentLevel <= 5)
            {
                BottomMenuBtns.Hide();
                HomeNode.Hide();
            }

            //连胜活动
            if (currentLevel >= GameConst.WIN_STREAK_BEGIN_LEVEL)
            {
                PotionActivity();
            }

            //BindBtn();
            //RegisterEvent();
            //InitSceneUI();
            //InitActivityState();
        }

        protected override void OnShow()
        {
            BindBtn();
            RegisterEvent();
            InitSceneUI();
            InitActivityState();
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
        }

        private void Update()
        {
            //后续改成异步线程(一秒触发一次)
            if (HealthManager.Instance.UnLimitHp || !HealthManager.Instance.IsMaxHp)
            {
                TxtTime.text = HealthManager.Instance.UnLimitHp ?
                    HealthManager.Instance.UnLimitHpTimeStr :
                    HealthManager.Instance.RecoverTimerStr;
            }
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
            foreach (var btn in bottomMenuBtns)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                   int index = bottomMenuBtns.IndexOf(btn);
                   //切换界面
                   ChangePanel(index);
                   if (nowButton != index)
                   {
                       for (int i = 0; i < bottomMenuRect.Count; i++)
                       {
                           var rt = bottomMenuBtns[i].GetComponent<RectTransform>();
                           if (i == index)
                           {
                               //设置选中效果
                               rt.localScale = new Vector3(minScaleValue, minScaleValue, minScaleValue);
                               rt.DOScale(new Vector3(maxScaleValue, maxScaleValue, 1), 0.1f);
                               rt.DOLocalMoveY(targetPosY, 0.1f);
                               bottomMenuRect[index].sizeDelta = SELECTED;
                           }
                           else
                           {
                               //设置未选中效果
                               rt.DOScale(Vector3.one, 0.2f);
                               rt.DOLocalMoveY(initPosY, 0.2f);
                               bottomMenuRect[i].sizeDelta = NSELECTED;
                           }
                       }
                       //等待一帧
                       ActionKit.DelayFrame(1, () =>
                       {
                           //同步按钮中心位置(可以设置按钮下的字体显示)
                           for (int i = 0; i < bottomMenuBtns.Count; i++)
                           {
                               var rt = bottomMenuBtns[i].GetComponent<RectTransform>();
                               rt.DOLocalMoveX(bottomMenuRect[i].localPosition.x, 0.2f);
                           }
                           //更新滑动块
                           selectedImg.DOMove(bottomMenuRect[index].position, 0.1f);
                           nowButton = index;
                       }).Start(this);
                   }
                });
            }
        }

        private void RegisterEvent()
        {
            //胜利结算=》返回主页事件
            this.RegisterEvent<LevelClearEvent>(e =>
            {
                LevelManager.Instance.InitBottle();
                BottomMenuBtns.Show();
                HomeNode.Show();
                SetStartLevel();
                StartCoroutine(ShowFx());

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<ReturnMainEvent>(e =>
            {
                LevelManager.Instance.InitBottle();
                BottomMenuBtns.Show();
                HomeNode.Show();
                SetStartLevel();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<VitalityChangeEvent>(e =>
            {
                SetVitality();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<UnlockSceneBackEvent>(e =>
            {
                this.gameObject.Show();
                SetScene();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<GameStartEvent>(e =>
            {
                UIKit.OpenPanel<UIGameNode>();

                LevelManager.Instance.StartGame(saveData.GetCurrentLevel());
                BottomMenuBtns.Hide();
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
                PotionActivity();
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
                bottomMenuBtns.Last().onClick.Invoke();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        //可拓展
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

        /// <summary>
        /// 菜单按钮点击切换界面
        /// </summary>
        /// <param name="index"></param>
        void ChangePanel(int index)
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
        void InitBeginMenuButton(int index = -1)
        {
            BottomMenuBtns.Show();
            //有参传入，初始按钮点击(切换对应界面)
            if (index > -1)
                bottomMenuBtns[index].onClick.Invoke();
        }

        #endregion

        #region 字体/金币/体力/星星/头像/建筑 初始化更新
        
        private void InitTxtFont()
        {
            TxtImgprogress.font = LevelManager.Instance.blueFont;
            TxtImgprogress.font.material.shader = Shader.Find(TxtImgprogress.font.material.shader.name);
            TxtArea.font = LevelManager.Instance.redFont;
        }

        private void InitSceneUI()
        {
            SetAvatar();
            SetVitality();
            SetCoin();
            SetStar();
            SetScene();
            // 开始按钮的变化逻辑下放给SetStartLevel,因为不止这里需要调用
            SetStartLevel();
        }

        private void SetAvatar()
        {
            BtnHead.GetComponent<Image>().sprite = AvatarManager.Instance.GetAvatarSprite(true);
            ImgHeadFrame.sprite = AvatarManager.Instance.GetAvatarSprite(false);
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
            TxtStartLevel.text = $"Level {currentLevel}"+$"<br><size=50>{appendString}</size>";
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

            //索引从0开始计算(显示时+1)
            TxtArea.text = "Area " + (_sceneIndex + 1);
            ImgProgress.fillAmount = (float)_unitUnlockProgress / _unitCount;
            TxtImgprogress.text = $"{_unitUnlockProgress} / {_unitCount}";

            //首套场景解锁完成
            if (_sceneIndex == 0 && _unitUnlockProgress == _unitCount)
            {
                UIKit.OpenPanel<SceneUnlockGuide>(UILevel.PopUI);
            }
        }
        #endregion

        #region 活动模块
        private void InitActivityState()
        {
            UpdateVAState();
            UpdateRAState();
            UpdateHTAState();
            UpdateMSAState();
            UptateTRAState();
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

            else if (_activity is TierRankActivity)
            {
                mTierRankActivity ??= _activity as TierRankActivity;
                UptateTRAState();
            }
            //...Other Activity
        }
        
        private void UpdateVAState()
        {
            if (mVolcanicActivity is null)
            {
                BtnVANode.interactable = false;
                TxtVolcanicActivity.text = "Locked";
                return;
            }
            BtnVANode.interactable = true;
            TxtVolcanicActivity.text = mVolcanicActivity.ActivityStatus switch
            {
                GameActivityStatus.Inactive => "Inactive",
                GameActivityStatus.Active => "Active",
                _ => "Finished"
            };
        }

        private void UpdateRAState()
        {
            if (mRocketActivity is null)
            {
                BtnRANode.interactable = false;
                TxtRocketActivity.text = "Locked";
                return;
            }
            BtnRANode.interactable = true;
            TxtRocketActivity.text = mRocketActivity.ActivityStatus switch
            {
                GameActivityStatus.Inactive => "Inactive",
                GameActivityStatus.Active => "Active",
                _ => "Finished"
            };
        }

        private void UpdateHTAState()
        {
            if (mHighTowerActivity is null)
            {
                BtnHTANode.interactable = false;
                TxtHighTowerActivity.text = "Locked";
                return;
            }
            BtnHTANode.interactable = true;
            TxtHighTowerActivity.text = mHighTowerActivity.ActivityStatus switch
            {
                GameActivityStatus.Inactive => "Inactive",
                GameActivityStatus.Active => "Active",
                _ => "Finished"
            };
        }

        private void UpdateMSAState()
        {
            if (!CountDownTimerManager.Instance.IsTimerFinished(GameConst.FIRST_LAUNCH_SIGN))
            {
                BtnMSANode.interactable = false;
                TxtMagicStreakActivity.text = "Locked";
                return;
            }

            BtnMSANode.interactable = true;
            TxtMagicStreakActivity.text = mMagicStreakActivity.ActivityStatus switch
            {
                SettlementActivityStatus.Inactive => "Inactive",
                SettlementActivityStatus.Active => "Active",
                _ => "Finished"
            };
        }

        private void UptateTRAState()
        {
            if (mTierRankActivity is null)
            {
                BtnTRANode.interactable = false;
                TxtTierRankActivity.text = "Locked";
                return;
            }

            BtnTRANode.interactable = true;
            TxtTierRankActivity.text = mTierRankActivity.ActivityStatus switch
            {
                SettlementActivityStatus.Inactive => "Inactive",
                SettlementActivityStatus.Active => "Active",
                _ => "Finished"
            };
        }

        /// <summary>
        /// 连胜活动
        /// </summary>
        private void PotionActivity()
        {
            //Debug.Log("实例活动");
            //CountDownTimerManager.Instance.ResetTimer(GameConst.POTION_ACTIVITY_SIGN, 10);
            CountDownTimerManager.Instance.StartTimer(GameConst.POTION_ACTIVITY_SIGN, 1440f);
            var potionActivityModel = this.GetModel<PotionActivityModel>();
            if (!CountDownTimerManager.Instance.IsTimerFinished(GameConst.POTION_ACTIVITY_SIGN) &&
                 !potionActivityModel.PotionActivityProgressEnd)
            {
                var potionNode = Resources.Load("Prefab/PotionActivityNode");
                var node = Instantiate(potionNode, HomeNode.transform);
                //Debug.Log("剩余时长：" + CountDownTimerManager.Instance.GetRemainingTimeText(GameConst.POTION_ACTIVITY_SIGN));
            }
        }
        #endregion
    }
}
