using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDefine;
using System;

public class SaveDataUtility : IUtility, ICanSendEvent
{
    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }

    public void SaveInt(string key,int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }

    public int LoadIntValue(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public void SaveBool(string key,bool value)
    {
        if (value)
            PlayerPrefs.SetInt(key, 1);
        else
            PlayerPrefs.SetInt(key, 0);
        PlayerPrefs.Save();
    }
   
    public bool LoadBoolValue(string key, bool defaultValue = true)
    {
        var value = PlayerPrefs.GetInt(key, defaultValue ? 1 : 0);
        return value == 1;
    }
    public void SaveLevel(int level)
    {
        //Debug.Log("LevelBefore " + Convert.ToString(clearLevel, 2) + " clearNowLevel " + clearNowLevel + " LevelNow " + Convert.ToString((clearLevel | clearNowLevel), 2));
        PlayerPrefs.SetInt("g_ClearWaterLevel", level);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// µ±Ç°¹Ø¿¨
    /// </summary>
    /// <returns></returns>
    public int GetCurrentLevel()
    {
        int clearLevel = PlayerPrefs.GetInt("g_ClearWaterLevel", 1);
        return clearLevel;
    }

    public void SaveChallenge(int level)
    {
        int clearLevel = PlayerPrefs.GetInt("g_ClearWaterChallenge", 0);
        int clearNowLevel = (int)Mathf.Pow(2, level - 1);
        clearLevel = clearLevel | clearNowLevel;
        PlayerPrefs.SetInt("g_ClearWaterChallenge", clearLevel);

    }

    public bool GetChallengeClear(int level)
    {
        int clearLevel = PlayerPrefs.GetInt("g_ClearWaterChallenge", 0);
        int checkLevel = (int)Mathf.Pow(2, level - 1);
        int isClear = clearLevel & checkLevel;
        if (isClear == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public string GetSelectLanguage()
    {
        string language =  PlayerPrefs.GetString("g_WaterLanguage", "-1");

        return language;
    }

    public void SaveSelectLanguage(LanguageType languageType)
    {
        string language = languageType.ToString();
     
        PlayerPrefs.SetString("g_WaterLanguage", language);
    }

    public void SaveSelectLanguage(string language)
    {
        PlayerPrefs.SetString("g_WaterLanguage", language);
    }
}
