using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:422ddc5e-5086-446a-b1e9-7c2e51efc363
	public partial class UIMallTurntable
	{
		public const string Name = "UIMallTurntable";
		
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
