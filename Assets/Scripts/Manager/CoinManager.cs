using QFramework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GameDefine;

public class CoinManager : MonoSingleton<CoinManager>, ICanSendEvent, ICanGetUtility, ICanRegisterEvent
{
    private int coin;
    public int Coin => coin;

    public override void OnSingletonInit()
    {

    }

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }

    #region Coin data storage
    private void SaveCoinNum(int value)
    {
        PlayerPrefs.SetInt("g_WaterCoinNum", value);
        this.SendEvent<CoinChangeEvent>(new CoinChangeEvent() { coin = coin});
    }

    private int GetCoinNum()
    {
        return PlayerPrefs.GetInt("g_WaterCoinNum", 0);
    }
    #endregion

    private void Awake()
    {
        coin = GetCoinNum();
    }

    /// <summary>
    /// Use Coins
    /// </summary>
    /// <param name="costValue"></param>
    /// <param name="action">Callback</param>
    /// <returns></returns>
    public void CostCoin(int costValue ,Action action = null)
    {
        if (costValue > coin)
            return;

        coin -= costValue;
        SaveCoinNum(coin);
        action?.Invoke();
    }

    /// <summary>
    /// Add Coins
    /// </summary>
    /// <param name="addValue"></param>
    /// <param name="action">Callback</param>
    public void AddCoin(int addValue,Action action = null)
    {
        coin += addValue;
        SaveCoinNum(coin);
    }
}

