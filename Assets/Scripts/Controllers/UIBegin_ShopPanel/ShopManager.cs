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

        private List<GiftPackCtrl> giftPacksCache;
        private Dictionary<string, Action> giftPackBuySuccessActions;
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

            giftPacksCache = new List<GiftPackCtrl>();
            // 初始化购买成功回调
            giftPackBuySuccessActions = new Dictionary<string, Action>();
            foreach (var btn in buyGiftPackBtns)
            {
                if (!btn.TryGetComponent<GiftPackCtrl>(out GiftPackCtrl _giftPack))
                    continue;
                giftPacksCache.Add(_giftPack);
                var _packSo = _giftPack.GiftPackSO;
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
                bool? isPurchased = (bool?)gameGlobalModel.GetFieldValue(gameGlobalModel.GameGlobalJsonData.GiftPackPurchases, pack.GiftPackSO.ID);
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
                if (!btn.TryGetComponent<GiftPackCtrl>(out GiftPackCtrl _giftPack))
                    continue;
                var _packSo = _giftPack.GiftPackSO;

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
        private void OnPaySuccess(GiftPackCtrl giftPack)
        {
            //获取购买记录(每个礼包首次购买附带特权),
            //购买礼包时会根据礼包ID进行校验,而像金币礼包这种不需要验证(不带特权),用空值接收
            var purchased = (bool?)gameGlobalModel.GetFieldValue(gameGlobalModel.GameGlobalJsonData.GiftPackPurchases,
                giftPack.GiftPackSO.ID);
            var ability = purchased.HasValue && purchased.Value == false 
                ? giftPack.AbilityPackSO : null;

            //发放奖励与表现
            rewardGrantUtility.GrantReward(giftPack.GiftPackSO);
            if (purchased.HasValue && !purchased.Value)
            {
                rewardGrantUtility.GrantReward(ability);
                gameGlobalModel.SetFieldAndSave(JsonType.GameGlobalJson, gameGlobalModel.GameGlobalJsonData.GiftPackPurchases,
                    giftPack.GiftPackSO.ID, true);
            }
            RewardUIManager.Instance.PlayRewardAnim(giftPack.GiftPackSO.Coins, true, null, giftPack.GiftPackSO, ability);

            //禁用特权礼包UI
            giftPack.DisableProduct();
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
