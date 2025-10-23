using UnityEngine;
using UnityEngine.UI;
using QFramework;
using DG.Tweening;

namespace QFramework.Example
{
	public class UIHighTowerActivityData : UIPanelData
	{
        public bool? isSuceed;
        public bool? IsManagedOpen;
    }

	public partial class UIHighTowerActivity : UIPanel ,ICanGetUtility
	{
		[SerializeField] private GameObject mStartNode;
        [SerializeField] private GameObject mMidNode;
        [SerializeField] private GameObject mEndNode;
        [SerializeField] private Transform mParNode;
		//各阶段对应礼包
		[SerializeField] private RewardPackSO[] mRewardPackSOs;

		private HighTowerActivity mHighTowerActivity;
        private Tween mCountDownTween;

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIHighTowerActivityData ?? new UIHighTowerActivityData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			mHighTowerActivity = GameActivityManager.Instance.GetActivity<HighTowerActivity>();
            mCountDownTween = DOTween.To(() => 0, x =>
            {
                TxtCountDown.text = mHighTowerActivity.GetActivityReamingTime();
            }, 1, 1f)
           .SetLoops(-1, LoopType.Restart)
           .SetUpdate(true);

            BtnClose.onClick.AddListener(() =>
            {
                CloseSelf();
            });
        }

        protected override void OnShow()
		{
            //经测试:AB包关闭时，预制体会被自动销毁
            HTANodeCtrl _node;
            if (mHighTowerActivity.IsAtBaseNode)
                _node = Instantiate(mStartNode, mParNode).GetComponent<HTANodeCtrl>();
            else if (mHighTowerActivity.IsAtTopNode)
                _node = Instantiate(mEndNode, mParNode).GetComponent<HTANodeCtrl>();
            else
                _node = Instantiate(mMidNode, mParNode).GetComponent<HTANodeCtrl>();
            //Debug.Assert(_node != null, "HTANodeCtrl not found on instantiated node.");
            _node.Init(mHighTowerActivity);

            if (mData.isSuceed == null)
                return;

            //NextRewardStageIndex - 1(因为索引0是占位取不到)
            //NextRewardStageIndex会在数据变更之前使用
            _node.PlayTween((bool)mData.isSuceed, mRewardPackSOs[mHighTowerActivity.NextRewardStageIndex - 1],
                ()=>this.GetUtility<RewardGrantUtility>().GrantReward(mRewardPackSOs[mHighTowerActivity.NextRewardStageIndex - 1]));
        }
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
            BtnClose.onClick.RemoveAllListeners();
            mHighTowerActivity = null;
           
            mCountDownTween?.Kill();

            if (mData.IsManagedOpen ?? false)
                StringEventSystem.Global.Send(GameDefine.GameConst.MANAGER_OPEN_NEXT_PANEL);
        }

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }
    }
}
