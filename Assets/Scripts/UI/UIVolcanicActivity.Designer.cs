using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:4f14aabf-5750-45b4-b2ca-05b03d7feeed
	public partial class UIVolcanicActivity
	{
		public const string Name = "UIVolcanicActivity";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public TMPro.TextMeshProUGUI Txt_Prompt;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtLevels;
		[SerializeField]
		public TMPro.TextMeshProUGUI Txt_Levels;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtPlayers;
		[SerializeField]
		public TMPro.TextMeshProUGUI Txt_Players;
		
		private UIVolcanicActivityData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnClose = null;
			Txt_Prompt = null;
			TxtLevels = null;
			Txt_Levels = null;
			TxtPlayers = null;
			Txt_Players = null;
			
			mData = null;
		}
		
		public UIVolcanicActivityData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIVolcanicActivityData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIVolcanicActivityData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
