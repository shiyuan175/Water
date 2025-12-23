using UnityEngine;
using QFramework;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using System;

namespace QFramework.Example
{
    public class UIPrograssGiftADActivityData : UIPanelData
    {
        public bool? IsManagedOpen;
    }
    public partial class UIPrograssGiftADActivity : UIPanel, ICanGetModel
    {
        [SerializeField] private List<Transform> giftPanels;
        [SerializeField] private List<Transform> panelPosions;
        [SerializeField] private TextMeshProUGUI[] TxtReds;
        [SerializeField] private List<string> GiftIDs;
        private PrograssGiftADActivityModel mPGModel;
        private PrograssGiftADActivity mPGADActivity;
        private GooglePayManager googlePay;
        private Tween mCountDownTween;
        private Sequence buttomImageFillSequence;
        private Dictionary<string, Action> giftPackBuySuccessActions;
        bool isBuy = false;

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UIPrograssGiftADActivityData ?? new UIPrograssGiftADActivityData();
            mPGModel = this.GetModel<PrograssGiftADActivityModel>();
            mPGModel.TempLevel = 0;
            foreach (var i in TxtReds)
            {
                i.font = LevelManager.Instance.redFont;
            }
            // please add init code here
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            if (giftPanels.Count == 0)
                Debug.LogError("没有找到6个礼包的transfrme");
            mPGADActivity = GameActivityManager.Instance.GetActivity<PrograssGiftADActivity>();
            googlePay = GooglePayManager.Instance;
            // 初始化购买成功回调
            giftPackBuySuccessActions = new Dictionary<string, Action>();
            foreach (var i in GiftIDs)
                giftPackBuySuccessActions[i] = () => OnPaySuccess();
            SetBtnClick();

        }

        protected override void OnShow()
        {

            InitUI();
            // 注册购买成功事件
            foreach (var kvp in giftPackBuySuccessActions)
            {
                StringEventSystem.Global.Register(kvp.Key, kvp.Value).UnRegisterWhenGameObjectDestroyed(gameObject);
            }
        }

        protected override void OnHide()
        {

        }

        protected override void OnClose()
        {
            if (mData.IsManagedOpen ?? false)
                StringEventSystem.Global.Send(GameDefine.GameConst.MANAGER_OPEN_NEXT_PANEL);
            buttomImageFillSequence?.Kill();
        }

