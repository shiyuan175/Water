using UnityEngine;
using UnityEngine.UI;
using QFramework;
using Unity.Collections;
using TMPro;

namespace QFramework.Example
{
	public class UIPersonalData : UIPanelData
	{
	}
	public partial class UIPersonal : UIPanel, ICanRegisterEvent
	{
        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIPersonalData ?? new UIPersonalData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
            TxtName1.font = LevelManager.Instance.redFont;
			TxtName2.font = LevelManager.Instance.redFont;
            TxtTitle1_Blue.font = LevelManager.Instance.blueFont;
            TxtTitle2_Blue.font = LevelManager.Instance.blueFont;

            //注册修改事件
            this.RegisterEvent<AvatarEvent>(e =>
			{
                BtnHead.GetComponent<Image>().sprite = AvatarManager.Instance.GetAvatarSprite(true, e.AvatarId);
                ImgHeadFrame.sprite = AvatarManager.Instance.GetAvatarSprite(false, e.AvatarFrameId);
            }).UnRegisterWhenGameObjectDestroyed(this);

            //读取初始头像和头像框
            BtnHead.GetComponent<Image>().sprite = AvatarManager.Instance.GetAvatarSprite(true);
			ImgHeadFrame.sprite = AvatarManager.Instance.GetAvatarSprite(false);

            BtnHead.onClick.AddListener(() =>
			{
				UIKit.OpenPanel<UIChooseAvatar>();
			});

			BtnClose.onClick.AddListener(() =>
            {
                UIKit.ClosePanel(this);
            });
        }

        protected override void OnShow()
		{
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
            BtnClose.onClick.RemoveAllListeners();
            BtnHead.onClick.RemoveAllListeners();
        }

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }
    }
}
