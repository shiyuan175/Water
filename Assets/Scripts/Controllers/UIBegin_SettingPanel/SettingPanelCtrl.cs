using UnityEngine;
using QFramework;

namespace QFramework.Example
{
	public partial class SettingPanelCtrl : ViewController
	{
        private GameGlobalModel gameGlobalModel;
        private bool volumeSetting;

        void Awake()
        {
        }

        void Start()
		{
            gameGlobalModel = this.GetModel<GameGlobalModel>();
            volumeSetting = gameGlobalModel.VolumeSetting;

            ImgSelected.gameObject.SetActive(volumeSetting);
            SetAudio();

            BtnSelect.onClick.AddListener(() =>
			{
                volumeSetting = !volumeSetting;
                ImgSelected.gameObject.SetActive(volumeSetting);
                SetAudio();

                gameGlobalModel.VolumeSetting = volumeSetting;
            });
		}

        void SetAudio()
        {
            //AudioKit.Settings.MusicVolume.Value = volumeSetting ? 1 : 0;
            AudioKit.Settings.SoundVolume.Value = volumeSetting ? 1 : 0;
        }
    }
}
