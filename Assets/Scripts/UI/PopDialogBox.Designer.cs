using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:12aa9b4f-ed64-4f52-903e-79105e247ecb
	public partial class PopDialogBox
	{
		public const string Name = "PopDialogBox";
		
		[SerializeField]
		public UnityEngine.UI.Image Mask;
		[SerializeField]
		public UnityEngine.RectTransform DialogBox;
		[SerializeField]
		public TMPro.TextMeshProUGUI TxtDialogBox;
		[SerializeField]
		public Spine.Unity.SkeletonGraphic HandleSpine;
		
		private PopDialogBoxData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Mask = null;
			DialogBox = null;
			TxtDialogBox = null;
			HandleSpine = null;
			
			mData = null;
		}
		
		public PopDialogBoxData Data
		{
			get
			{
				return mData;
			}
		}
		
		PopDialogBoxData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new PopDialogBoxData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
