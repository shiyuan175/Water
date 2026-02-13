using Game.Water;
using QFramework;

namespace Game.Water
{
    public class UISettingData : UIPanelData
    {
    }

    public partial class UISetting : UIPanel, ICanGetModel
    {
        private GameGlobalModel gameGlobalModel;
        private bool volumeSetting;

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UISettingData ?? new UISettingData();
            // please add init code here
        }

        protected override void OnOpen(IUIData uiData = null)
        {
        }

        protected override void OnClose()
        {
        }

        protected override void OnShow()
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
            BtnClose.onClick.AddListener(() =>
            {
                CloseSelf();
            });
        }

        private void SetAudio()
        {
            //AudioKit.Settings.MusicVolume.Value = volumeSetting ? 1 : 0;
            AudioKit.Settings.SoundVolume.Value = volumeSetting ? 1 : 0;
        }
    }
}