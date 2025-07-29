using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:551170ba-e1f1-432d-bf08-8909a7c1de59
	public partial class UIRankA
	{
		public const string Name = "UIRankA";
		
		[SerializeField]
		public UnityEngine.UI.ScrollRect RankScroll;
		
		private UIRankAData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			RankScroll = null;
			
			mData = null;
		}
		
		public UIRankAData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIRankAData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIRankAData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
