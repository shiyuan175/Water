using UnityEngine;
using UnityEngine.UI;
using QFramework;
using TMPro;

namespace QFramework.Example
{
    public class UIGuideLevelStepBackData : UIGuideLevelData
    {
    }
    public partial class UIGuideLevelStepBack : UIGuideLevel
    {
        [SerializeField]
        GameObject setpGetItem;
        private readonly Vector3 mStep1HandlePos = new(88, -370, 0);
        private readonly Vector3 mStep2HandlePos = new(-90, -370, 0);
        private readonly Vector3 mStepGetItemPos = new(-445, -815, 0);

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UIGuideLevelStepBackData ?? new UIGuideLevelStepBackData();
            // please add init code here
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            TxtGuide.font = LevelManager.Instance.blueFont;
        }

        protected override void OnShow()
        {
            SetLocalPosition(SpineHandle.transform, mStep2HandlePos);
            SpineHandle.AnimationState.SetAnimation(0, "animation", true);
            BtnBottle1.onClick.AddListener(() =>
            {
                BtnBottle1.Hide();
                //触发中间瓶子点击
                LevelManager.Instance.nowBottles[6].bottle.onClick.Invoke();
                Step2.Show();
                /*SpineHandle.transform.localPosition = mStep2HandlePos;*/
                SetLocalPosition(SpineHandle.transform, mStep1HandlePos);
            });

            BtnBottle2.onClick.AddListener(() =>
            {
                //触发左边瓶子点击
                LevelManager.Instance.nowBottles[7].bottle.onClick.Invoke();
                SetLocalPosition(SpineHandle.transform, mStep2HandlePos);
                SpineHandle.Hide();
                Step2.Hide();
                //等待倒完水
                ActionKit.Delay(2.2f, () =>
                {
                    SpineHandle.Show();
                    SetpGetItem(); ;
                }).Start(this);
            });

            BtnItem.onClick.AddListener(() =>
            {
                LevelManager.Instance.ReturnLast();
                CloseSelf();
            });
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            BtnBottle1.onClick.RemoveAllListeners();
            BtnBottle2.onClick.RemoveAllListeners();
            BtnGet.onClick.RemoveAllListeners();
            BtnItem.onClick.RemoveAllListeners();
        }

        protected void SetpGetItem()
        {
            StepGetItem.Show();
            SpineHandle.gameObject.SetActive(false);
            BtnGet.onClick.AddListener(() =>
            {
                this.SendEvent<UnLockItem>();
                StepGetItem.Hide();
                SpineHandle.gameObject.SetActive(true);
                SetLocalPosition(SpineHandle.transform, mStepGetItemPos);
                StepItem.Show();
            });
        }
    }
}
