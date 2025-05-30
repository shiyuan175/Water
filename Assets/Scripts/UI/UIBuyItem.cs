using UnityEngine;
using UnityEngine.UI;
using QFramework;
using System.Collections.Generic;

namespace QFramework.Example
{
    [System.Serializable]
    public class BuyItemInfo
    {
        public string ItemName;
        public string ItemDescription;
        public int ItemCost;
		public int ItemNum;
        public Sprite ItemIcon;
    }

    public class UIBuyItemData : UIPanelData
	{
		public int item;
	}

	public partial class UIBuyItem : UIPanel, ICanGetUtility, ICanSendEvent, ICanGetModel
    {
        private StageModel stageModel;
        [SerializeField] private List<BuyItemInfo> buyItemInfos;

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIBuyItemData ?? new UIBuyItemData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
            stageModel = this.GetModel<StageModel>();
            var item = buyItemInfos[mData.item - 1];

            var needCoin = item.ItemCost;
            TxtCost.color = CoinManager.Instance.Coin < needCoin ? Color.red : Color.white;

            ImgItem.sprite = item.ItemIcon;
            TxtTitle.text = item.ItemName;
            TxtDesc.text = item.ItemDescription;
            TxtCost.text = item.ItemCost.ToString();
            TxtNum.text = $"X{item.ItemNum}";

            BtnClose.onClick.AddListener(() =>
			{
                CloseSelf();
            });
            BtnBuy.onClick.AddListener(() =>
            {
                if (CoinManager.Instance.Coin < needCoin)
                {
                    //������1-5�ĵ���/6-8�ĵ���(������ڲ�һ��)
                    if (mData.item > 5)
                    {
                        UIKit.ClosePanel<UIBeginSelect>();
                        CloseSelf();
                        StringEventSystem.Global.Send("OpenShopPanel");
                    }
                    else
                    {
                        UIKit.OpenPanel<UIShop>();
                        CloseSelf();
                    }
                    
                    return;
                }
                CoinManager.Instance.CostCoin(needCoin, () =>
				{
                    stageModel.AddItem(mData.item, item.ItemNum);
                });
                CloseSelf();
            });
        }
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
            BtnClose.onClick.RemoveAllListeners();
            BtnBuy.onClick.RemoveAllListeners();
        }
    }
}
