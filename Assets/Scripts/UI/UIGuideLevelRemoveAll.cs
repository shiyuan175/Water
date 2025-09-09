using UnityEngine;
using UnityEngine.UI;
using QFramework;
using TMPro;

namespace QFramework.Example
{
	public class UIGuideLevelRemoveAllData : UIGuideLevelData
    {
	}
	public partial class UIGuideLevelRemoveAll : UIGuideLevel
    {
        private readonly Vector3 mStep2HandlePos = new(-190, -50, 0);
		private readonly Vector3 mStep3HandlePos = new (190, -50, 0);

        private bool mIsStep2;

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIGuideLevelRemoveAllData ?? new UIGuideLevelRemoveAllData();
			// please add init code here
		}
        protected override void OnOpen(IUIData uiData = null)
        {
            
        }
        private readonly Vector3 mStepGetItemPos = new(430, -815, 0);
        protected override void OnShow()
        {
            SpineHandle.AnimationState.SetAnimation(0, "animation", true);
            BtnItem.onClick.AddListener(() =>
            {
                LevelManager.Instance.RemoveAll();
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
