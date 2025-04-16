using UnityEngine;
using UnityEngine.UI;
using QFramework;
using System;
using System.Collections;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;

namespace QFramework.Example
{
    public class UIBeginSelectData : UIPanelData
    {
    }
    public partial class UIBeginSelect : UIPanel, ICanGetUtility, ICanSendEvent
    {
        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UIBeginSelectData ?? new UIBeginSelectData();
            // please add init code here
        }

        protected override void OnOpen(IUIData uiData = null)
        {
        }

        protected override void OnShow()
        {
            UpdateItemNum();

            BtnClose.onClick.RemoveAllListeners();
            BtnClose.onClick.AddListener(() =>
            {
                StringEventSystem.Global.Send("ClearTakeItem");
                CloseSelf();
            });

            BtnItem1.onClick.RemoveAllListeners();
            BtnItem1.onClick.AddListener(() =>
            {
                if (this.GetUtility<SaveDataUtility>().GetItemNum(6) > 0)
                {
                    var show = !ImgSelect1.gameObject.activeSelf;
                    ImgSelect1.gameObject.SetActive(show);
                    if (show)
                        AddItemIfNotExists(6);
                    else
                        RemoveItemIfExists(6);
                }
                else
                {
                    ImgSelect1.gameObject.SetActive(false);
                }

            });

            BtnItem2.onClick.RemoveAllListeners();
            BtnItem2.onClick.AddListener(() =>
            {
                if (this.GetUtility<SaveDataUtility>().GetItemNum(7) > 0)
                {
                    var show = !ImgSelect2.gameObject.activeSelf;
                    ImgSelect2.gameObject.SetActive(show);

                    if (show)
                        AddItemIfNotExists(7);
                    else
                        RemoveItemIfExists(7);
                }
                else
                {
                    ImgSelect2.gameObject.SetActive(false);
                }
            });

            BtnItem3.onClick.RemoveAllListeners();
            BtnItem3.onClick.AddListener(() =>
            {
                if (this.GetUtility<SaveDataUtility>().GetItemNum(8) > 0)
                {
                    var show = !ImgSelect3.gameObject.activeSelf;
                    ImgSelect3.gameObject.SetActive(show);

                    if (show)
                        AddItemIfNotExists(8);
                    else
                        RemoveItemIfExists(8);
                }
                else
                {
                    ImgSelect3.gameObject.SetActive(false);
                }
            });

            BtnStart.onClick.RemoveAllListeners();
            BtnStart.onClick.AddListener(() =>
            {
                this.SendEvent<GameStartEvent>();
                CloseSelf();
            });

            //�鿴����
            BtnInfo.onClick.RemoveAllListeners();
            BtnInfo.onClick.AddListener(() =>
            {
                ImgReward.gameObject.SetActive(!ImgReward.gameObject.activeSelf);
            });

            BtnBox.onClick.RemoveAllListeners();
            BtnBox.onClick.AddListener(() =>
            {
                //�ж��Ƿ��ܴ򿪽�����3ʤ�����ϣ���
                if (this.GetUtility<SaveDataUtility>().GetCountinueWinNum() >= 3)
                {
                    RewardNode.Show();
                    ImgItem1.Show();
                    ImgItem2.Show();
                    ImgItem3.Show();
                }
            });

            CloseReward.onClick.RemoveAllListeners();
            CloseReward.onClick.AddListener(() =>
            {
                //�򿪽�������������������������
                UIKit.OpenPanel<UIMask>(UILevel.PopUI);
                FlyReward();
            });

            ImgReward.Hide();
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
        }

        /// <summary>
        /// Я������
        /// </summary>
        /// <param name="itemId"></param>
        void AddItemIfNotExists(int itemId)
        {
            //�����ظ�����
            if (!LevelManager.Instance.takeItem.Contains(itemId))
                LevelManager.Instance.takeItem.Add(itemId);
        }

        /// <summary>
        /// �Ƴ�Я���ĵ���
        /// </summary>
        /// <param name="itemId"></param>
        void RemoveItemIfExists(int itemId)
        {
            //����ûѡ����Я��
            if (LevelManager.Instance.takeItem.Contains(itemId))
                LevelManager.Instance.takeItem.Remove(itemId);
        }

        void FlyReward()
        {
            RewardNode.Hide();

            //��Ҫȷ������ʱ��һ��
            var seq = DOTween.Sequence();

            seq.Join(ImgItem1.transform.DOMove(TargetPos.transform.position, 1f).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    ImgItem1.gameObject.SetActive(false);
                    ImgItem1.transform.position = BeginPos1.transform.position;
                }));

            seq.Join(ImgItem2.transform.DOMove(TargetPos.transform.position, 1f).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    ImgItem2.gameObject.SetActive(false);
                    ImgItem2.transform.position = BeginPos2.transform.position;
                }));

            seq.Join(ImgItem3.transform.DOMove(TargetPos.transform.position, 1f).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    ImgItem3.gameObject.SetActive(false);
                    ImgItem3.transform.position = BeginPos3.transform.position;
                }));

            //�ص�
            seq.AppendCallback(() =>
            {
                //����������
                this.GetUtility<SaveDataUtility>().SetCountinueWinNum(0);

                //������������
                var saveU = this.GetUtility<SaveDataUtility>();
                saveU.AddItemNum(6, 1);
                saveU.AddItemNum(7, 1);
                saveU.AddItemNum(8, 1);
                UpdateItemNum();

                //�ر�����
                UIKit.ClosePanel<UIMask>();
                //Debug.Log("���ж�������ˣ�");
            });
        }

        void UpdateItemNum()
        {
            var saveU = this.GetUtility<SaveDataUtility>();
            TxtProgress.text = saveU.GetCountinueWinNum() + " / 3";
            TxtItem1.text = saveU.GetItemNum(6) + "";
            TxtItem2.text = saveU.GetItemNum(7) + "";
            TxtItem3.text = saveU.GetItemNum(8) + "";
            ImgProgress.fillAmount = saveU.GetCountinueWinNum() * 1f / 3;
        }
    }
}
