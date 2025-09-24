using UnityEngine;
using QFramework;
using System;
using DG.Tweening;

namespace QFramework.Example
{
    public class UIVolcanicActivityData : UIPanelData
    {
        //由首页打开，空bool，不播动画
        //闯关成功/退出打开，传入非空值播放动画
        public bool? isSuceed;
        public bool? IsManagedOpen;
    }

    public partial class UIVolcanicActivity : UIPanel, ICanSendEvent
    {
        private const int VA_MAX_STREAK_WIN_NUM = 7;
        private const int VA_MAX_PLAYER_NUM = 100;
        //八个台阶起始点位(HeadNodesPar)
        private readonly Vector2[] mSetpPos = new[]
        {
            new Vector2(428, -825),
            new Vector2(50, -818),
            new Vector2(-180, -731),
            new Vector2(-318, -573),
            new Vector2(0, -561),
            new Vector2(224, -456),
            new Vector2(224, -308),
            new Vector2(0 , -204)
        };

        [SerializeField] private GameObject[] HeadNodes;

        private VolcanicActivity mVolcanicActivity;
        private Tween mLevelTween;
        private Tween mPlayerTween;
        private Tween mCountDownTween;

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UIVolcanicActivityData ?? new UIVolcanicActivityData();
            // please add init code here
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            TxtLevels.font = LevelManager.Instance.redFont;
            TxtPlayers.font = LevelManager.Instance.redFont;
            Txt_Levels.font = LevelManager.Instance.redFont;
            Txt_Players.font = LevelManager.Instance.redFont;
            mVolcanicActivity = GameActivityManager.Instance.GetActivity<VolcanicActivity>();

            string _naimName = UnityEngine.Random.Range(0, 2) == 0 ? "idle1" : "idle2";
            Spine_rongyanpaopao.AnimationState.SetAnimation(0, _naimName, true);
            
            mCountDownTween = DOTween.To(() => 0, x =>
            {
                if (mVolcanicActivity.ActivityStatus == GameActivityStatus.Active)
                    TxtCountDown.text = mVolcanicActivity.GetActivityReamingTime();
                else
                    TxtCountDown.text = "Finished";
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

            //记录获胜前数据(做文本动画)
            int _recordPlayerNum = mVolcanicActivity.VACurrentPlayerNum;
            int _recordStreakWinNum = mVolcanicActivity.VAStreakWinNum;
            Txt_Prompt.text = $"Beat {VA_MAX_STREAK_WIN_NUM - mVolcanicActivity.VAStreakWinNum} more levels to complete the challenge!";
            Txt_Levels.text = $"{mVolcanicActivity.VAStreakWinNum}/{VA_MAX_STREAK_WIN_NUM}";
            Txt_Players.text = $"{mVolcanicActivity.VACurrentPlayerNum}/{VA_MAX_PLAYER_NUM}";

            int _curStep = _recordStreakWinNum;

            //头像节点实例
            var _headNodes = Instantiate(HeadNodes[UnityEngine.Random.Range(0, HeadNodes.Length)], HeadNodesPar);
            HeadNodesPar.localPosition = mSetpPos[_curStep];

            if (mData.isSuceed == null)
                return;

            var _headNodeCtrl = _headNodes.GetComponent<VA_HeadNodesCtrl>();
            Action _action = null;

            if ((bool)mData.isSuceed)
            {
                mVolcanicActivity.StreakWin();
                //最后一个台阶-发放奖励
                if (mVolcanicActivity.EndWin)
                {
                    UIKit.OpenPanel<UIMask>();
                    CoinManager.Instance.AddCoin(mVolcanicActivity.RewardCoins);
                    _action = () =>
                    {
                        UIKit.ClosePanel<UIMask>();
                        RewardUIManager.Instance.PlayRewardAnim(mVolcanicActivity.RewardCoins, true, null);
                    };
                }

                Txt_Prompt.text = $"Beat {VA_MAX_STREAK_WIN_NUM - mVolcanicActivity.VAStreakWinNum} more levels to complete the challenge!";
            }
            else
            {
                mVolcanicActivity.Fail();
                Txt_Prompt.text = $"You failed!";
            }

            //UI更新
            mLevelTween = DOTween.To(() => _recordStreakWinNum,
                 x => Txt_Levels.text = $"{x}/{VA_MAX_STREAK_WIN_NUM}",
                mVolcanicActivity.VAStreakWinNum,1f);

            mPlayerTween = DOTween.To(() => _recordPlayerNum,
                x => Txt_Players.text = $"{x}/{VA_MAX_PLAYER_NUM}",
                mVolcanicActivity.VACurrentPlayerNum,1f);

            _headNodeCtrl.Jump(_curStep, _action);
        }
        
        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            BtnClose.onClick.RemoveAllListeners();
            mVolcanicActivity = null;
            mLevelTween?.Kill();
            mPlayerTween?.Kill();
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
