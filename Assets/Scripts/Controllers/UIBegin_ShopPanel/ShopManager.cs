using UnityEngine;
using QFramework;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using GameGlobalJson;

namespace QFramework.Example
{
	public partial class ShopManager : ViewController, IController
	{
        [SerializeField] private List<Button> buyGiftPackBtns;
        [SerializeField] private List<TMPro.TextMeshProUGUI> mRedTMP;
        [SerializeField] private List<TMPro.TextMeshProUGUI> mBlueTMP;
        [SerializeField] private List<TMPro.TextMeshProUGUI> mGreenTMP;

        private List<GiftPack> giftPacksCache;
        private Dictionary<string ,Action> giftPackBuySuccessActions;
        private GooglePayManager googlePay;
        private GameGlobalModel gameGlobalModel;
        private RewardGrantUtility rewardGrantUtility;

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
            gameGlobalModel = this.GetModel<GameGlobalModel>();
            rewardGrantUtility = this.GetUtility<RewardGrantUtility>();

            giftPacksCache = new List<GiftPack>();
            // 初始化购买成功回调
            giftPackBuySuccessActions = new Dictionary<string, Action>();
            foreach (var btn in buyGiftPackBtns)
            {
                if (!btn.TryGetComponent<GiftPack>(out GiftPack _giftPack))
                    continue;
                giftPacksCache.Add(_giftPack);
                var _packSo = _giftPack.giftPack;
                giftPackBuySuccessActions[_packSo.ID] = () => OnPaySuccess(_giftPack);
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

            // 获取特权礼包的购买记录
            foreach (var pack in giftPacksCache)
            {
                bool? isPurchased = (bool?)gameGlobalModel.GetFieldValue(gameGlobalModel.GameGlobalJsonData.GiftPackPurchases, pack.giftPack.ID);
                if (isPurchased is true)
                    pack.DisableProduct();
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
        private void OnPaySuccess(GiftPack giftPack)
        {
            //发放奖励、记录特权礼包购买
            rewardGrantUtility.GrantReward(giftPack.giftPack);
            gameGlobalModel.SetFieldAndSave(JsonType.GameGlobalJson, gameGlobalModel.GameGlobalJsonData.GiftPackPurchases,
                giftPack.giftPack.ID, true);

            //奖励发放表现
            RewardUIManager.Instance.PlayRewardAnim(giftPack.giftPack.Coins, true, null, giftPack.giftPack);
            //禁用礼包对象
            //giftPack.DisableProduct();
            UIKit.OpenPanel<UIBuyPackSuccess>();

            //这是用于AB包唤起的商店
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
