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

        private Sequence topImageFillSequence;
        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

        private void Awake()
        {
            mBattlePassADActivity = GameActivityManager.Instance.GetActivity<BattlePassADActivity>();
            battlePassModel = this.GetModel<BattlePassModel>();
        /*    InitUI();*/
            topImageFillSequence = DOTween.Sequence();
        }


        private void OnEnable()
        {
            UpdateUI();
            topImageFillSequence = DOTween.Sequence();
        }

        private void OnDisable()
        {
            topImageFillSequence?.Kill();
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
            for(int i =0;i < BattlePassContent.transform.childCount;i++)
            {
                int _level = i;
                BattlePassContent.transform.GetChild(_level).GetComponent<BattlePassContentPanel>().Initialize(_level);             
            }
        }
        #endregion
        #region 每次进入面板时调用
        public void UpdateUI()
        {
            UpTopPanelUI();
            UpButtomPanelUI();
        }
        public void UpTopPanelUI()
        {
            int oldLevel = int.Parse(ImgLevel.transform.Find("Text").GetComponent<TextMeshProUGUI>().text);
            float oldFillAmout = ImgBar.fillAmount;
            topImageFillSequence = DOTween.Sequence();
            // 开始到结束
            for (int level = oldLevel; level <= battlePassModel.RewardLevel; level++)
            {
                int _level = level;
                Tween fillTween = ImgBar.DOFillAmount(1, 0.5f)
                    .SetEase(Ease.Linear)
                    .OnStart(() => {
                        Debug.Log(_level);
                    })
                    .Pause()
                    .OnComplete(() => {
                        ImgBar.fillAmount = 0;
                        ImgLevel.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = _level.ToString();
                    });

                topImageFillSequence.Append(fillTween);
            }
            Debug.Log($"序列中的动画数量: {topImageFillSequence.Duration()}");
            Debug.Log($"序列总时长: {topImageFillSequence.Duration()}");
            topImageFillSequence.Play();
            Debug.Log($"序列中的动画数量: {topImageFillSequence.Duration()}");
            Debug.Log($"序列总时长: {topImageFillSequence.Duration()}");
            // 3. 播放序列
        
           
            /* ImgBar.fillAmount = (float)battlePassModel.GameWinNum / battlePassModel.CurrentGetConditions;
             ImgLevel.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = battlePassModel.RewardLevel.ToString();
             TextTaskProgressBar.text = $"{battlePassModel.GameWinNum}/{battlePassModel.CurrentGetConditions}";
 */

        }
        
        public void UpButtomPanelUI()
        {
            for (int i = 0; i < BattlePassContent.transform.childCount; i++)
            {
                int _level = i;
                BattlePassContent.transform.GetChild(_level).GetComponent<BattlePassContentPanel>().UpdateUI(_level);
            }
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
