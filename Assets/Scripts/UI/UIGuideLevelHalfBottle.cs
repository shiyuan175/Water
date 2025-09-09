using UnityEngine;
using UnityEngine.UI;
using QFramework;
using TMPro;

namespace QFramework.Example
{
	public class UIGuideLevelHalfBottleData : UIGuideLevelData
    {
	}
	public partial class UIGuideLevelHalfBottle : UIGuideLevel
    {
        private readonly Vector3 mStep2HandlePos = new(-190, -50, 0);
		private readonly Vector3 mStep3HandlePos = new (190, -50, 0);

        private bool mIsStep2;

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIGuideLevelHalfBottleData ?? new UIGuideLevelHalfBottleData();
			// please add init code here
		}
        protected override void OnOpen(IUIData uiData = null)
        {
            TxtGuide.font = LevelManager.Instance.blueFont;
        }
        private readonly Vector3 mStepGetItemPos = new(207, -810, 0);
        protected override void OnShow()
        {
            SpineHandle.AnimationState.SetAnimation(0, "animation", true);
            BtnItem.onClick.AddListener(() =>
            {
                LevelManager.Instance.AddBottle(true);
                CloseSelf();
            });
            BtnGet.onClick.AddListener(() =>
            {
                this.SendEvent<UnLockItem>();
                StepGetItem.Hide();
                SpineHandle.gameObject.SetActive(true);
                SetLocalPosition(SpineHandle.transform, mStepGetItemPos);
                StepItem.Show();
            });
            SetpGetItem();
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            BtnGet.onClick.RemoveAllListeners();
            BtnItem.onClick.RemoveAllListeners();
        }

        protected void SetpGetItem()
        {
            StepGetItem.Show();
            SpineHandle.gameObject.SetActive(false);
        }
    }
}
