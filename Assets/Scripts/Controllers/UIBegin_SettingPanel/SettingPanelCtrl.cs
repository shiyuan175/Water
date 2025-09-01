using UnityEngine;
using QFramework;

namespace QFramework.Example
{
	public partial class SettingPanelCtrl : ViewController
	{
        private StageModel stageModel;
        private bool volumeSetting;

        void Awake()
        {
        }

        void Start()
		{
            stageModel = this.GetModel<StageModel>();
            volumeSetting = stageModel.VolumeSetting;

            ImgSelected.gameObject.SetActive(volumeSetting);
            SetAudio();

            BtnSelect.onClick.AddListener(() =>
			{
                volumeSetting = !volumeSetting;
                ImgSelected.gameObject.SetActive(volumeSetting);
                SetAudio();

                stageModel.VolumeSetting = volumeSetting;
            });
		}

        void SetAudio()
        {
            //AudioKit.Settings.MusicVolume.Value = volumeSetting ? 1 : 0;
            AudioKit.Settings.SoundVolume.Value = volumeSetting ? 1 : 0;
        }
    }
}
