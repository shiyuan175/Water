using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:323da4d2-310c-45a1-90e4-f50968ec9274
	public partial class SceneUnlock1
	{
		public const string Name = "SceneUnlock1";
		
		[SerializeField]
		public UnityEngine.UI.Image ImgBox;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtRemainStar;
		[SerializeField]
		public UnityEngine.UI.Button BtnUnitUnlock;
		[SerializeField]
		public UnityEngine.UI.Image ImgUnitIcon;
		[SerializeField]
		public UnityEngine.UI.Text TxtNeedStar;
		[SerializeField]
		public UnityEngine.UI.Button BtnReturen;
		[SerializeField]
		public RectTransform FlightEffectsToBox;
		[SerializeField]
		public RectTransform FlightEffectsToBtn;
		
		private SceneUnlock1Data mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			ImgBox = null;
			TxtRemainStar = null;
			BtnUnitUnlock = null;
			ImgUnitIcon = null;
			TxtNeedStar = null;
			BtnReturen = null;
			FlightEffectsToBox = null;
			FlightEffectsToBtn = null;
			
			mData = null;
		}
		
		public SceneUnlock1Data Data
		{
			get
			{
				return mData;
			}
		}
		
		SceneUnlock1Data mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new SceneUnlock1Data());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
