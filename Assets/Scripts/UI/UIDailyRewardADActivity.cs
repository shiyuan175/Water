using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
    public class UIDailyRewardADActivityData : UIPanelData
    {
    }
    public partial class UIDailyRewardADActivity : UIPanel
    {
        [SerializeField] private GiftPackSO[] dailyRewardADActivityPackSO;
        [SerializeField] private Button[] adBtns;

        private DailyRewardADActivity mDailyRewardADActivity;
        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UIDailyRewardADActivityData ?? new UIDailyRewardADActivityData();

        }

        protected override void OnOpen(IUIData uiData = null)
        {
            mDailyRewardADActivity = GameActivityManager.Instance.GetActivity<DailyRewardADActivity>();
            BindBtn();
        }

        protected override void OnShow()
        {
            RefershUI();
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
        }
        private void BindBtn()
        {
            for (int i = 0; i < dailyRewardADActivityPackSO.Length; i++)
            {
                int _index = i;
                adBtns[_index].onClick.AddListener(() =>
                {
                    TopOnADManager.Instance.ShowVideoAd(() =>
                    {
                        mDailyRewardADActivity.ADPlaybackCompleted(dailyRewardADActivityPackSO[_index]);
                        RefershUI();
                    }, () => { });
#if UNITY_EDITOR
                    Debug.Log("模拟广告");
                    mDailyRewardADActivity.ADPlaybackCompleted(dailyRewardADActivityPackSO[_index]);
                    RefershUI();
#endif

                });

            }


        }
        private void RefershUI()
        {
            for (int i = 0; i < dailyRewardADActivityPackSO.Length; i++)
            {
                // <=
                if (i == mDailyRewardADActivity.CurrentWatchADCount)
                    adBtns[i].interactable = true;
                else
                    adBtns[i].interactable = false;
            }
        }

    }
}
