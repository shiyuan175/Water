using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:6d23facb-dfb8-4de0-ba17-1dc966f47498
	public partial class UIBattlePassADActivity
	{
		public const string Name = "UIBattlePassADActivity";
		
		
		private UIBattlePassADActivityData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UIBattlePassADActivityData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIBattlePassADActivityData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIBattlePassADActivityData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
