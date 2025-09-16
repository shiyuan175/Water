using UnityEngine;
using UnityEngine.UI;
using QFramework;
using TMPro;

namespace QFramework.Example
{
	public class UIGuideLevel2Data : UIGuideLevelData
    {
	}
	public partial class UIGuideLevel2 : UIGuideLevel
    {
        private readonly Vector3 mStep2HandlePos = new(-190, -50, 0);
		private readonly Vector3 mStep3HandlePos = new (190, -50, 0);

        private bool mIsStep2;

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIGuideLevel2Data ?? new UIGuideLevel2Data();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
            TxtGuide.font = LevelManager.Instance.blueFont;
        }
		
		protected override void OnShow()
		{
			mIsStep2 = true;
            SpineHandle.AnimationState.SetAnimation(0, "animation", true);

            BtnBottle1.onClick.AddListener(() =>
            {
                BtnBottle1.Hide();
                //触发中间瓶子点击
                LevelManager.Instance.nowBottles[1].bottle.onClick.Invoke();

                if (mIsStep2)
				{
                    Step2.Show();
                    /*SpineHandle.transform.localPosition = mStep2HandlePos;*/
                    SetLocalPosition(SpineHandle.transform, mStep2HandlePos);
                }
				else
				{
					Step3.Show();
                    /*  SpineHandle.transform.localPosition = mStep3HandlePos;*/
                    SetLocalPosition(SpineHandle.transform, mStep3HandlePos);
                }
                
            });

            BtnBottle2.onClick.AddListener(() =>
            {
                //触发左边瓶子点击
                LevelManager.Instance.nowBottles[0].bottle.onClick.Invoke();
                SpineHandle.transform.localPosition = Vector3.zero;
                SpineHandle.Hide();
				Step2.Hide();
				mIsStep2 = false;

                //等待倒完水
                ActionKit.Delay(2.2f, () =>
				{
                    SpineHandle.Show();
                    BtnBottle1.Show();
                }).Start(this);
            });

			BtnBottle3.onClick.AddListener(() =>
			{
                //触发右边瓶子点击
                LevelManager.Instance.nowBottles[2].bottle.onClick.Invoke();
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
        }
	}
}
