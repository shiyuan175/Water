using UnityEngine;
using QFramework;
using UnityEngine.UI;
using Spine;
using JsonFileData;
using System;
using System.Linq;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.Example
{
    public partial class BattlePassPanel : ViewController, ICanGetModel
    {
        private BattlePassModel battlePassModel;
        private BattlePassADActivity battlePassADActivity;
        private GooglePayManager googlePay;
        private RewardGrantUtility rewardGrantUtility;
        private BattlePassADActivity mBattlePassADActivity;

        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

/*        private void Awake()
        {
            mBattlePassADActivity = GameActivityManager.Instance.GetActivity<BattlePassADActivity>();
            battlePassModel = this.GetModel<BattlePassModel>();
            InitUI();
        }*/


        private void OnEnable()
        {

        }

        private void OnDisable()
        {

        }

        private void Start()
        {

        }

        #region 初始化的时候调用
        public void InitUI()
        {
            InitButtomPanelUI();
            InitTopPanelUI();
        }
        

        public void InitTopPanelUI()
        {
            ImgBar.fillAmount = battlePassModel.GameWinNum;
            TextTaskProgressBar.text = $"{battlePassModel.GameWinNum}/{battlePassModel.CurrentGetConditions}";
        }
       
        public void InitButtomPanelUI()
        {
            GameObject _prefab = BattlePassContent.transform.GetChild(0).gameObject;

            // 战令预制体增删
            for (int i = BattlePassContent.transform.childCount; i < battlePassModel.BPDate.Rewards.Length; i++)
                Instantiate(_prefab, BattlePassContent.transform);
            for (int i = battlePassModel.BPDate.Rewards.Length; i < BattlePassContent.transform.childCount; i++)
                BattlePassContent.transform.GetChild(i).gameObject.Hide();

            // 战令内容的设置
            for(int i =0;i < BattlePassContent.transform.childCount;i++)
            {
                BattlePassContent.transform.GetChild(i).GetComponent<BattlePassContentPanel>().Initialize(i);
            }
        }
        #endregion
        #region 每次进入面板时调用
        public void UpTopPanelUI()
        {

        }
        
        public void UpButtomPanelUI()
        {

        }

        #endregion 
        public void SetBtnClick()
        {

            /// 内购开启战令
            BtnActivate.onClick.AddListener(() =>
            {
                //battlePassModel.HightBattlePassActivation();
            });
        }

        public void CreateBattlePassContentPanel()
        {

        }


    }
}
