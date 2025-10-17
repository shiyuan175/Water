using DG.Tweening;
using GameDefine;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

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
			TxtNum.text = $"X{mData.Goals}";
			mSequence.Append(ImgCup.transform.DOScale(1.8f, 0.5f));
            mSequence.Append(ImgCup.transform.DOScale(1.5f, 0.3f));

            //双倍获取生效
            if (!CountDownTimerManager.Instance.IsTimerFinished(GameEnum.GetDescription(SpecialRewardsType.UnlimitedDoubleBuff)))
            {
                mSequence.AppendCallback(() =>
                {
                    ImgDouble.Show();
                });

                mSequence.Append(ImgDouble.transform.DOScale(0f, 0.5f).From(1.5f));
				mSequence.Join(ImgDouble.transform.DOMove(ImgCup.transform.position, 0.5f));
                mSequence.Append(DOTween.To(() => mData.Goals, value =>
				{
					TxtNum.text = $"X{value}";

                }, mData.Goals * mStageModel.SettlementMultiple, 0.5f));
            }

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
