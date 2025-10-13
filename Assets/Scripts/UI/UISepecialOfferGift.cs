using UnityEngine;
using UnityEngine.UI;
using QFramework;
using TMPro;
using DG.Tweening;

namespace QFramework.Example
{
	public class UISepecialOfferGiftData : UIPanelData
	{
        public bool? IsManagedOpen;
    }
	public partial class UISepecialOfferGift : UIPanel
	{
		[SerializeField] private TextMeshProUGUI[] textRed;
		private Tween mCountDownTween;
		private SepecialOfferADActivity mSepecialOfferADActivity;

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UISepecialOfferGiftData ?? new UISepecialOfferGiftData();
			foreach (var i in textRed)
				i.font = LevelManager.Instance.redFont;

		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			mSepecialOfferADActivity = GameActivityManager.Instance.GetActivity<SepecialOfferADActivity>();
        }
		
		protected override void OnShow()
		{
            mCountDownTween = DOTween.To(() => 0, x =>
            {
                if (mSepecialOfferADActivity.ActivityStatus == GameActivityStatus.Active)
                    Time_Red.text = mSepecialOfferADActivity.GetActivityReamingTime();
                else
                    Time_Red.text = "Finished";
            }, 1, 1f)
          .SetLoops(-1, LoopType.Restart)
          .SetUpdate(isIndependentUpdate: true);

        }

        protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}
	}
}
