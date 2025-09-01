using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:f2d8a02d-1526-412c-8827-046da2315dee
	public partial class UILessStar
	{
		public const string Name = "UILessStar";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public UnityEngine.UI.Button BtnContinue;
		
		private UILessStarData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnClose = null;
			BtnContinue = null;
			
			mData = null;
		}
		
		public UILessStarData Data
		{
			get
			{
				return mData;
			}
		}
		
		UILessStarData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UILessStarData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
