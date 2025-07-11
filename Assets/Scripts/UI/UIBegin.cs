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
        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

        public GameObject[] SceneNodes;
        public ScenePartCtrl[] scenePartCtrls;

        public RectTransform[] ItemBeginPos;
        public Image[] ImgItems;

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

        private TextMeshProUGUI mTxtCoinAdd;
        private StageModel stageModel;
        private SaveDataUtility saveData;

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
            LevelManager.Instance.InitBottle();
            mTxtCoinAdd = TxtCoinAdd.GetComponent<TextMeshProUGUI>();
           

            if (saveData.GetCurrentLevel() <= 5)
            {
                BottomMenuBtns.Hide();
                HomeNode.Hide();
            }

            else if (saveData.GetCurrentLevel() >= GameConst.WIN_STREAK_BEGIN_LEVEL)
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
            if (HealthManager.Instance.UnLimitHp || !HealthManager.Instance.IsMaxHp)
            {
                TxtTime.text = HealthManager.Instance.UnLimitHp ?
                    HealthManager.Instance.UnLimitHpTimeStr :
                    HealthManager.Instance.RecoverTimerStr;
            }
        }

        private void OnEnable()
        {
            
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
                UIKit.OpenPanel<UIUnlockScene>();
            });

            BtnGetReward.onClick.RemoveAllListeners();
            //获取完奖励回调 
            BtnGetReward.onClick.AddListener(() =>
            {
                SetScene();
                StartCoroutine(FlyReward());
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
                BottomMenuBtns.Show();
                HomeNode.Show();
                AudioKit.ResumeMusic();
                StartCoroutine(ShowFx());

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<ReturnMainEvent>(e =>
            {
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
                SetScene();
                ShowFx(e.scene, e.part);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<RewardSceneEvent>(e =>
            {
                ShowReward();
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

            StringEventSystem.Global.Register("OpenShopPanel", () =>
            {
                InitBeginMenuButton(0);

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            StringEventSystem.Global.Register("StartPotionActivity", () =>
            {
                PotionActivity();
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
            var nowStar = saveData.GetCurrentLevel() - 1;
            var sceneNow = saveData.GetSceneRecord();
            var partNow = saveData.GetScenePartRecord();
            //可以记录一个已使用星星数量
            var useStar = LevelManager.Instance.GetUnlockNeedStar(sceneNow, partNow);
            TxtStar.text = (nowStar - useStar).ToString();

            //Debug.Log("更新星星数量");
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
        /// 金币和星星的飞行粒子效果
        /// </summary>
        /// <returns></returns>
        IEnumerator ShowFx()
        {
            mTxtCoinAdd.text = $"+{GameConst.WIN_COINS * stageModel.GoldCoinsMultiple}";
            TxtCoinAdd.Play("TxtUp");
            coinFx.Play(10);
            yield return new WaitForSeconds(0.5f);
            CoinManager.Instance.AddCoin((int)(GameConst.WIN_COINS * stageModel.GoldCoinsMultiple));

            starFx.Play(10);
            yield return new WaitForSeconds(0.5f);
            SetStar();
        }

        /// <summary>
        /// 更新主页场景建筑和部分UI
        /// </summary>
        void SetScene()
        {
            SetStar();
            var sceneNow = saveData.GetSceneRecord();
            var partNow = saveData.GetScenePartRecord();

            TxtArea.text = "Area " + sceneNow;

            //解锁完成(锁定最后一个场景)
            if (saveData.GetOverUnLock())
            {
                ImgProgress.fillAmount = 0;
                TxtImgprogress.text = 0 + " / 5";

                var _scene = LevelManager.Instance.SceneUnLockSOs.Count() - 1;
                SceneNodes[_scene].Show();
                SetScenePart(_scene, partNow);
                return;
            }

            //未全部解锁
            for (int i = 0; i < SceneNodes.Length; i++)
            {
                //场景从1开始计算
                if ((i + 1) == sceneNow)
                    SceneNodes[i].Show();
                else
                    SceneNodes[i].Hide();
            }

            ImgProgress.fillAmount = partNow / 5f;
            TxtImgprogress.text = partNow + " / 5";

            SetScenePart(sceneNow, partNow);
        }

        /// <summary>
        /// 解锁场景建筑
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="partNow"></param>
        void SetScenePart(int scene, int partNow)
        {
            //场景从1开始计算
            int _index = scene - 1;
            if (_index >= 0 && _index < scenePartCtrls.Length)
            {
                for (int j = 0; j < scenePartCtrls[_index].SceneParts.Length; j++)
                {
                    scenePartCtrls[_index].SceneParts[j].SetActive(partNow > j);
                }
            }
        }

        /// <summary>
        /// 解锁场景建筑触发特效
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="num"></param>
        void ShowFx(int scene, int num)
        {
            //场景从1开始计算
            int _index = scene - 1;
            if (_index >= 0 && _index < scenePartCtrls.Length)
            {
                StartCoroutine(scenePartCtrls[_index].ShowUnlock(num));
            }
        }

        /// <summary>
        /// 解锁建筑宝箱奖励
        /// </summary>
        void ShowReward()
        {
            RewardNode.gameObject.SetActive(true);

            foreach (var item in ImgItems)
            {
                item.Show();
            }
        }

        /// <summary>
        /// 道具飞行效果(完成回调)
        /// </summary>
        /// <returns></returns>
        IEnumerator FlyReward()
        {
            RewardNode.gameObject.SetActive(false);

            for (int i = 0; i < ImgItems.Length; i++)
            {
                int index = i;
                ImgItems[index].transform.DOMove(Target.transform.position, 1f).SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        ImgItems[index].Hide();
                        ImgItems[index].transform.position = ItemBeginPos[index].transform.position;
                    });

                yield return new WaitForSeconds(0.2f);
            }

            RewardCoinFx.Play(10);
            yield return new WaitForSeconds(1.5f);
            CoinManager.Instance.AddCoin(200);
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
