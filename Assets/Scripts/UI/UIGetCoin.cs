using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class UIGetCoinData : UIPanelData
	{
	}
	public partial class UIGetCoin : UIPanel, ICanSendEvent, ICanGetUtility, ICanGetModel
    {
        [SerializeField] private GiftPackSO[] rewardPackSO;
        private StageModel stageModel;
        private SaveDataUtility saveDataUtility;
        private int curLevel;
        private int getReward;

        private const int STAR_LEVEL = 6;
        private const int END_LEVEL = 97;
        public const int REWARD_INTERVAL = 7;
        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIGetCoinData ?? new UIGetCoinData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
            stageModel = this.GetModel<StageModel>();
            saveDataUtility = this.GetUtility<SaveDataUtility>();
        }

        protected override void OnShow()
        {
            getReward = -1;
            //���غ���¼��ǰ�ؿ�Ϊ��һ��(��һ��ʾͨ���Ĺؿ�)
            curLevel = saveDataUtility.GetLevelClear() - 1;
            //6-97����ʾ(ͨ��97��֮����ʾ)
            if (curLevel >= STAR_LEVEL && curLevel < END_LEVEL)
            {
                ImgProcessNode.Show();
                int _progress = (curLevel - STAR_LEVEL + 1) % REWARD_INTERVAL;
                if (_progress == 0)
                    getReward = ((curLevel - STAR_LEVEL + 1) / REWARD_INTERVAL) - 1;//��һ��������

                int _displayedProgress = _progress == 0 ? REWARD_INTERVAL : _progress;
                TxtProcess.text = $"{_displayedProgress} / {REWARD_INTERVAL}";
                ImgProcess.fillAmount = (float)_displayedProgress / REWARD_INTERVAL;
            }
            BindClick();
            TxtCoin.text = ((int)(GameDefine.GameConst.WIN_COINS * stageModel.GoldCoinsMultiple)).ToString();
            TxtLevel.text = "Level " + curLevel.ToString();
        }
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
            stageModel = null;
            saveDataUtility = null;
            BtnClose.onClick.RemoveAllListeners();
            BtnContinue.onClick.RemoveAllListeners();
        }

		void BindClick()
		{
            BtnClose.onClick.AddListener(() =>
            {
                BackUIBegin();
            });

            BtnContinue.onClick.AddListener(() =>
            {
                BackUIBegin();
            });
        }

        void BackUIBegin()
        {
            if (getReward >= 0 && getReward < rewardPackSO.Length)
            {
                var _packSO = rewardPackSO[getReward];
                foreach (var item in _packSO.ItemReward)
                {
                    stageModel.AddItem(item.ItemIndex,item.Quantity);
                }
                //ʹ�ö����+DoTween���Ŷ���
                Debug.Log("���Ŷ���");
            }
            //else
            //    Debug.Log($"����Խ�� getReward:{ getReward }");

            this.SendEvent<LevelClearEvent>(new LevelClearEvent());
            CloseSelf();
        }
	}
}
