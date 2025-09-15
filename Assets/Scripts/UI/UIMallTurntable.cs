using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GameDefine;
using System;
using DG.Tweening;
using TMPro;

namespace QFramework.Example
{
    public class UIMallTurntableData : UIPanelData
    {
        public bool? IsManagedOpen;
    }
    public partial class UIMallTurntable : UIPanel, ICanGetUtility
    {
		[SerializeField]
		float MinRingCount = 1f;
		[SerializeField]
		List<GameObject> mTurnTablePack;
		[SerializeField]
        public Ease easeType;

		List<List<GameObject>> packs;
        GameObject targetGameObject;
        TurnTableTimesProbability[] allValues;
		

		private TurnTableADActivity mTurnTableADActivity;
        public IArchitecture GetArchitecture()
        {
            return GameMainArc.Interface;
        }
        protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIMallTurntableData ?? new UIMallTurntableData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
            mTurnTableADActivity = GameActivityManager.Instance.GetActivity<TurnTableADActivity>();
	
			
            //读取enum的值
            allValues = (TurnTableTimesProbability[])Enum.GetValues(typeof(TurnTableTimesProbability));

			//按照枚举的值排序mTurnTablePack  概率越低在越前面
			mTurnTablePack.Sort((a, b) => ((int)a.GetComponent<TurnTablePack>().turnTablePack.AwardLevel).CompareTo((int)b.GetComponent<TurnTablePack>().turnTablePack.AwardLevel));
			foreach(var pack in mTurnTablePack)
			{
				pack.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = pack.GetComponent<TurnTablePack>().turnTablePack.ItemCount();
            }

            foreach (var pack in mTurnTablePack)
            {
                int giftProbability = (int)pack.GetComponent<TurnTablePack>().turnTablePack.AwardLevel;
            }
        }

        protected override void OnShow()
		{
			BindBtn();
			RefreshUI();
        }
		
		protected override void OnHide()
		{         
        }
		
		protected override void OnClose()
		{
            if (mData.IsManagedOpen ?? false)
                StringEventSystem.Global.Send(GameDefine.GameConst.MANAGER_OPEN_NEXT_PANEL);
        }

		private void BindBtn()
		{
            BtnBeginTurnTable.onClick.RemoveAllListeners();
			BtnBeginTurnTable.onClick.AddListener(() =>
			{
				Debug.Log(mTurnTableADActivity.CurrentTurnTableCount);
				//不是第一次，播放广告
				if (mTurnTableADActivity.CurrentTurnTableCount> 0)
				{
					TopOnADManager.Instance.ShowVideoAd(() => {
						
                        BtnBeginTurnTable.interactable = false;                  
                        TurnAnimation();
						mTurnTableADActivity.ADPlaybackCompleted(targetGameObject);
						
						
                    }, () => { });
#if UNITY_EDITOR
					Debug.Log("模拟广告");
                    BtnBeginTurnTable.interactable = false;                 
                    TurnAnimation();
                    mTurnTableADActivity.ADPlaybackCompleted(targetGameObject);
					
#endif
                }
				else
				{
					BtnBeginTurnTable.interactable = false;									
					TurnAnimation();
                    mTurnTableADActivity.ADPlaybackCompleted(targetGameObject);
				
                }	
            });

            BtnExit.onClick.RemoveAllListeners();
            BtnExit.onClick.AddListener(() =>
			{
				CloseSelf();
			});
        
            BtnTurnTableRule.onClick.RemoveAllListeners();
            BtnTurnTableRule.onClick.AddListener(() =>
			{
				TextRuleBk.Show();
			});
		
		}
		private void RefreshUI()
		{
            // 到达游玩次数限制点击
            if (mTurnTableADActivity.CurrentTurnTableCount >= GameConst.TURNTABLE_LIMIT_GAMETIMES_PER_CYCLE)
            {
                BtnBeginTurnTable.interactable = false;
            }
            TextPlayTime.text = $"Number of playable times:<br>{mTurnTableADActivity.CurrentTurnTableCount}/{GameDefine.GameConst.TURNTABLE_LIMIT_GAMETIMES_PER_CYCLE}";
			if (mTurnTableADActivity.CurrentTurnTableCount > 0)
                TextTipBk.Show();

        }
		/// <summary>
		/// 进行动画
		/// </summary>
		private void TurnAnimation()
		{
			// 计算目标
            targetGameObject = CalculateProbability();
			Transform targetTransform = targetGameObject.transform;
            // 计算目标选项与上方世界的夹角
            float angle = Vector3.Angle(transform.up, targetTransform.up);
            // 确定目标在左方还是右方（叉积判断）
            Vector3 dir = Vector3.Cross(targetTransform.up, transform.up);
            // 如果在左方，需要计算优弧角度（360-角度）
            angle = dir.z < 0 ? 360 - angle : angle;
			// 偏移角度 -20 ~ 20
			float OffsetAngle = UnityEngine.Random.Range(-20f, 20f);

			ImgTurn.transform.DORotate(new Vector3(0, 0, MinRingCount * 360 + angle + OffsetAngle),
				4 , RotateMode.FastBeyond360).SetEase(easeType).SetRelative().OnComplete(() =>
				{
					BtnBeginTurnTable.interactable = true;
					RefreshUI();
				});

        }

		/// <summary>
		/// 计算转盘的结果，计算概率公式 概率 = 次数概率+礼品概率
		/// </summary>
		private GameObject CalculateProbability()
		{
			int probabilityNumber = UnityEngine.Random.Range(0, GameConst.TURNTABLE_PROBABIlITY );
		
		
			// 用来做同概率礼物的随机取值
			GameObject[] _SameTypeGift = new GameObject[mTurnTablePack.Count];
			int _index = 0;
			for(int i =0;i<mTurnTablePack.Count; i++)
			{
				/*int timeProbability = (int)allValues[mTurnTableADActivity.CurrentTurnTableCount];*/
				int giftProbability = (int)mTurnTablePack[i].GetComponent<TurnTablePack>().turnTablePack.AwardLevel;
                if (probabilityNumber <  giftProbability)
				{
					// 同一个概率的礼物有多个，需要随机取其中一个 从0开始是为了把自己也丢进数组
					for(int j=0;j+i<mTurnTablePack.Count;j++)
					{
						int _giftProbability = (int)mTurnTablePack[i + j].GetComponent<TurnTablePack>().turnTablePack.AwardLevel;
						if (_giftProbability != giftProbability)
							break;
						GameObject _gameObject = mTurnTablePack[i + j];
						_SameTypeGift[_index++] = _gameObject;
                    }
					
					
					return _SameTypeGift[UnityEngine.Random.Range(0, _index)];
                }
			}

			// 返回最坏的奖励作为边界
			return mTurnTablePack[mTurnTablePack.Count - 1];
        }



    }
}
