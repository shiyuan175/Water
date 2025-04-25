using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

[MonoSingletonPath("[Health]/HealthManager")]
public class HealthManager : MonoSingleton<HealthManager> ,ICanSendEvent
{
    private const int MAXHP = 5;
    private const float RECOVERTIME = 1800;
    [SerializeField] private int nowHp;
    //������ȫ�ָ���ʱ���
    private DateTime recoverEndTime;
    [SerializeField] private string recoverTimeStr;

    //���ں���������������
    public bool UseHpSign = true;

    /// <summary>
    /// ��ǰ�����ָ�ʣ��ʱ��
    /// </summary>
    public string RecoverTimerStr => recoverTimeStr;

    /// <summary>
    /// ��ǰ����
    /// </summary>
    public int NowHp => nowHp;

    /// <summary>
    /// ��ʹ������
    /// </summary>
    public int UsedHp => MAXHP - nowHp;

    /// <summary>
    /// ��ǰ�ָ���������
    /// </summary>
    public int CurRecoverySlot => nowHp == MAXHP ? MAXHP : nowHp + 1;

    /// <summary>
    /// �Ƿ�������
    /// </summary>
    public bool HasHp => nowHp > 0;

    #region QF

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }

    public override void OnSingletonInit()
    {
        Init();
    }

    #endregion

    #region Private Function

    private void Init()
    {
        nowHp = GetNowHp();
        recoverTimeStr = "00:00";
        string time = GetRecoverEndTime();

        if (!string.IsNullOrEmpty(time))
            recoverEndTime = DateTime.Parse(time);
        else
            recoverEndTime = DateTime.MinValue;

        //��������Ƿ���Ҫ�ָ�
        CheckRecoverHp();
    }

    /// <summary>
    /// ��������Ƿ���Ҫ�ָ�
    /// </summary>
    private void CheckRecoverHp()
    {
        //Ĭ��ֵ����������
        if (recoverEndTime == DateTime.MinValue)
            return;

        float timer = GetrecoverTime();
        // �����ָ����
        if (timer <= 0)
            SaveNowHpToMax();
        else
        {
            //���㻹��Ҫ�ָ����ٵ����� �� ��������ʱ�� / һ�������ָ�ʱ��  =�� ����ȡ��
            int num = (int)Math.Ceiling(timer / RECOVERTIME);
            //����������ֵ num���ڻ��ߵ���5ʱ(���۲�����ִ���maxHp���) ����������һ����0��
            nowHp = num >= MAXHP ? 0 : MAXHP - num;
            SaveNowHp(nowHp);
        }
    }

    /// <summary>
    /// ��ȡ�ָ���������������ʱ��
    /// </summary>
    /// <returns></returns>
    private float GetrecoverTime()
    {
        //��ȡ��ǰʱ���� �ָ����ʱ���ʱ����
        TimeSpan recoverInterval = recoverEndTime - DateTime.Now;
        float remainingTime = (float)recoverInterval.TotalSeconds;
        return remainingTime;
    }

    /// <summary>
    /// ��ȡ��ǰ��һ�������Ļָ�ʱ��
    /// </summary>
    /// <returns></returns>
    private TimeSpan GetNowRecoverTime()
    {
        // ���ڼ��㵱ǰ������Ӧ�Ļָ�ʱ��(����ǰ���� hpDifference ���ָ�����)
        int hpDifference = MAXHP - nowHp - 1;
        // AddSeconds,��������(�۵�hpDifference��ָ�ʱ�䣬ʣ��ı�ʾ��ǰ�����ָ�ʱ��)
        DateTime nowRecoverTime = recoverEndTime.AddSeconds(-RECOVERTIME * hpDifference);
        // ��ȡ��ǰʱ����ָ����ʱ���ʱ����
        TimeSpan recoverInterval = nowRecoverTime - DateTime.Now;
        if (recoverInterval.TotalSeconds < 0)
        {
            recoverInterval = TimeSpan.Zero;
        }
        return recoverInterval;
    }

    private void Update()
    {
        if (nowHp < MAXHP)
        {
            //TotalMinutesתΪ�ܷ�����
            //TotalSecondsתΪ������
            TimeSpan timer = GetNowRecoverTime();
            //Debug.Log($"�ܷ�����:{timer.TotalSeconds}");
            //Debug.Log($"������:{timer.TotalMinutes}");
            //Debug.Log($"������:{timer.Seconds}");
            //Debug.Log($"����:{timer.Minutes}");
            int minutes = (int)timer.TotalMinutes;
            int seconds = timer.Seconds;
            recoverTimeStr = $"{minutes}:{seconds:D2}";
            if (timer.TotalSeconds <= 0)
                //AddHp();
                CountDownAddHp();
        }
    }

    /// <summary>
    /// ����ʱ�����ָ�һ������
    /// </summary>
    private void CountDownAddHp()
    {
        nowHp = nowHp + 1 > MAXHP ? MAXHP : nowHp + 1;
        SaveNowHp(nowHp);
    }
    
    #endregion

    #region SaveHp

    /// <summary>
    /// �洢��ǰ����
    /// </summary>
    /// <param name="value"></param>
    private void SaveNowHp(int value)
    {
        PlayerPrefs.SetInt("_NowHp", value);
        //�����¼�֪ͨ���������
        this.SendEvent<VitalityChangeEvent>(new VitalityChangeEvent());
        
    }
    
    /// <summary>
    /// ��������/���������ָ���/���������������
    /// </summary>
    private void SaveNowHpToMax()
    {
        nowHp = MAXHP;
        recoverEndTime = DateTime.MinValue;
        recoverTimeStr = "00:00";
        SaveNowHp(MAXHP);
        SaveRecoverEndTime(string.Empty);
    }

    /// <summary>
    /// ��ȡ��ǰ����
    /// </summary>
    /// <returns></returns>
    private int GetNowHp()
    {
        return PlayerPrefs.GetInt("_NowHp", MAXHP);
    }

    /// <summary>
    /// ��ȡ������ȫ�ָ���ʱ���
    /// </summary>
    /// <returns></returns>
    private string GetRecoverEndTime()
    {
       return PlayerPrefs.GetString("_RecoverEndTime", string.Empty);
    }

    /// <summary>
    /// ����������ȫ�ָ���ʱ���
    /// </summary>
    /// <param name="value"></param>
    private void SaveRecoverEndTime(string value)
    {
        PlayerPrefs.SetString("_RecoverEndTime", value);
    }

    #endregion

    /// <summary>
    /// �ָ�һ������
    /// </summary>
    public void AddHp()
    {
        nowHp = nowHp + 1 > MAXHP ? MAXHP : nowHp + 1;
        if (nowHp == MAXHP)
        {
            SaveNowHpToMax();
            return;
        }

        SaveNowHp(nowHp);
        //�������������ָ�����ʱ���(����ʹ����������)
        recoverEndTime = DateTime.Now.AddSeconds(UsedHp * RECOVERTIME);
        SaveRecoverEndTime(recoverEndTime.ToString());
    }

    /// <summary>
    /// ���������ķ���(����һ������)
    /// </summary>
    public void UseHp()
    {
        if (nowHp > 0)
        {
            nowHp--;
            SaveNowHp(nowHp);
            if (nowHp == MAXHP - 1)
            {
                recoverEndTime = DateTime.Now.AddSeconds(RECOVERTIME);
                SaveRecoverEndTime(recoverEndTime.ToString());
            }
            else if (nowHp >= 0)
            {
                //��ȡ�����ָ���ȫ��ʱ��
                string time = GetRecoverEndTime();
                DateTime lastTime = DateTime.Parse(time);
                recoverEndTime = lastTime.AddSeconds(RECOVERTIME);
                SaveRecoverEndTime(recoverEndTime.ToString());
            }
        }
        else
        {
            nowHp = 0;
            SaveNowHp(nowHp);
        }
    }

    /// <summary>
    /// �ָ�������
    /// </summary>
    public void SetNowHpToMax()
    {
        SaveNowHpToMax();
    }
}
