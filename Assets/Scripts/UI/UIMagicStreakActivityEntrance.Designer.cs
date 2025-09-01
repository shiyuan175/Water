using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:75ed1528-5064-41e5-bbd1-3b1738f96314
	public partial class UIMagicStreakActivityEntrance
	{
		public const string Name = "UIMagicStreakActivityEntrance";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnClose;
		[SerializeField]
		public UnityEngine.UI.Button BtnStart;
		
		private UIMagicStreakActivityEntranceData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnClose = null;
			BtnStart = null;
			
			mData = null;
		}
		
		public UIMagicStreakActivityEntranceData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIMagicStreakActivityEntranceData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIMagicStreakActivityEntranceData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
