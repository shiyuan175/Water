using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:dccdba0b-2310-48b3-9f16-44f7925d7212
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
