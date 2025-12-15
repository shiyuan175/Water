using System;
using QFramework;
using UnityEngine;

[MonoSingletonPath("[Health]/HealthManager")]
public class HealthManager : MonoSingleton<HealthManager> ,ICanSendEvent ,ICanGetModel
{
    #region Public Field
    /// <summary>
    /// 离线通知用
    /// </summary>
    public DateTime RecoverEndTime => recoverEndTime;

    /// <summary>
    /// 当前体力恢复剩余时间
    /// </summary>
    public string RecoverTimerStr => recoverTimeStr;

    /// <summary>
    /// 当前体力
    /// </summary>
    public int NowHp => nowHp;

    /// <summary>
    /// 已使用体力
    /// </summary>
    public int UsedHp => maxHp - nowHp;

    /// <summary>
    /// 是否满体力
    /// </summary>
    public bool IsMaxHp => nowHp == maxHp;

    /// <summary>
    /// 当前恢复的体力点
    /// </summary>
    public int CurRecoverySlot => nowHp == maxHp ? maxHp : nowHp + 1;

    /// <summary>
    /// 是否有体力
    /// </summary>
    public bool HasHp => nowHp > 0;

    /// <summary>
    /// 无限体力状态
    /// </summary>
    public bool UnLimitHp => unLimitHp;

    /// <summary>
    /// 无限体力剩余时长
    /// </summary>
    public string UnLimitHpTimeStr => unLimitHpTimeStr;
    
    #endregion
    
    #region private Field
    private const int MINHP = 5;
    private const float SECOND = 60;

    private int maxHp;
    private int nowHp;
    private float recovertimer;
    
    private DateTime recoverEndTime;        // 体力完全恢复的时间点
    private string recoverTimeStr;

    private DateTime unLimitHpEndTime;      // 无限体力截止的时间点
    private string unLimitHpTimeStr;
    private bool unLimitHp;
    
    private GameGlobalModel mGameGlobalModel;

    
    #endregion

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
        mGameGlobalModel = this.GetModel<GameGlobalModel>();
        mGameGlobalModel.LoadGlobalJson();
        maxHp = mGameGlobalModel.GameGlobalJsonData.MaxHp > MINHP ? mGameGlobalModel.GameGlobalJsonData.MaxHp : MINHP;
        recovertimer = mGameGlobalModel.GameGlobalJsonData.HpRecoverTimer * SECOND;
        
        nowHp = GetNowHp();
        recoverTimeStr = "00:00";
        string time = GetRecoverEndTime();

        if (!string.IsNullOrEmpty(time))
            recoverEndTime = DateTime.Parse(time);
        else
            recoverEndTime = DateTime.MinValue;


        //检查体力是否需要恢复
        CheckRecoverHp();

        unLimitHpTimeStr = "00:00";
        string UnLimitTime = GetUnLimitHpEndTime();
        if (!string.IsNullOrEmpty(UnLimitTime))
            unLimitHpEndTime = DateTime.Parse(UnLimitTime);
        else
            unLimitHpEndTime = DateTime.MinValue;

