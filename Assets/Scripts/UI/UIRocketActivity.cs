using UnityEngine;
using UnityEngine.UI;
using QFramework;
using DG.Tweening;
using System.Collections.Generic;

namespace QFramework.Example
{
	public class UIRocketActivityData : UIPanelData
	{
        public bool? isSuceed;
        public bool? IsManagedOpen;
    }

	public partial class UIRocketActivity : UIPanel ,ICanSendEvent
	{
        private const int REWARD_COIN = 1000;

        //起始、终点点位(做移动线性插值)
        [SerializeField] private RectTransform mStartPos;
        [SerializeField] private RectTransform mEndPos;

        [SerializeField] private Image mPlayerFrame;
        [SerializeField] private Image mPlayerAvatar;
		[SerializeField] private Image[] mRobotsFrame;
        [SerializeField] private Image[] mRobotsAvatar;

		[SerializeField] private RectTransform mPlayerCat;
		[SerializeField] private RectTransform mRobot1Cat;
        [SerializeField] private RectTransform mRobot2Cat;

        //动画容器，玩家第一个播放，机器人动画可以同时播放
        private List<Tween> mTweenList;
        private RocketActivity mRocketActivity;

        private float mTotalDistance;

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIRocketActivityData ?? new UIRocketActivityData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			InitAvatarImg();
            mRocketActivity = GameActivityManager.Instance.GetActivity<RocketActivity>();
            mTweenList = new List<Tween>();

            Canvas.ForceUpdateCanvases();
            //中心点偏移
            float _pivotOffset1 = (0.5f - mEndPos.pivot.y) * mEndPos.rect.height;
            float _pivotOffset2 = (0.5f - mStartPos.pivot.y) * mStartPos.rect.height;
            mTotalDistance = (mEndPos.localPosition.y + _pivotOffset1) - (mStartPos.localPosition.y + _pivotOffset2);

            if (mRocketActivity.ActivityStatus == GameActivityStatus.Active)
                TxtDailyRefresh.text = $"{mRocketActivity.DailyUsedRefreshCount}/3 Today";
            else
                TxtDailyRefresh.text = "Finished";

            Tween _countDownTween = DOTween.To(() => 0, x =>
            {
                TxtRefreshCountDown.text = mRocketActivity.GetActivityReamingTime();
            }, 1, 1f)
            .SetLoops(-1, LoopType.Restart)
            .SetUpdate(true);
            mTweenList.Add(_countDownTween);
        }

        protected override void OnShow()
		{
            BtnClose.onClick.AddListener(() =>
            {
                CloseSelf();
            });

            int _playerStreakWin = mRocketActivity.PlayerStreakWin;
            int _robot1StreakWin = mRocketActivity.Robot1StreakWin;
            int _robot2StreakWin = mRocketActivity.Robot2StreakWin;
            Txt_PlayerWin.text = _playerStreakWin.ToString();
            Txt_Robot1Win.text = _robot1StreakWin.ToString();
            Txt_Robot2Win.text = _robot2StreakWin.ToString();

            mPlayerCat.anchoredPosition = new Vector2(mPlayerCat.anchoredPosition.x,
                mStartPos.anchoredPosition.y + (_playerStreakWin / (float)mRocketActivity.RAMaxStreakWinNum) * mTotalDistance);
            mRobot1Cat.anchoredPosition = new Vector2(mRobot1Cat.anchoredPosition.x,
                mStartPos.anchoredPosition.y + (_robot1StreakWin / (float)mRocketActivity.RAMaxStreakWinNum) * mTotalDistance);
            mRobot2Cat.anchoredPosition = new Vector2(mRobot2Cat.anchoredPosition.x,
                mStartPos.anchoredPosition.y + (_robot2StreakWin / (float)mRocketActivity.RAMaxStreakWinNum) * mTotalDistance);
            Txt_Prompt.text = $"Beat {mRocketActivity.RAMaxStreakWinNum - _playerStreakWin} levels on your first try before others to win!";

			if (mData.isSuceed == null)
				return;

            if ((bool)mData.isSuceed)
            {
                mRocketActivity.StreakWin();
                Txt_Prompt.text = $"Beat {mRocketActivity.RAMaxStreakWinNum - mRocketActivity.PlayerStreakWin} levels on your first try before others to win!";

                //防闭包
                bool _playerWin = mRocketActivity.PlayWin;
                bool _robotWin = mRocketActivity.RobotWin;

                if (_playerWin)
                {
                    UIKit.OpenPanel<UIMask>();
                    CoinManager.Instance.AddCoin(REWARD_COIN);
                    Txt_Prompt.text = $"You win!";
                    //Debug.Log("玩家获胜-发放奖励");
                }
                var _targetPos = mStartPos.anchoredPosition.y + 
                    (mRocketActivity.PlayerStreakWin / (float)mRocketActivity.RAMaxStreakWinNum) * mTotalDistance;
                Tween _playerUp = mPlayerCat.DOAnchorPosY(_targetPos, 2f).SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        if (_playerWin)
                        {
                            UIKit.ClosePanel<UIMask>();
                            RewardUIManager.Instance.PlayRewardAnim(REWARD_COIN, true, null);
                            //Debug.Log("玩家胜利回调播放奖励动画");
                        }
                    });
                mTweenList.Add(_playerUp);

