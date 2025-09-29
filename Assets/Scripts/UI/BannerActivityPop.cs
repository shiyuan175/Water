using UnityEngine;
using UnityEngine.UI;
using QFramework;
using DG.Tweening;

namespace QFramework.Example
{
	public class BannerActivityPopData : UIPanelData
	{
		public int Goals;
		public Vector3 TargetPos;
	}

	public partial class BannerActivityPop : UIPanel ,ICanGetModel
	{
		private Sequence mSequence;
        private StageModel mStageModel;

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as BannerActivityPopData ?? new BannerActivityPopData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			mStageModel = this.GetModel<StageModel>();

            mSequence = DOTween.Sequence();
			TxtNum.font = LevelManager.Instance.redFont;
			TxtNum.text = $"X{mData.Goals * mStageModel.SettlementMultiple}";
			mSequence.Append(ImgCup.transform.DOScale(1.8f, 0.5f));
            mSequence.Append(ImgCup.transform.DOScale(1.5f, 0.3f));
			mSequence.Append(ImgCup.transform.DOMove(mData.TargetPos, 0.7f));
			mSequence.Join(ImgCup.transform.DOScale(0.5f, 0.7f));

			mSequence.OnComplete(() =>
			{
				CloseSelf();
			});
        }

        protected override void OnShow()
		{
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
			mSequence.Kill();
            mSequence = null;
        }

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }
    }
}
