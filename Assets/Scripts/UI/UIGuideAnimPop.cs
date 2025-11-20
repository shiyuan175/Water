using UnityEngine;
using UnityEngine.UI;
using QFramework;
using static ATUnityCBridge;
using System;
using DG.Tweening;

namespace QFramework.Example
{
	public class UIGuideAnimPopData : UIPanelData
	{
		public string GuideText;
		public RectTransform Node1;
		public RectTransform Node2;
    }
	public partial class UIGuideAnimPop : UIPanel
	{
		private Vector2 pos1;
		private Vector2 pos2;
        private Sequence seq;

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIGuideAnimPopData ?? new UIGuideAnimPopData();
			// please add init code here
		}

		protected override void OnOpen(IUIData uiData = null)
		{
            seq = DOTween.Sequence();
            TxtGuide.text = mData.GuideText;
            Canvas.ForceUpdateCanvases();
			if (mData.Node1 != null)
                pos1 = ChangePosition(mData.Node1, GuideArrow);
            if (mData.Node2 != null)
                pos2 = ChangePosition(mData.Node2, GuideArrow);

            GuideArrow.anchoredPosition = pos1;

            if (mData.Node2 != null)
                PlayArrowTween();
            else
                PlaySinglePointYTween();
        }

        protected override void OnShow()
		{
            ActionKit.Delay(5f, () =>
            {
                CloseSelf();
            }).Start(this);
        }
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
            seq?.Kill();
            seq = null;
        }

		private Vector2 ChangePosition(RectTransform sourceObj, RectTransform targetObj)
		{
            Canvas _sourceObjCanvas = sourceObj.GetComponentInParent<Canvas>().rootCanvas;
            Canvas _targetObjCanvas = targetObj.GetComponentInParent<Canvas>().rootCanvas;

            var _screenPoint = _sourceObjCanvas.worldCamera.WorldToScreenPoint(sourceObj.position);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                 targetObj.parent as RectTransform,
                 _screenPoint,
                 _targetObjCanvas.worldCamera,
                 out Vector2 _localPoint);
            
            return _localPoint;
		}

        private void PlayArrowTween()
        {
            seq.Append(GuideArrow.DOAnchorPos(pos2, 0.5f).SetEase(Ease.InOutSine));
            seq.Append(GuideArrow.DOAnchorPos(pos1, 0.5f).SetEase(Ease.InOutSine));
            seq.Append(GuideArrow.DOAnchorPos(pos2, 0.5f).SetEase(Ease.InOutSine));
            seq.Append(GuideArrow.DOAnchorPos(pos1, 0.5f).SetEase(Ease.InOutSine));
        }

        private void PlaySinglePointYTween()
        {
            float offset = 50f;
            float duration = 0.5f; 
            seq.Append(GuideArrow.DOAnchorPosY(pos1.y - offset, duration).SetEase(Ease.InOutSine));
            seq.Append(GuideArrow.DOAnchorPosY(pos1.y, duration).SetEase(Ease.InOutSine));
            seq.Append(GuideArrow.DOAnchorPosY(pos1.y - offset, duration).SetEase(Ease.InOutSine));
            seq.Append(GuideArrow.DOAnchorPosY(pos1.y, duration).SetEase(Ease.InOutSine));
        }
    }
}
