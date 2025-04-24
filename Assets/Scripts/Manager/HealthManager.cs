using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

[MonoSingletonPath("[Health]/HealthManager")]
public class HealthManager : MonoSingleton<HealthManager> ,ICanGetUtility
{
    private const int maxHp = 5;
    public int nowHp;
    public bool useHp = true;//���ں���������������

    public int NowHp
    {
        get { return nowHp; }
    }

    //�����ָ�һ�������ʱ�䣨60��*30��
    private float recoverTime = 60 * 10;
    //������ȫ�ָ���ʱ���
    public DateTime recoverEndTime;
    //��ǰ�����ָ�ʣ��ʱ��
    public string recoverTimeStr;

    public override void OnSingletonInit()
    {
        Init();
    }

    private void Init()
    {
        //��ȡ��ǰ����
        nowHp = PlayerPrefs.GetInt("_NowHp", maxHp);
        //��ȡ�����ָ���ȫ��ʱ��
        string time = PlayerPrefs.GetString("_RecoverEndTime", string.Empty);
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
    void CheckRecoverHp()
    {
        //�����Ĭ��ֵ ˵��û�������� ���������
        if (recoverEndTime == DateTime.MinValue)
            return;

        //��ȡ�ָ���������������ʱ��
        float timer = GetrecoverTime();
        //����������Ǹ��� ˵�������ָ����
        if (timer <= 0)
        {
            nowHp = maxHp;
            PlayerPrefs.SetInt("_NowHp", maxHp);
            recoverEndTime = DateTime.MinValue;
            PlayerPrefs.SetString("_RecoverEndTime", string.Empty);
        }
        //timer����0 ������Ҫ����ʱ�ָ�����
        else
        {
            //���㻹��Ҫ�ָ����ٵ����� �� ��������ʱ�� / һ�������ָ�ʱ��  =�� ����ȡ��
            int num = (int)Math.Ceiling(timer / recoverTime);
            //����������ֵ num���ڻ��ߵ���5ʱ(���۲�����ִ���maxHp���) ����������һ����0��
            if (num >= maxHp)
                nowHp = 0;

            //�������ֵ - ��������ֵ = ��ǰֵ
            else
                nowHp = maxHp - num;
            //��������ֵ
            PlayerPrefs.SetInt("_NowHp", nowHp);
        }

        //Debug.Log(recoverEndTime);
    }

    /// <summary>
    /// ��ȡ�ָ���������������ʱ��
    /// </summary>
    /// <returns></returns>
    float GetrecoverTime()
    {
        //��ȡ��ǰʱ���� �ָ����ʱ���ʱ����
        TimeSpan recoverInterval = recoverEndTime - DateTime.Now;
        // ��ʱ��תΪ��
        float remainingTime = (float)recoverInterval.TotalSeconds;
        return remainingTime;
    }

    //��ȡ��ǰ��һ�������Ļָ�ʱ��
    TimeSpan GetNowRecoverTime()
    {
        // ���㵱ǰ������Ӧ�Ļָ�ʱ��
        int hpDifference = maxHp - nowHp - 1;
        DateTime nowRecoverTime = recoverEndTime.AddSeconds(-recoverTime * hpDifference);
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
        if (nowHp < maxHp)
        {
            TimeSpan timer = GetNowRecoverTime();
            int minutes = (int)timer.TotalMinutes;
            int seconds = timer.Seconds;
            recoverTimeStr = $"{minutes}:{seconds:D2}";
            if (timer.TotalSeconds <= 0)
            {
                //������һ
                AddHp();

            }
        }
    }

    void AddHp()
    {
        nowHp = nowHp + 1 > maxHp ? maxHp : nowHp + 1;
        PlayerPrefs.SetInt("_NowHp", nowHp);
    }

    //���������ķ���
    public void UseHp()
    {
        if (nowHp > 0)
        {
            nowHp--;
            PlayerPrefs.SetInt("_NowHp", nowHp);
            //����ǵ�һ����������
            if (nowHp == maxHp - 1)
            {
                //������ȫ�ָ�ʱ��
                recoverEndTime = DateTime.Now.AddSeconds(recoverTime);
                //����ָ����ʱ��
                PlayerPrefs.SetString("_RecoverEndTime", recoverEndTime.ToString());
            }
            else if (nowHp >= 0)  //���ǵ�һ����������
            {
                //��ȡ�����ָ���ȫ��ʱ��
                string time = PlayerPrefs.GetString("_RecoverEndTime", string.Empty);
                DateTime lastTime = DateTime.Parse(time);
                recoverEndTime = lastTime.AddSeconds(recoverTime);
                //����ָ����ʱ��
                PlayerPrefs.SetString("_RecoverEndTime", recoverEndTime.ToString());
            }
        }
        else
        {
            nowHp = 0;
            PlayerPrefs.SetInt("_NowHp", 0);
        }
    }

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }
}
