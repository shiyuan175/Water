using UnityEngine;
using UnityEngine.UI;
using QFramework;
using TMPro;

namespace QFramework.Example
{
	public class UIGuideLevelRemoveHideData : UIGuideLevelData
    {
	}
	public partial class UIGuideLevelRemoveHide : UIGuideLevel
    {
         [SerializeField]
        GameObject setpGetItem;

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UIGuideLevelRemoveHideData ?? new UIGuideLevelRemoveHideData();
            // please add init code here
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            TxtGuide.font = LevelManager.Instance.blueFont;
        }
        private readonly Vector3 mStepGetItemPos = new(-227, -815, 0);
        protected override void OnShow()
        {
            SpineHandle.AnimationState.SetAnimation(0, "animation", true);
            BtnItem.onClick.AddListener(() =>
            {
                LevelManager.Instance.RemoveHide();
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