        /// <summary>
        /// 初始化界面UI
        /// </summary>
        protected void InitUI()
        {
            #region 顶部UI

            // 倒计时
            mCountDownTween = DOTween.To(() => 0, x =>
            {
                if (mPGADActivity.ActivityStatus == GameActivityStatus.Active)
                    TxtCountDown.text = mPGADActivity.GetActivityReamingTime();
                else
                    TxtCountDown.text = "Completeed";
            }, 1, 1f)
          .SetLoops(-1, LoopType.Restart)
          .SetUpdate(true);
            #endregion

            #region 底部UI	

            for (int i = 0; i < giftPanels.Count; i++)
            {
                if (mPGModel.RewardLevel + 6 <= mPGModel.mPGData.Rewards.Length)
                {
                    int level = mPGModel.RewardLevel + i;
                    if (i == 0)
                        giftPanels[i].GetComponent<PrograssGiftPanel>().Initialize(BuyItem, mPGModel.mPGData.Rewards[level], false, false);
                    else
                        giftPanels[i].GetComponent<PrograssGiftPanel>().Initialize(BuyItem, mPGModel.mPGData.Rewards[level], false, true);
                }
                else
                {
                    int level = mPGModel.mPGData.Rewards.Length - 6 + i;
                    if (level < mPGModel.RewardLevel)
                        giftPanels[i].GetComponent<PrograssGiftPanel>().Initialize(BuyItem, mPGModel.mPGData.Rewards[level], true, false);
                    else if (level == mPGModel.RewardLevel)
                    {
                        giftPanels[i].GetComponent<PrograssGiftPanel>().Initialize(BuyItem, mPGModel.mPGData.Rewards[level], false, false);
                    }
                    else
                        giftPanels[i].GetComponent<PrograssGiftPanel>().Initialize(BuyItem, mPGModel.mPGData.Rewards[level], false, true);
                }

            }


            #endregion
        }
        private void SetBtnClick()
        {
            BtnClose.onClick.RemoveAllListeners();
            BtnClose.onClick.AddListener(() =>
            {
                CloseSelf();
            });
        }
        /// <summary>
        /// 播放界面动画
        /// </summary>
        private void UIPlayAnimation()
        {
            // 上一个已经领取
            buttomImageFillSequence = DOTween.Sequence();
            int lodLevel = mPGModel.TempLevel - 1;
            // 判断是否需要动画 执行动画前要先判断位置
            if (mPGModel.mPGData.Rewards.Length - mPGModel.RewardLevel >= giftPanels.Count)
            {
                // 移动到第一个位置
                buttomImageFillSequence.Append(DisappearGiftPanel(giftPanels[(lodLevel) % 6]));
                for (int i = 1; i <= 5 && mPGModel.mPGData.Rewards.Length - lodLevel - i >= giftPanels.Count; i++)
                {
                    Transform changerTransform = giftPanels[(lodLevel + i) % 6];
                    if (i == 1)
                    {
                        buttomImageFillSequence.Append(ChangeGiftPanelPostion(changerTransform, panelPosions[i - 1]));
                        buttomImageFillSequence.Join(changerTransform.GetComponent<PrograssGiftPanel>().UnLock());
                    }
                    else
                        buttomImageFillSequence.Append(ChangeGiftPanelPostion(changerTransform, panelPosions[i - 1]));
                }
                buttomImageFillSequence.Append(AppearGiftPanel(giftPanels[(lodLevel) % 6], panelPosions[panelPosions.Count - 1]));
            }
            else
            {
                Debug.Log(mPGModel.RewardLevel % 6);
                Transform needChangerTransform = giftPanels[(mPGModel.RewardLevel) % 6];
                buttomImageFillSequence.Append(needChangerTransform.GetComponent<PrograssGiftPanel>().UnLock());
            }
            buttomImageFillSequence.Play();
            /* // 增加下一个等级
             mPGModel.AddRewardLevel();*/
        }

        #region 动画
        private Tween DisappearGiftPanel(Transform disapperPanel)
        {
            // 动画时间
            float durationTime = 0.5f;

            return disapperPanel.DOScale(Vector3.zero, durationTime)
                .OnStart(() =>
                {
                    RectTransform rectTransform = disapperPanel.GetComponent<RectTransform>();

                    // 保存当前位置和尺寸
                    Vector3 savedWorldPosition = rectTransform.position;
                    Vector2 savedSize = rectTransform.rect.size;
                    Vector2 originalPivot = rectTransform.pivot;

                    // 修改锚点到中心
                    rectTransform.anchorMin = new Vector2(0, 0.5f);
                    rectTransform.anchorMax = new Vector2(0, 0.5f);
                    rectTransform.pivot = new Vector2(0, 0.5f);

                    // 计算轴心点变化导致的偏移补偿
                    Vector2 pivotDelta = new Vector2(0, 0.5f) - originalPivot;
                    Vector2 positionCompensation = new Vector2(pivotDelta.x * savedSize.x, pivotDelta.y * savedSize.y);

                    rectTransform.position = savedWorldPosition;
                    rectTransform.anchoredPosition += positionCompensation;
                })
                .SetEase(Ease.InBack);

        }

