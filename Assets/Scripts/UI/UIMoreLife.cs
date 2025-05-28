using UnityEngine;
using UnityEngine.UI;
using QFramework;
using GameDefine;

namespace QFramework.Example
{
	public class UIMoreLifeData : UIPanelData
	{
	}
	public partial class UIMoreLife : UIPanel, ICanGetUtility, ICanRegisterEvent
    {
        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }
        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIMoreLifeData ?? new UIMoreLifeData();
			// please add init code here
		}

        protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
			BindBtn();
            //RegisterEvent();//����
            //CheckVitality();//����
            //����������һЩUI����
            TxtNextHeart.text = HealthManager.Instance.CurRecoverySlot.ToString();
        }

        protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
        }

        private void BindBtn()
        {
            var coin = CoinManager.Instance.Coin;
            TxtCoinCost.color = coin < 900 ? Color.red : Color.white;

            BtnClose.onClick.AddListener(() =>
            {
                this.CloseSelf();
            });

            BtnCoinBuy.onClick.AddListener(() =>
            {
                if (HealthManager.Instance.UsedHp > 0)
                {
                    if (CoinManager.Instance.Coin < 900)
                    {
                        UIKit.ClosePanel<UIBeginSelect>();
                        CloseSelf();
                        StringEventSystem.Global.Send("OpenShopPanel");
                        return;
                    }
                    CoinManager.Instance.CostCoin(900, () =>
                    {
                        HealthManager.Instance.SetNowHpToMax();
                        //TxtCoinCost.color = CoinManager.Instance.Coin < 900 ? Color.red : Color.white;
                    });
                    CloseSelf();
                }
            });

            BtnAD.onClick.AddListener(() =>
            {
                TopOnADManager.Instance.ShowVideoAd(() => HealthManager.Instance.AddHp(), null);
            });
        }

        private void Update()
        {
            //�ж��Ƿ��������������������������Ӧ�ø���ʣ��ʱ�䵹��ʱ
            TxtTime.text = HealthManager.Instance.RecoverTimerStr;
            TxtNextHeart.text = HealthManager.Instance.CurRecoverySlot.ToString();
        }
    }
}
