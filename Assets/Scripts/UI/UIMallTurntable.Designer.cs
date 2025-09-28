using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:371d2213-1304-442d-bb90-cf776a898029
	public partial class UIMallTurntable
	{
		public const string Name = "UIMallTurntable";
		
		[SerializeField]
		public TMPro.TextMeshProUGUI TextTitle;
		[SerializeField]
		public UnityEngine.UI.Image ImgTurn;
		[SerializeField]
		public RectTransform Pointer;
		[SerializeField]
		public UnityEngine.UI.Button BtnExit;
		[SerializeField]
		public UnityEngine.UI.Button BtnTurnTableRule;
		[SerializeField]
		public UnityEngine.UI.Image TextRuleBk;
		[SerializeField]
		public UnityEngine.UI.Button BtnBeginTurnTable;
		[SerializeField]
		public UnityEngine.UI.Image TextTipBk;
		[SerializeField]
		public TMPro.TextMeshProUGUI TextPlayTime;
		
		private UIMallTurntableData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			TextTitle = null;
			ImgTurn = null;
			Pointer = null;
			BtnExit = null;
			BtnTurnTableRule = null;
			TextRuleBk = null;
			BtnBeginTurnTable = null;
			TextTipBk = null;
			TextPlayTime = null;
			
			mData = null;
		}
		
		public UIMallTurntableData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIMallTurntableData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIMallTurntableData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
