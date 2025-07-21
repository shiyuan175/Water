using UnityEngine;
using UnityEngine.UI;
using QFramework;
using System;
using TMPro;

namespace QFramework.Example
{
	public class UIChooseAvatarData : UIPanelData
	{
	}
	public partial class UIChooseAvatar : UIPanel, ICanSendEvent
	{
        //改用动态加载的方式(根据AvatarManager长度加载,并修改精灵)
        [SerializeField] private Toggle[] mToggleAvatars;
		[SerializeField] private Toggle[] mToggleAvatarFrames;

		private int avatarSpriteID;
		private int avatarFrameSpriteID;

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIChooseAvatarData ?? new UIChooseAvatarData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
            TxtSave.font = LevelManager.Instance.greenFont;
            TxtTitle_Blue.font = LevelManager.Instance.blueFont;
        }
		
		protected override void OnShow()
		{
            for (int i = 0; i < mToggleAvatars.Length; i++)
            {
				var _index = i;
                //避免打包顺序错误
                mToggleAvatars[_index].GetComponent<Image>().sprite = AvatarManager.Instance.GetAvatarSprite(true, _index);
                mToggleAvatars[_index].onValueChanged.AddListener(ison =>
                {
                    if (ison)
                        avatarSpriteID = Array.IndexOf(mToggleAvatars, mToggleAvatars[_index]);
                });
            }

            for (int i = 0; i < mToggleAvatarFrames.Length; i++)
            {
                var _index = i;
                mToggleAvatarFrames[_index].GetComponent<Image>().sprite = AvatarManager.Instance.GetAvatarSprite(false, _index);
                mToggleAvatarFrames[_index].onValueChanged.AddListener(ison =>
                {
                    if (ison)
                        avatarFrameSpriteID = Array.IndexOf(mToggleAvatarFrames, mToggleAvatarFrames[_index]);
                });
            }

			BtnClose.onClick.AddListener(() =>
			{
				CloseSelf();
			});

			BtnSave.onClick.AddListener(() =>
			{
				//发送变更头像，头像框事件
				this.SendEvent<AvatarEvent>(new AvatarEvent
				{
					AvatarId = avatarSpriteID,
					AvatarFrameId = avatarFrameSpriteID
				});
				CloseSelf();
            });

            avatarSpriteID = AvatarManager.Instance.GetAvatarId(true);
            avatarFrameSpriteID = AvatarManager.Instance.GetAvatarId(false);

			mToggleAvatars[avatarSpriteID].isOn = true;
			mToggleAvatarFrames[avatarFrameSpriteID].isOn = true;
        }

        protected override void OnHide()
		{

		}
		
		protected override void OnClose()
		{
            mToggleAvatars.ForEach(toggle => toggle.onValueChanged.RemoveAllListeners());
            mToggleAvatarFrames.ForEach(toggle => toggle.onValueChanged.RemoveAllListeners());
            BtnClose.onClick.RemoveAllListeners();
			BtnSave.onClick.RemoveAllListeners();
        }

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }
    }
}
