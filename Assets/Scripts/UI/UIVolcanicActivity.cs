using UnityEngine;
using QFramework;
using System;
using DG.Tweening;

namespace QFramework.Example
{
    public class UIVolcanicActivityData : UIPanelData
    {
        //可空bool
        //由首页打开，空bool，不播动画,同时可以表示不开启下一关面板(多活动时会使用)
        //闯关成功/退出打开，传入非空值播放动画
        public bool? isSuceed;
    }

    public partial class UIVolcanicActivity : UIPanel
    {
        private const int VA_MAX_STREAK_WIN_NUM = 7;
        private const int VA_MAX_PLAYER_NUM = 100;
        //七个台阶起始点位(HeadNodesPar)
        private readonly Vector2[] mSetpPos = new[]
        {
            new Vector2(428, -825),
            new Vector2(50, -818),
            new Vector2(-180, -731),
            new Vector2(-318, -573),
            new Vector2(0, -561),
            new Vector2(224, -456),
            new Vector2(224, -308),
        };


        [SerializeField] private GameObject[] HeadNodes;
        //无实际用(只是需作为参数传入)
        [SerializeField] private GiftPackSO PackSO;

        private VolcanicActivity mVolcanicActivity;
        private Tween mLevelTween;
        private Tween mPlayerTween;

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
                    CoinManager.Instance.AddCoin(mVolcanicActivity.RewardCoins);
                    _action = () =>
                    {
                        StartCoroutine(RewardItemManager.Instance.PlayRewardAnim(PackSO, true));
                    };
                }
            }
            else
                mVolcanicActivity.Fail();

            //UI更新
            mLevelTween = DOTween.To(() => _recordStreakWinNum,
                 x => Txt_Levels.text = $"{(int)x}/{VA_MAX_STREAK_WIN_NUM}",
                mVolcanicActivity.VAStreakWinNum,1f);

            mPlayerTween = DOTween.To(() => _recordPlayerNum,
                x => Txt_Players.text = $"{(int)x}/{VA_MAX_PLAYER_NUM}",
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

            //有新活动在这去开启面板,然后传入mData.isSuceed(最后一个优先级的面板去调用UIGetCoin)
            if (mData.isSuceed != null)
                UIKit.OpenPanel<UIGetCoin>();

        }
    }
}
