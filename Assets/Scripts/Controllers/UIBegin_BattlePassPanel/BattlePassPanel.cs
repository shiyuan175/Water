using UnityEngine;
using QFramework;
using UnityEngine.UI;
using Spine;
using JsonFileData;
using System;
using System.Linq;
using TMPro;
using DG.Tweening;
using Sequence = DG.Tweening.Sequence;

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
        private Tween mCountDownTween;
        private Sequence topImageFillSequence;
        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

        //private void Awake()
        //{
        //    mBattlePassADActivity = GameActivityManager.Instance.GetActivity<BattlePassADActivity>();
        //    battlePassModel = this.GetModel<BattlePassModel>();
        //    InitUI();
        //}
       

        private void OnEnable()
        { 
            topImageFillSequence = DOTween.Sequence();
            UpdateUI();     
        }

        private void OnDisable()
        {
            topImageFillSequence?.Kill();
            ImgBar.fillAmount = (float)battlePassModel.GameWinNum / battlePassModel.CurrentGetConditions;
            ImgLevel.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = battlePassModel.RewardLevel.ToString();
            TextTaskProgressBar.text = $"{battlePassModel.GameWinNum}/{battlePassModel.CurrentGetConditions}";
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
            ImgBar.fillAmount = (float)battlePassModel.GameWinNum/ battlePassModel.CurrentGetConditions;
            ImgLevel.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = battlePassModel.RewardLevel.ToString();
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
            for(int i =0;i < battlePassModel.BPDate.Rewards.Length; i++)
            {
                int _level = i;
                BattlePassContent.transform.GetChild(_level).GetComponent<BattlePassContentPanel>().Initialize(_level);             
            }
        }
        #endregion
        #region 每次进入面板时调用
        public void UpdateUI()
        {
            int oldLevel = int.Parse(ImgLevel.transform.Find("Text").GetComponent<TextMeshProUGUI>().text);
            #region 更新顶部内容
            float oldFillAmout = ImgBar.fillAmount;

            mCountDownTween = DOTween.To(() => 0, x =>
            {
                if (mBattlePassADActivity.ActivityStatus == GameActivityStatus.Active)
                    TxtCountDown.text = mBattlePassADActivity.GetActivityReamingTime();
                else
                    TxtCountDown.text = "Finished";
            }, 1, 1f)
           .SetLoops(-1, LoopType.Restart)
           .SetUpdate(true);

            #region 顶部进度条动画

            for (int level = oldLevel + 1; level <= battlePassModel.RewardLevel; level++)
            {
                int _level = level;
                Tween fillTween = ImgBar.DOFillAmount(1, 0.5f)
                    .SetEase(Ease.Linear)
                    .Pause()
                    .OnStepComplete(() => {
                        ImgBar.fillAmount = 0;
                        ImgLevel.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = _level.ToString();
                        TextTaskProgressBar.text = $"{battlePassModel.BPDate.Rewards[_level].GetConditions}/{battlePassModel.BPDate.Rewards[_level].GetConditions}";
                    });

                topImageFillSequence.Append(fillTween);
            }

            topImageFillSequence.AppendCallback(() =>
            {

                ImgLevel.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = (battlePassModel.RewardLevel).ToString();
                TextTaskProgressBar.text = $"{battlePassModel.GameWinNum}/{battlePassModel.CurrentGetConditions}";
            });
            Tween endFillTween = ImgBar.DOFillAmount((float)battlePassModel.GameWinNum / battlePassModel.CurrentGetConditions, 0.5f)
                   .SetEase(Ease.Linear)
                   .Pause();
            topImageFillSequence.Append(endFillTween);
            topImageFillSequence.Play();
            #endregion

            #endregion

            #region 更新底部内容
            for (int i = oldLevel; i < battlePassModel.BPDate.Rewards.Length; i++)
            {
                int _level = i;
                BattlePassContent.transform.GetChild(_level).GetComponent<BattlePassContentPanel>().UpdateUI(_level);
            }
            #endregion
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
