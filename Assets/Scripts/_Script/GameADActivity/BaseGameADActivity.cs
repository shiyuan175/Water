using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGameADActivity : IGameActivity, ICanGetModel, ICanGetUtility, ICanSendEvent
{
    public abstract string ActivitySign { get; }
    public abstract string ActivityID { get; }
    // �������ʱ�����ؿ���
    public abstract int ActivityBeginLevel { get; }

    public abstract GameActivityStatus ActivityStatus { get; }

    public virtual string ActivityCooldownSign { get; }
    public virtual float ActivityCooldownMinutes { get; }
    public virtual float ActivityDurationMinutes { get; }

    public SaveDataUtility mSaveUtility;
    public GameGlobalModel MGameGlobalModel;
    public RewardGrantUtility mRewardGrantUtility;

    // ��������ά���״̬
    private GameActivityStatus mLastActivityStatus;
    /// <summary>
    /// ��ȡ�ʣ��ʱ��
    /// </summary>
    /// <returns></returns>

    public BaseGameADActivity()
    {
        mSaveUtility = this.GetUtility<SaveDataUtility>();
        MGameGlobalModel = this.GetModel<GameGlobalModel>();
        mRewardGrantUtility = this.GetUtility<RewardGrantUtility>();
    }
    public abstract void StartActivity();
    public abstract void RestartActivity();
    public virtual void CoolDownActivity()
    {

    }
    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }

    //�ע�����Զ�����
    public virtual void Tick()
    {
        //����ʱĬ�Ϸ���һ��״̬�¼�(δ��������)
        //��CoolDown ���� WaitStart ״̬ʱ���¼����Բ�����(��ʵ������)
        if (ActivityStatus != mLastActivityStatus)
        {
            this.SendEvent(new OnActivityStatusChanged()
            {
                Sender = this,
                Status = ActivityStatus
            });
            mLastActivityStatus = ActivityStatus;
        }
    }
    public string GetActivityReamingTime()
    {
        return CountDownTimerManager.Instance.GetRemainingTimeText(ActivitySign);
    }
    public virtual void Fail()
    {

    }
   
    public virtual void StreakWin()
    {
        
    }

    public virtual void ADPlaybackCompleted<T>(T target) { }
}