        //检查无限体力是否到期
        unLimitHp = CheckUnLimitHp();
    }

    /// <summary>
    /// 检查体力是否需要恢复
    /// </summary>
    private void CheckRecoverHp()
    {
        //默认值无消耗体力
        if (recoverEndTime == DateTime.MinValue)
            return;

        float timer = GetRemainingTime(recoverEndTime);
        // 体力恢复完成
        if (timer <= 0)
            SaveNowHpToMax();
        else
        {
            //计算还需要恢复多少点体力 ， 回满所需时间 / 一点体力恢复时间  =》 向上取整
            int num = (int)Math.Ceiling(timer / recovertimer);
            //当回满所需值 num大于或者等于5时(理论不会出现大于maxHp情况) ，代表体力一定是0点
            nowHp = num >= maxHp ? 0 : maxHp - num;
            SaveNowHp(nowHp);
        }
    }

    /// <summary>
    /// 获取当前这一点体力的恢复时间
    /// </summary>
    /// <returns></returns>
    private TimeSpan GetNowRecoverTime()
    {
        // 用于计算当前体力对应的恢复时间(即往前倒推 hpDifference 个恢复周期)
        int hpDifference = maxHp - nowHp - 1;
        // AddSeconds,增加秒数(扣掉hpDifference点恢复时间，剩余的表示当前体力恢复时长)
        DateTime nowRecoverTime = recoverEndTime.AddSeconds(-recovertimer * hpDifference);
        // 获取当前时间与恢复完成时间的时间间隔
        TimeSpan recoverInterval = nowRecoverTime - DateTime.Now;
        if (recoverInterval.TotalSeconds < 0)
        {
            recoverInterval = TimeSpan.Zero;
        }
        return recoverInterval;
    }

    /// <summary>
    /// 倒计时结束恢复一点体力
    /// </summary>
    private void CountDownAddHp()
    {
        nowHp = nowHp + 1 > maxHp ? maxHp : nowHp + 1;
        SaveNowHp(nowHp);
    }

    /// <summary>
    /// 检查无限体力是否到期
    /// </summary>
    private bool CheckUnLimitHp()
    {
        if (unLimitHpEndTime == DateTime.MinValue)
            return false;

        float timer = GetRemainingTime(unLimitHpEndTime);
        if (timer > 0)
            return true;
        else
        {
            unLimitHpEndTime = DateTime.MinValue;
            unLimitHpTimeStr = "00:00";
            SaveUnLimitHpEndTime(string.Empty);
            return false;
        }
    }

    /// <summary>
    /// 获取恢复所有体力的所需时间
    /// 获取无限体力剩余时间
    /// </summary>
    /// <returns></returns>
    private float GetRemainingTime(DateTime endTime)
    {
        //获取当前时间与 目标时间的时间间隔
        TimeSpan recoverInterval = endTime - DateTime.Now;
        float remainingTime = (float)recoverInterval.TotalSeconds;
        return remainingTime;
    }

    private void Update()
    {
        if (nowHp < maxHp)
        {
            //TotalMinutes转为总分钟数
            //TotalSeconds转为总秒数
            TimeSpan timer = GetNowRecoverTime();
            //Debug.Log($"总分钟数:{timer.TotalSeconds}");
            //Debug.Log($"总秒数:{timer.TotalMinutes}");
            //Debug.Log($"分钟数:{timer.Seconds}");
            //Debug.Log($"秒数:{timer.Minutes}");
            int minutes = (int)timer.TotalMinutes;
            int seconds = timer.Seconds;
            recoverTimeStr = $"{minutes}:{seconds:D2}";
            if (timer.TotalSeconds <= 0)
                CountDownAddHp();
        }

        if (unLimitHp)
        {
            float remainingTime = GetRemainingTime(unLimitHpEndTime);
            TimeSpan timer = TimeSpan.FromSeconds(remainingTime);
            //unLimitHpTimeStr = $"{(int)timer.TotalMinutes}:{timer.Seconds:D2}";
            unLimitHpTimeStr = timer.ToString(@"hh\:mm\:ss");
            if (remainingTime <= 0)
            {
                unLimitHpEndTime = DateTime.MinValue;
                unLimitHpTimeStr = "00:00";
                SaveUnLimitHpEndTime(string.Empty);
                unLimitHp = false;
                // 更新UI状态(取消无限体力状态)
                this.SendEvent<VitalityChangeEvent>(new VitalityChangeEvent());
            }
        }
    }
    
    /// <summary>
    /// 存储当前体力
    /// </summary>
    /// <param name="value"></param>
    private void SaveNowHp(int value)
    {
        PlayerPrefs.SetInt("_NowHp", value);
        //发送事件通知体力变更等
        this.SendEvent<VitalityChangeEvent>(new VitalityChangeEvent());
        
    }
    
    /// <summary>
    /// 购买体力/离线体力恢复满/满体力等情况调用
    /// </summary>
    private void SaveNowHpToMax()
    {
        nowHp = maxHp;
        recoverEndTime = DateTime.MinValue;
        recoverTimeStr = "00:00";
        SaveNowHp(maxHp);
        SaveRecoverEndTime(string.Empty);
    }

    /// <summary>
    /// 获取当前体力
    /// </summary>
    /// <returns></returns>
    private int GetNowHp()
    {
        return PlayerPrefs.GetInt("_NowHp", maxHp);
    }

    /// <summary>
    /// 获取体力完全恢复的时间点
    /// </summary>
    /// <returns></returns>
    private string GetRecoverEndTime()
    {
       return PlayerPrefs.GetString("_RecoverEndTime", string.Empty);
    }

    /// <summary>
    /// 保存体力完全恢复的时间点
    /// </summary>
    /// <param name="value"></param>
    private void SaveRecoverEndTime(string value)
    {
        PlayerPrefs.SetString("_RecoverEndTime", value);
    }

    /// <summary>
    /// 设置无限体力截止时间
    /// </summary>
    /// <param name="value"></param>
    private void SaveUnLimitHpEndTime(string value)
    {
        PlayerPrefs.SetString("_UnLimitHpEndTime", value);
    }

    /// <summary>
    /// 获取无限体力截止时间
    /// </summary>
    private string GetUnLimitHpEndTime()
    {
        return PlayerPrefs.GetString("_UnLimitHpEndTime", string.Empty);
    }
    
    #endregion

    /// <summary>
    /// 恢复一点体力
    /// </summary>
    public void AddHp()
    {
        nowHp = nowHp + 1 > maxHp ? maxHp : nowHp + 1;
        if (nowHp == maxHp)
        {
            SaveNowHpToMax();
            return;
        }

        SaveNowHp(nowHp);
        //重新设置体力恢复满的时间点(用已使用体力设置)
        recoverEndTime = DateTime.Now.AddSeconds(UsedHp * recovertimer);
        SaveRecoverEndTime(recoverEndTime.ToString());
    }

    /// <summary>
    /// 消耗体力的方法(消耗一点体力)
    /// </summary>
    public void UseHp()
    {
        if (unLimitHp)
            return;

        if (nowHp > 0)
        {
            nowHp--;
            SaveNowHp(nowHp);
            if (nowHp == maxHp - 1)
            {
                recoverEndTime = DateTime.Now.AddSeconds(recovertimer);
                SaveRecoverEndTime(recoverEndTime.ToString());
            }
            else if (nowHp >= 0)
            {
                //获取体力恢复完全的时间
                string time = GetRecoverEndTime();
                DateTime lastTime = DateTime.Parse(time);
                recoverEndTime = lastTime.AddSeconds(recovertimer);
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
    /// 恢复满体力
    /// </summary>
    public void SetNowHpToMax()
    {
        SaveNowHpToMax();
    }

    /// <summary>
    /// 设置无限体力
    /// </summary>
    /// <param name="minute">时长(单位分钟)</param>
    public void SetUnLimitHp(int minute)
    {
        unLimitHp = true;

        // 判断是否累加时长
        string time = GetUnLimitHpEndTime();
        if (!string.IsNullOrEmpty(time))
            unLimitHpEndTime = unLimitHpEndTime.AddSeconds(minute * SECOND);
        else
            unLimitHpEndTime = DateTime.Now.AddSeconds(minute * SECOND);
        SaveUnLimitHpEndTime(unLimitHpEndTime.ToString());

        // 优化项
        // 无限体力的时间大于当前的体力恢复时间则恢复满
        if (nowHp < maxHp)
        {
            float remainingRecoverTime = GetRemainingTime(recoverEndTime);
            //float unLimitDuration = minute * SECOND;
            float unLimitDuration = GetRemainingTime(unLimitHpEndTime);
            if (unLimitDuration >= remainingRecoverTime)
                SetNowHpToMax();
        }

        // 更新UI状态(设置无限体力状态)
        this.SendEvent<VitalityChangeEvent>(new VitalityChangeEvent());
    }

    /// <summary>
    /// 重新更新体力恢复满的时刻
    /// </summary>
    public void RecalculateRecoverEndTime()
    {
        maxHp = mGameGlobalModel.GameGlobalJsonData.MaxHp > MINHP ? mGameGlobalModel.GameGlobalJsonData.MaxHp : MINHP;
        recovertimer = mGameGlobalModel.GameGlobalJsonData.HpRecoverTimer * SECOND;

        //还需要考虑当前体力的剩余恢复时间还要多长，
        //否则会出现，还差1分钟恢复满体力，原先是30分钟，获取了减10分钟后，然后又变成20分钟恢复当前体力

        if (nowHp >= maxHp) return;
        recoverEndTime = DateTime.Now.AddSeconds(UsedHp * recovertimer);
        SaveRecoverEndTime(recoverEndTime.ToString());
    }

    #region TestFun

    public void CancelUnLimitHp()
    {
        unLimitHpEndTime = DateTime.MinValue;
        unLimitHpTimeStr = "00:00";
        SaveUnLimitHpEndTime(string.Empty);
    }
    #endregion
}
