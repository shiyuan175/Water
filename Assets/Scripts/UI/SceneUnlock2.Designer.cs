
using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public partial class SceneUnlock2
	{
		public const string Name = "SceneUnlock2";
		
		[SerializeField]
		public Spine.Unity.SkeletonGraphic SpineHeartRise;
		[SerializeField]
		public UnityEngine.UI.Image ImgReward;
		[SerializeField]
		public UnityEngine.UI.Button BtnBox;
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
			SpineHeartRise = null;
			ImgReward = null;
			BtnBox = null;
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
