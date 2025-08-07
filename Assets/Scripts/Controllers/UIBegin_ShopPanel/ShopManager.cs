using UnityEngine;
using QFramework;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Collections;

namespace QFramework.Example
{
	public partial class ShopManager : ViewController, IController
	{
        [SerializeField] private List<Button> buyGiftPackBtns;
        [SerializeField] private List<TMPro.TextMeshProUGUI> mRedTMP;
        [SerializeField] private List<TMPro.TextMeshProUGUI> mBlueTMP;
        [SerializeField] private List<TMPro.TextMeshProUGUI> mGreenTMP;

        private GooglePayManager googlePay;
        private Dictionary<string ,Action> giftPackBuySuccessActions;
        private StageModel stageModel;

        private void Awake()
        {
            foreach (var item in mRedTMP)
            {
                item.font = LevelManager.Instance.redFont;
            }

            foreach (var item in mBlueTMP)
            {
                item.font = LevelManager.Instance.blueFont;
            }

            foreach (var item in mGreenTMP)
            {
                item.font = LevelManager.Instance.greenFont;
            }

            googlePay = GooglePayManager.Instance;
            stageModel = this.GetModel<StageModel>();

            // 初始化购买成功回调
            giftPackBuySuccessActions = new Dictionary<string, Action>();
            foreach (var btn in buyGiftPackBtns)
            {
                if (!btn.TryGetComponent<GiftPack>(out GiftPack _giftPack))
                    continue;
                var _packSo = _giftPack.giftPack;
                giftPackBuySuccessActions[_packSo.ID] = () => OnPaySuccess(_packSo);
            }
        }

        private void OnEnable()
        {
            ShopScrollView.verticalNormalizedPosition = 1f;

            // 注册购买成功事件
            foreach (var kvp in giftPackBuySuccessActions)
            {
                StringEventSystem.Global.Register(kvp.Key, kvp.Value).UnRegisterWhenGameObjectDestroyed(gameObject);
            }
        }

        private void OnDisable()
        {
            // 卸载购买成功事件(避免从UIKit打开商店购买导致重复发放奖励)
            foreach (var kvp in giftPackBuySuccessActions)
            {
                StringEventSystem.Global.UnRegister(kvp.Key, kvp.Value);
            }
        }

        private void Start()
		{
            //注册按钮
            foreach (var btn in buyGiftPackBtns)
            {
                if (!btn.TryGetComponent<GiftPack>(out GiftPack _giftPack))
                    continue;
                var _packSo = _giftPack.giftPack;

                btn.onClick.AddListener(() => BuyGiftPackEvent(_packSo));
            }
        }

        /// <summary>
        /// 购买礼包事件
        /// </summary>
        /// <param name="_packSo"></param>
        private void BuyGiftPackEvent(GiftPackSO _packSo)
        {
            //Debug.Log("礼包ID ： " + _packSo.ID);
            googlePay.BuyProduct(_packSo.ID);
        }

        /// <summary>
        /// 礼包购买成功回调
        /// </summary>
        private void OnPaySuccess(GiftPackSO _packSo)
        {
            StartCoroutine(RewardUIManager.Instance.PlayRewardAnim(_packSo,true));

            //金币发放
            CoinManager.Instance.AddCoin(_packSo.Coins);
            //道具发放
            foreach (var item in _packSo.ItemReward)
            {
                stageModel.AddItem(item.ItemIndex, item.Quantity);
            }
            //无限体力发放
            HealthManager.Instance.SetUnLimitHp(_packSo.UnlimitedHp);
            //无限道具
            CountDownTimerManager.Instance.AddTimer(GameDefine.GameConst.UNLIMIT_ITEM_SIGN, _packSo.UnlimitedHp);
            UIKit.OpenPanel<UIBuyPackSuccess>();
            ActionKit.Delay(1, () =>
            {
                UIKit.ClosePanel<UIShop>();//延迟1s等待协程结束关闭
            }).Start(this);
        }

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }
    }
}
