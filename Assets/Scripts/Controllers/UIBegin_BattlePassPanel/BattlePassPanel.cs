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
using UnityEngine.UIElements;

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
        private Sequence buttomImageFillSequence;
        private int oldLevel = 0;
        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

        private void Awake()
        {
            mBattlePassADActivity = GameActivityManager.Instance.GetActivity<BattlePassADActivity>();
            battlePassModel = this.GetModel<BattlePassModel>();
            // 发布需打开
           /* InitUI();*/
            /*SetBtnClick();*/
        }
       

        private void OnEnable()
        { 
            topImageFillSequence = DOTween.Sequence();
            buttomImageFillSequence = DOTween.Sequence();
            UpdateUI();     
        }

        private void OnDisable()
        {
            topImageFillSequence?.Kill();
            buttomImageFillSequence?.Kill();
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
            oldLevel = int.Parse(ImgLevel.transform.Find("Text").GetComponent<TextMeshProUGUI>().text);
            UpTopPanelUI();
            // 底部面板的更新由顶部动画结束触发
        }
        public void UpTopPanelUI()
        {       
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

        
            for (int level = oldLevel+1; level <= battlePassModel.RewardLevel; level++)
            {
                int _level = level;
                Tween fillTween = ImgBar.DOFillAmount(1, 1f / (battlePassModel.RewardLevel - oldLevel))
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
            Tween endFillTween = ImgBar.DOFillAmount((float)battlePassModel.GameWinNum / battlePassModel.CurrentGetConditions,1f / (battlePassModel.RewardLevel - oldLevel))
                   .SetEase(Ease.Linear)
                   .Pause()
                   .OnComplete(UpButtomPanelUI);
            topImageFillSequence.Append(endFillTween);
            topImageFillSequence.Play();
            #endregion
        }

        public void UpButtomPanelUI()
        {

            // 设置旧的奖励条
            BattlePassContent.transform.GetChild(oldLevel-1).GetComponent<BattlePassContentPanel>().SetDividingLine(0);
            buttomImageFillSequence = DOTween.Sequence();
            // 设置进度条动画和面板内容
            for (int i = oldLevel; i < battlePassModel.RewardLevel; i++)
            {
                int _level = i;
                Tween _tween = BattlePassContent.transform.GetChild(_level).GetComponent<BattlePassContentPanel>().UpdateUI(_level, 1.0f / (battlePassModel.RewardLevel - oldLevel));
                buttomImageFillSequence.Append(_tween);
                if(i == (oldLevel+ battlePassModel.RewardLevel) /2)
                    SmoothScrollController(battlePassModel.RewardLevel);
            }
           
            buttomImageFillSequence.OnComplete(() =>
            {
                // 设置新奖励条
                BattlePassContent.transform.GetChild(battlePassModel.RewardLevel ).GetComponent<BattlePassContentPanel>().SetDividingLine(1);
            });

            buttomImageFillSequence.Play();

            // 战令内容的更新
            for (int i = 0; i < battlePassModel.BPDate.Rewards.Length; i++)
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
                battlePassModel.HightBattlePassActivation();
                // 战令内容的更新
                for (int i = 0; i < battlePassModel.BPDate.Rewards.Length; i++)
                {
                    int _level = i;
                    BattlePassContent.transform.GetChild(_level).GetComponent<BattlePassContentPanel>().UpdateUI(_level);
                }
            });
        }

        public  void SmoothScrollController(int index)
        {
            RectTransform targetElement = ScrollView.content.GetChild(index) as RectTransform;
            Vector2 targetPosition = GetSnapToPositionToBringChildIntoView(targetElement);

            // 使用DOTween平滑滚动
            DOTween.To(
                () => ScrollView.content.anchoredPosition,
                x => ScrollView.content.anchoredPosition = x,
                targetPosition,
                1f
            ).SetEase(Ease.OutCubic);
        }
        private Vector2 GetSnapToPositionToBringChildIntoView(RectTransform child)
        {
            Canvas.ForceUpdateCanvases();

            Vector2 viewportLocalPosition = ScrollView.viewport.localPosition;
            Vector2 childLocalPosition = child.localPosition;

            Vector2 result = new Vector2(
                0,
                -viewportLocalPosition.y - childLocalPosition.y - (child.rect.height / 2)
            );

            return result;
        }
    }
}