                Tween _platerUI = DOTween.To(() => _playerStreakWin,
                    x => Txt_PlayerWin.text = $"{x}",
                    mRocketActivity.PlayerStreakWin, 1f);
                mTweenList.Add(_platerUI);

                if (!_playerWin)
                {
                    var _robot1TargetPos = mStartPos.anchoredPosition.y + (mRocketActivity.Robot1StreakWin / (float)mRocketActivity.RAMaxStreakWinNum) * mTotalDistance;
                    var _robot2TargetPos = mStartPos.anchoredPosition.y + (mRocketActivity.Robot2StreakWin / (float)mRocketActivity.RAMaxStreakWinNum) * mTotalDistance;
                    Tween _robot1Up = mRobot1Cat.DOAnchorPosY(_robot1TargetPos, 2f).SetEase(Ease.OutBack);
                    Tween _robot2Up = mRobot2Cat.DOAnchorPosY(_robot2TargetPos, 2f).SetEase(Ease.OutBack);
                    mTweenList.Add(_robot1Up);
                    mTweenList.Add(_robot2Up);

                    //机器人文本更新
                    Tween _robot1UI = DOTween.To(() => _robot1StreakWin,
                        x => Txt_Robot1Win.text = $"{x}",
                        mRocketActivity.Robot1StreakWin, 1f);
                    mTweenList.Add(_robot1UI);

                    Tween _robot2UI = DOTween.To(() => _robot2StreakWin,
                        x => Txt_Robot2Win.text = $"{x}",
                        mRocketActivity.Robot2StreakWin, 1f);
                    mTweenList.Add(_robot2UI);

                    if (_robotWin)
                        Txt_Prompt.text = $"You failed!";
                }
            }
            else
            {
                mRocketActivity.Fail();
                Tween _playerDown = mPlayerCat.DOLocalMoveY(0, 2f).SetEase(Ease.OutBack);
                mTweenList.Add(_playerDown);

                Tween _platerUI = DOTween.To(() => _playerStreakWin, x => Txt_PlayerWin.text = $"{x}", 0, 1f);
                mTweenList.Add(_platerUI);
                Txt_Prompt.text = $"You failed!";
            }

            //UI表现做完，尝试重置活动(必须放在 OnShow最后调用)
            mRocketActivity.TryRestarActivity();
        }

        protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
			BtnClose.onClick.RemoveAllListeners();

            foreach (var tween in mTweenList)
            {
                tween?.Kill();
            }
            mTweenList.Clear();
            mTweenList = null;

            if (mData.IsManagedOpen ?? false)
                StringEventSystem.Global.Send(GameDefine.GameConst.MANAGER_OPEN_NEXT_PANEL);
        }

        public IArchitecture GetArchitecture()
        {
			return GameMainArc.Interface;
        }

        private void InitAvatarImg()
		{
            //玩家头像初始化
            mPlayerFrame.sprite = AvatarManager.Instance.GetAvatarSprite(false);
            mPlayerAvatar.sprite = AvatarManager.Instance.GetAvatarSprite(true);
            //机器人头像初始化
            for (int i = 0; i < mRobotsFrame.Length; i++) 
				mRobotsFrame[i].sprite = AvatarManager.Instance.GetAvatarSprite(false, i);

			for (int i = 0; i < mRobotsAvatar.Length; i++)
                mRobotsAvatar[i].sprite = AvatarManager.Instance.GetAvatarSprite(true, i);
        }
    }
}
