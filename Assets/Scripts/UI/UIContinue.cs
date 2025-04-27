using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class UIContinueData : UIPanelData
	{
	}
	public partial class UIContinue : UIPanel, ICanGetUtility
    {
        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }
        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIContinueData ?? new UIContinueData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
            var utility = this.GetUtility<SaveDataUtility>();
            var coin = utility.GetCoinNum();
            TxtCoin.text = coin.ToString();
            if (coin < 90)
            {
                TxtCoin.color = Color.red;
                TxtCoinCost.color = Color.red;
            }
            else
            {
                TxtCoin.color = Color.white;
                TxtCoinCost.color = Color.white;
            }

            BtnContinue.onClick.RemoveAllListeners();
			BtnContinue.onClick.AddListener(() =>
			{
				//���ǻ���90������¿�ʼ��Ϸ��������Ҫ�������֮���Ч��
				if (coin >= 90)
				{
                    utility.SetCoinNum(coin - 90);
                    LevelManager.Instance.RefreshLevel();
                    CloseSelf();
                }
				
            });

			BtnClose.onClick.RemoveAllListeners();
            BtnClose.onClick.AddListener(() =>
			{
				CloseSelf();
				UIKit.OpenPanel<UIDeleteLife>();
            });
        }
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}
	}
}
