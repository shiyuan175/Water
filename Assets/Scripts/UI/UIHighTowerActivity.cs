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
        [SerializeField] private GameObject mEndNode;
        [SerializeField] private GameObject mBlueNode;
        [SerializeField] private GameObject mRedNode;
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
        }

        protected override void OnShow()
		{
			BtnClose.onClick.AddListener(() =>
			{
				CloseSelf();
			});

            HTANodeCtrl _node;
            if (mHighTowerActivity.IsAtBaseNode)
                _node = Instantiate(mStartNode, mParNode).GetComponent<HTANodeCtrl>();
            else if (mHighTowerActivity.IsAtTopNode)
                _node = Instantiate(mEndNode, mParNode).GetComponent<HTANodeCtrl>();
            else
            {
                //根据索引判断使用预制体(奇 -> 蓝,偶 -> 红)
                var _nextStage = mHighTowerActivity.NextRewardStageIndex;
                GameObject _prefabToUse = (_nextStage % 2 == 1) ? mBlueNode : mRedNode;
                _node = Instantiate(_prefabToUse, mParNode).GetComponent<HTANodeCtrl>();
            }
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

            if (mData.IsManagedOpen ?? false)
                StringEventSystem.Global.Send(GameDefine.GameConst.MANAGER_OPEN_NEXT_PANEL);
        }

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }
    }
}
