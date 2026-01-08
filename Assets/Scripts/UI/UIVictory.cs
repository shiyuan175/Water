using System;
using System.Linq;
using GameDefine;
using UnityEngine;

namespace QFramework.Example
{
    public class UIVictoryData : UIPanelData
    {
    }

    public partial class UIVictory : UIPanel, ICanSendEvent, ICanGetUtility, ICanGetModel
    {
        private int mLastRankingScore;
        private SaveDataUtility saveDataUtility;
        [SerializeField] private Sprite[] unlockSprites;
        private readonly int[] UNLOCKLEVEL = new int[]
            { 11, 21, 31, 41, 51, 61, 71, 81, 91, 101, 121, 141, 161, 181, 201, 301, 401 };

        public Material TMPFont_red;
        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UIVictoryData ?? new UIVictoryData();
            // please add init code here
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            saveDataUtility = this.GetUtility<SaveDataUtility>();
        }

        protected override void OnShow()
        {
            BtnSkip.onClick.AddListener(() =>
            {
                UnlockNewItem();
            });
            WaitClose();
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            BtnSkip.onClick.RemoveAllListeners();
        }

        private void WaitClose()
        {
            ActionKit.Delay(3f, () =>
            {
                UnlockNewItem();
            }).Start(this);
        }

        private void UnlockNewItem()
        {
            int curLevel = saveDataUtility.GetCurrentLevel();
            if (UNLOCKLEVEL.Contains(curLevel))
            {
                int _idx = Array.IndexOf(UNLOCKLEVEL, curLevel);
                NewItemNode.Show();
                TxtNewItem.fontSharedMaterial = TMPFont_red;
                TxtNewItem.Hide();
                TxtNewItem.Show();
                TxtNewItem.text = GameConst.GameplayTutorialInfo[curLevel].guideInfo;
                ImgNewItem.sprite = unlockSprites[_idx];
                ImgNewItem.SetNativeSize();

                BtnNewItemClose.onClick.AddListener(() =>
                {
                    NewItemNode.Hide();
                    this.SendEvent<GameStartEvent>();
                    LevelManager.Instance.StartGame(this.GetUtility<SaveDataUtility>().GetCurrentLevel());
                    CloseSelf();
                });
            }
            else
            {
                this.SendEvent<GameStartEvent>();
                LevelManager.Instance.StartGame(this.GetUtility<SaveDataUtility>().GetCurrentLevel());
                CloseSelf();
            }
                
        }
    }


}
