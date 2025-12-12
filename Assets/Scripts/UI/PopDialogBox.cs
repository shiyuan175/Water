using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class PopDialogBoxData : UIPanelData
	{
		public bool EnableMask = false;
		//是否需要上下翻转(默认气泡角朝下)
		public bool EnableFlip = false;

        //是否使用对话框(使用时，传入要设置位置的Rect 和 文字内容)
        public RectTransform DialogBoxPosNode;
		public string DialogBoxMes;

        //是否使用点击引导(使用时，传入传入要设置位置的Rect)
        public RectTransform HandleSpineNode;

		public float? AutoClose;
    }

	public partial class PopDialogBox  : UIPanel
	{
        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as PopDialogBoxData ?? new PopDialogBoxData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			if (mData.AutoClose.HasValue && mData.AutoClose.Value > 0)
                ActionKit.Delay(mData.AutoClose.Value, () => CloseSelf()).Start(this);

            if (mData.EnableMask)
				Mask.Show();

			//取0时气泡角朝上
			if (mData.EnableFlip)
			{
				DialogBox.transform.localEulerAngles = Vector3.zero;
                TxtDialogBox.transform.localEulerAngles = Vector3.zero;
            }

			if (mData.DialogBoxPosNode)
			{
                DialogBox.Show();
				GameUtilityManager.Instance.GetLocalPositionInCanvas(mData.DialogBoxPosNode , DialogBox);
                TxtDialogBox.font = LevelManager.Instance.blueFont;
				TxtDialogBox.text = mData.DialogBoxMes;
            }

			if (mData.HandleSpineNode)
			{
                HandleSpine.Show();
                GameUtilityManager.Instance.GetLocalPositionInCanvas(mData.HandleSpineNode, HandleSpine.GetComponent<RectTransform>());
                HandleSpine.AnimationState.SetAnimation(0, "animation", true);
            }
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

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                CloseSelf();
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
               CloseSelf();
            }
        }
    }
}
