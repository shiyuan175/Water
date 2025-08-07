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

            LevelManager.Instance.InitBottle();
           
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
        }

        protected override void OnShow()
        {
            BindBtn();
            RegisterEvent();
            SetAvatar();
            SetVitality();
            SetCoin();
            SetStar();
            SetScene();
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

            //更新火山活动的剩余时间
            if (BtnVANode.gameObject.activeSelf && mVolcanicActivity != null)
            {
                TxtVolcanicActivity.text = mVolcanicActivity.GetActivityReamingTime();
            }

            if (BtnRANode.gameObject.activeSelf && mRocketActivity != null)
            {
                TxtRocketActivity.text = mRocketActivity.GetActivityReamingTime();
            }

            if (BtnHTANode.gameObject.activeSelf && mHighTowerActivity != null)
            {
                TxtHighTowerActivity.text = mHighTowerActivity.GetActivityReamingTime();
            }
        }

        private void InitTxtFont()
        {
            TxtImgprogress.font = LevelManager.Instance.blueFont;
            TxtImgprogress.font.material.shader = Shader.Find(TxtImgprogress.font.material.shader.name);
            TxtArea.font = LevelManager.Instance.redFont;
            TxtCoinAdd.GetComponent<TMPro.TextMeshProUGUI>().font = LevelManager.Instance.redFont;
        }

        //按钮监听
        void BindBtn()
        {
            BtnStart.onClick.RemoveAllListeners();
            BtnStart.onClick.AddListener(() =>
            {
                UIKit.OpenPanel<UIBeginSelect>();
            });

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
                UIKit.OpenPanel<UIVolcanicActivityEntrance>();
            });

            BtnRANode.onClick.RemoveAllListeners();
            BtnRANode.onClick.AddListener(() =>
            {
                UIKit.OpenPanel<UIRocketActivity>();
            });

            BtnHTANode.onClick.RemoveAllListeners();
            BtnHTANode.onClick.AddListener(() =>
            {
                UIKit.OpenPanel<UIHighTowerActivity>();
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

        //事件注册
        void RegisterEvent()
        {
            //胜利结算=》返回主页事件
            this.RegisterEvent<LevelClearEvent>(e =>
            {
                LevelManager.Instance.InitBottle();
                BottomMenuBtns.Show();
                HomeNode.Show();
                AudioKit.ResumeMusic();
                StartCoroutine(ShowFx());

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<ReturnMainEvent>(e =>
            {
                LevelManager.Instance.InitBottle();
                BottomMenuBtns.Show();
                HomeNode.Show();
                AudioKit.ResumeMusic();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<CoinChangeEvent>(e =>
            {
                SetCoin(e.coin);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<VitalityChangeEvent>(e =>
            {
                SetVitality();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<UnlockSceneEvent>(e =>
            {
                this.gameObject.Show();
                SetScene();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<GameStartEvent>(e =>
            {
                UIKit.OpenPanel<UIGameNode>();
                AudioKit.PauseMusic();
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
                SetActivity(e);

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            StringEventSystem.Global.Register(GameConst.OPEN_SHOP_PANEL_EVENT, () =>
            {
                InitBeginMenuButton(0);

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            StringEventSystem.Global.Register(GameConst.START_POTION_ACTIVITY, () =>
            {
                PotionActivity();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            StringEventSystem.Global.Register(GameConst.CLOSE_VOLCANIC_ACTIVITY_EVENT, () =>
            {
                BtnVANode.Hide();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
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

        private void SetAvatar()
        {
            BtnHead.GetComponent<Image>().sprite = AvatarManager.Instance.GetAvatarSprite(true);
            ImgHeadFrame.sprite = AvatarManager.Instance.GetAvatarSprite(false);
        }

        /// <summary>
        /// 更新金币数量
        /// </summary>
        /// <param name="num"></param>
        void SetCoin(int num = 0)
        {
            if (num == 0)
                num = CoinManager.Instance.Coin;

            TxtCoin.text = num.ToString();
        }

        /// <summary>
        /// 更新星星数量
        /// </summary>
        void SetStar()
        {
            TxtStar.text = mSceneUnlockModel.RemainingStar.ToString();
        }

        /// <summary>
        /// 更新体力
        /// </summary>
        void SetVitality()
        {
            TxtHeart.text = HealthManager.Instance.UnLimitHp ? "∞" : HealthManager.Instance.NowHp.ToString();

            if (!HealthManager.Instance.UnLimitHp && HealthManager.Instance.IsMaxHp)
                TxtTime.text = "FULL";
        }

        /// <summary>
        /// 设置活动入口状态
        /// </summary>
        private void SetActivity(OnActivityStatusChanged eventData)
        {
            //Debug.Log("收到事件、状态：" + eventData.Status);
            var _activity = eventData.Sender;
            if (_activity is VolcanicActivity)
            {
                if (mVolcanicActivity == null)
                    mVolcanicActivity = _activity as VolcanicActivity;
                BtnVANode.gameObject.SetActive(eventData.Status == GameActivityStatus.Active);
            }

            else if (_activity is RocketActivity)
            {
                if (mRocketActivity == null)
                    mRocketActivity = _activity as RocketActivity;
                BtnRANode.gameObject.SetActive(eventData.Status == GameActivityStatus.Active);
            }

            else if (_activity is HighTowerActivity)
            {
                if(mHighTowerActivity == null)
                    mHighTowerActivity = _activity as HighTowerActivity;
                BtnHTANode.gameObject.SetActive(eventData.Status == GameActivityStatus.Active);
            }
            //...Other Activity
        }

        /// <summary>
        /// 金币和星星的飞行粒子效果
        /// </summary>
        /// <returns></returns>
        IEnumerator ShowFx()
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
        void SetScene()
        {
            var _unitUnlockProgress = mSceneUnlockModel.SceneUnlockUnitIndex;
            var _sceneIndex = mSceneUnlockModel.SceneIndex;

            //最后一个场景判断
            bool isLastScene = _sceneIndex >= GameConst.SceneUnlock.Count;

            if (isLastScene)
                _sceneIndex = GameConst.SceneUnlock.Keys.Max();

            var _unitCount = mSceneUnlockPanels[_sceneIndex].GetComponent<SceneUnlockCtrl>().UnitCount;

            if (isLastScene)
                _unitUnlockProgress = _unitCount;

            for (int i = 0; i < mSceneUnlockPanels.Length; i++)
            {
                mSceneUnlockPanels[i].Hide();
                if (i == _sceneIndex)
                {
                    mSceneUnlockPanels[i].Show();
                }
            }

            mSceneUnlockPanels[_sceneIndex].GetComponent<SceneUnlockCtrl>().UpdateUnitSprite(_unitUnlockProgress);

            SetStar();

            //索引从0开始计算(显示时+1)
            TxtArea.text = "Area " + (_sceneIndex + 1);
            ImgProgress.fillAmount = (float)_unitUnlockProgress / _unitCount;
            TxtImgprogress.text = $"{_unitUnlockProgress} / {_unitCount}";
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
    }
}
