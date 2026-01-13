using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:df605b5c-b64f-4042-a87a-cf28b2aad6d0
	public partial class UIBattlePassPay
	{
		public const string Name = "UIBattlePassPay";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnBuy;
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		
		private UIBattlePassPayData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnBuy = null;
			BtnClose = null;
			
			mData = null;
		}
		
		public UIBattlePassPayData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIBattlePassPayData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIBattlePassPayData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
