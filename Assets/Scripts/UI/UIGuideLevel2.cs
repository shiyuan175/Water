using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class UIGuideLevel2Data : UIPanelData
	{
	}
	public partial class UIGuideLevel2 : UIPanel
	{
		private readonly Vector3 mMidHandlePos = new(25, -50, 0);
        private readonly Vector3 mStep2HandlePos = new(-160, -50, 0);
		private readonly Vector3 mStep3HandlePos = new (215, -50, 0);

        private bool mIsStep2;

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIGuideLevel2Data ?? new UIGuideLevel2Data();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
			mIsStep2 = true;

            BtnBottle1.onClick.AddListener(() =>
            {
                BtnBottle1.Hide();
                //触发中间瓶子点击
                LevelManager.Instance.nowBottles[1].bottle.onClick.Invoke();

                if (mIsStep2)
				{
                    Step2.Show();
                    AnimHandle.transform.localPosition = mStep2HandlePos;
                }
				else
				{
					Step3.Show();
                    AnimHandle.transform.localPosition = mStep3HandlePos;
                }
                
            });

            BtnBottle2.onClick.AddListener(() =>
            {
                //触发左边瓶子点击
                LevelManager.Instance.nowBottles[0].bottle.onClick.Invoke();
                AnimHandle.transform.localPosition = mMidHandlePos;
                AnimHandle.Hide();
				Step2.Hide();
				mIsStep2 = false;

				//等待倒完水
				ActionKit.Delay(2.2f, () =>
				{
					AnimHandle.Show();
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