        private Tween AppearGiftPanel(Transform AppearPanel, Transform targerPanel)
        {
            // 动画时间
            float durationTime = 0.35f;

            AppearPanel.gameObject.SetActive(true);
            return AppearPanel.DOScale(Vector3.one, durationTime)
                .SetDelay(0.1f)
                .OnStart(() =>
                {
                    #region 设置位置

                    AppearPanel.GetComponent<PrograssGiftPanel>().Initialize(BuyItem,
                        mPGModel.mPGData.Rewards[mPGModel.RewardLevel + 5], false, true);
                    AppearPanel.localScale = Vector3.one;
                    // 移动位置
                    RectTransform changeRect = AppearPanel.GetComponent<RectTransform>();
                    RectTransform targerRect = targerPanel.GetComponent<RectTransform>();
                    Vector3 savedWorldPosition = changeRect.position;
                    Vector2 savedSize = changeRect.rect.size;
                    Vector2 originalPivot = changeRect.pivot;

                    // 修改轴心和锚点
                    changeRect.anchorMax = targerRect.anchorMax;
                    changeRect.anchorMin = targerRect.anchorMin;
                    changeRect.pivot = targerRect.pivot;

                    // 计算轴心点变化导致的偏移补偿
                    Vector2 pivotDelta = targerRect.pivot - originalPivot;
                    Vector2 positionCompensation = new Vector2(pivotDelta.x * savedSize.x, pivotDelta.y * savedSize.y);


                    changeRect.position = savedWorldPosition;
                    changeRect.anchoredPosition += positionCompensation;
                    AppearPanel.position = targerPanel.position;
                    AppearPanel.gameObject.SetActive(false);
                    AppearPanel.localScale = Vector3.one;

                    #endregion

                    // 设置锚点
                    RectTransform rectTransform = AppearPanel.GetComponent<RectTransform>();

                    // 保存当前位置和尺寸
                    savedWorldPosition = rectTransform.position;
                    savedSize = rectTransform.rect.size;
                    originalPivot = rectTransform.pivot;

                    // 修改锚点到右侧中心
                    rectTransform.anchorMin = new Vector2(1, 0.5f);
                    rectTransform.anchorMax = new Vector2(1, 0.5f);
                    rectTransform.pivot = new Vector2(1, 0.5f);

                    // 计算轴心点变化导致的偏移补偿
                    pivotDelta = new Vector2(1, 0.5f) - originalPivot;
                    positionCompensation = new Vector2(pivotDelta.x * savedSize.x, pivotDelta.y * savedSize.y);

                    rectTransform.position = savedWorldPosition;
                    rectTransform.anchoredPosition += positionCompensation;
                    AppearPanel.gameObject.SetActive(true);
                    AppearPanel.localScale = Vector3.zero;
                })
               .SetEase(Ease.InBack);
        }
        private Tween ChangeGiftPanelPostion(Transform changePanel, Transform targerPanel)
        {
            float durationTime = 0.6f;
            // 保存当前位置和尺寸
            RectTransform changeRect = changePanel.GetComponent<RectTransform>();
            RectTransform targerRect = targerPanel.GetComponent<RectTransform>();
            Vector3 savedWorldPosition = changeRect.position;
            Vector2 savedSize = changeRect.rect.size;
            Vector2 originalPivot = changeRect.pivot;

            // 修改轴心和锚点
            changeRect.anchorMax = targerRect.anchorMax;
            changeRect.anchorMin = targerRect.anchorMin;
            changeRect.pivot = targerRect.pivot;

            // 计算轴心点变化导致的偏移补偿
            Vector2 pivotDelta = targerRect.pivot - originalPivot;
            Vector2 positionCompensation = new Vector2(pivotDelta.x * savedSize.x, pivotDelta.y * savedSize.y);


            changeRect.position = savedWorldPosition;
            changeRect.anchoredPosition += positionCompensation;
            return changePanel.DOMove(targerPanel.position, durationTime);

        }
        #endregion
        private void ReFreshUI()
        {
            // 播放动画
            UIPlayAnimation();
        }
        private void OnPaySuccess()
        {
            isBuy = true;
            mPGModel.AddGiftLevel();
            UIKit.OpenPanel<UIBuyPackSuccess>();

        }
        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

        /// <summary>
        ///  购买项同时作为按钮点击的统一入口
        /// </summary>
        /// <returns></returns>
        public bool BuyItem()
        {
            if (CheckBuy() == true)
            {
                // 增加下一个等级
                mPGModel.AddRewardLevel();
                /*   ReFreshUI();*/
                mPGADActivity.DistributeReward(ReFreshUI, mPGModel.mPGData.Rewards[mPGModel.RewardLevel - 1].RewardItem);
                isBuy = false;
                return true;
            }
            return false;

        }
        public bool CheckBuy()
        {
            // 免费获取
            if (mPGModel.mPGData.Rewards[mPGModel.RewardLevel].Price == 0)
            {
                return true;
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("调用购买成功回调");
                Debug.Log(mPGModel.GiftLevel);
#endif
                googlePay.BuyProduct(GiftIDs[mPGModel.GiftLevel]);
                return isBuy;
            }
        }
    }
}