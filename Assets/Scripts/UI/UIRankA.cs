using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UnityEngine.SocialPlatforms.Impl;

namespace QFramework.Example
{
	public class UIRankAData : UIPanelData 
	{
        public int LastRankScore; 
    }

	public partial class UIRankA : UIPanel, ICanGetModel
    {
        private RankNodePool mRankPool;
		private RankDataModel mRankDataModel;
		private PotionActivityModel mPotionActivityModel;

		[SerializeField] private Button Close;

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIRankAData ?? new UIRankAData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
            mRankPool = RankNodePool.Instance;
            mRankDataModel = this.GetModel<RankDataModel>();
            mPotionActivityModel = this.GetModel<PotionActivityModel>();

            mRankPool.RankRaise(mRankDataModel.GetPlayerRank((uiData as UIRankAData).LastRankScore),
               mPotionActivityModel.PotionActivityTotalGoal, RankScroll);
        }

        protected override void OnShow()
		{
            Close.onClick.AddListener(() =>
            {
				RankNodePool.Instance.ClearRankNode();
                CloseSelf();
            });
        }
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
            RankNodePool.Instance.ClearRankNode();

            if (GameActivityManager.Instance.GetActivity<VolcanicActivity>() is VolcanicActivity volcanicActivity
                && volcanicActivity.ActivityStatus == GameActivityStatus.Active)
            {
                UIKit.OpenPanel<UIVolcanicActivity>(new UIVolcanicActivityData()
                {
                    isSuceed = true
                });
            }
            else
                UIKit.OpenPanel<UIGetCoin>();

            Close.onClick.RemoveAllListeners();
            mRankPool = null;
            mRankDataModel = null;
            mPotionActivityModel = null;
        }
	}
}
