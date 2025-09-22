using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class SceneUnlockGuideData : UIPanelData
	{
	}
	public partial class SceneUnlockGuide : UIPanel ,ICanGetModel
	{
		private bool mStep1 = false;
		private bool mStep2 = false;
		private Button mBtnStep1;
        private Button mBtnStep2;

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as SceneUnlockGuideData ?? new SceneUnlockGuideData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
            InitUI();
		}
		
		protected override void OnShow()
		{
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}

		private void InitUI()
		{
			SpineHandle.AnimationState.SetAnimation(0, "animation", true);
			mBtnStep1 = UIKit.GetPanel<UIBegin>().BtnArea;

            GameUtilityManager.Instance.GetLocalPositionInCanvas(
				mBtnStep1.GetComponent<RectTransform>(), SpineHandle.GetComponent<RectTransform>());

			mStep1 = true;
        }

        private void Update()
        {
            if(Input.GetMouseButtonDown(0))
			{
				if (mStep1
                    && GameUtilityManager.Instance.IsPointerOverTargetUI(Input.mousePosition, mBtnStep1.gameObject))
				{
                    mStep1 = false;
                    mBtnStep1.onClick?.Invoke();
                    mBtnStep2 = UIKit.GetPanel<SceneUnlock1>().BtnUnitUnlock;
                    GameUtilityManager.Instance.GetLocalPositionInCanvas(
                        mBtnStep2.GetComponent<RectTransform>(), SpineHandle.GetComponent<RectTransform>());
                    mStep2 = true;
				}
				else if (mStep2
                    && GameUtilityManager.Instance.IsPointerOverTargetUI(Input.mousePosition, mBtnStep2.gameObject))
				{
                    mStep2 = false;
                    mBtnStep2.onClick?.Invoke();
                    CloseSelf();
				}
			}
        }

        public IArchitecture GetArchitecture()
        {
			return GameMainArc.Interface;
        }
    }
}
