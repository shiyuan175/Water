using GameDefine;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

[MonoSingletonPath("[Text]/TextManager")]
public class TextManager : MonoSingleton<TextManager>, ICanGetUtility, ICanSendEvent
{
    TextAsset text_ZH, text_JA, text_EN, text_KO;
    TextAsset Rank;

    Dictionary<string, string> textDic = new Dictionary<string, string>();
    public List<string> nameList = new List<string>();
    public List<string> starList = new List<string>();

    private void Awake()
    {
        text_ZH = Resources.Load<TextAsset>("Text/Text_ZH");
        //text_JA = Resources.Load<TextAsset>("Text/Text_JA");
        //text_KO = Resources.Load<TextAsset>("Text/Text_KO");
        text_EN = Resources.Load<TextAsset>("Text/Text_EN");
        Rank = Resources.Load<TextAsset>("Text/Rank");
    }

    public override void OnSingletonInit()
    {
        string languageStr = this.GetUtility<SaveDataUtility>().GetSelectLanguage();
        if (languageStr == "-1")
        {
            if (this.GetUtility<LanguageUtility>() == null)
            {
                Debug.Log("没有Utility");
            }
            languageStr = this.GetUtility<LanguageUtility>().GetSystemLanguage();
            this.GetUtility<SaveDataUtility>().SaveSelectLanguage(languageStr);
        }
        ReadTextCfg(languageStr);
    }

    public void ReadTextCfg(string languageType)
    {
        textDic.Clear();

        TextAsset textAsset = text_EN;
        switch (languageType)
        {
            case "zh":
                textAsset = text_ZH;
                break;
            //case "ja":
            //    textAsset = text_JA;
            //    break;
            case "en":
                textAsset = text_EN;
                break;
            //case "ko":
            //    textAsset = text_KO;
            //    break;
        }


        string[] textList = textAsset.text.Split("\r\n");

        foreach (var item in textList)
        {
            item.Replace(" ", "");
            //Debug.Log(item);
            string[] textPair = item.Split("=", 2);
            //string context = "";
            //for(int i = 1; i < textPair.Length; i++)
            //{
            //    context += textPair[i];
            //}
            textDic.Add(textPair[0].Trim(), textPair[1].Trim());
        }

        textList = Rank.text.Split("\r\n");

        foreach (var item in textList)
        {
            item.Replace(" ", "");
            //Debug.Log(item);
            string[] textPair = item.Split("\t", 2);
            nameList.Add(textPair[0].Trim());
            starList.Add(textPair[1].Trim());
        }
    }

    public string GetConvertText(string txt, List<string> replaceStr = null)
    {
        string returnTxt;
        //return (textDic.TryGetValue(txt, out returnTxt) ? returnTxt : "缺少文本");
        string getStr = (textDic.TryGetValue(txt, out returnTxt) ? returnTxt : txt);

        var strsSplit = getStr.Split("<##>");


        var returnStr = "";
        if(replaceStr != null)
        {
            for(int i = 0; i < strsSplit.Length; i++)
            {
                returnStr += strsSplit[i];
                if (replaceStr.Count > i)
                {
                    returnStr += replaceStr[i];
                }
            }
        }
        else
        {
            returnStr = getStr;
        }

        returnStr = returnStr.Replace("\\n", "\n");
        return returnStr;
    }

    public void ChangeLanguage(GameDefine.LanguageType languageType)
    {
        this.GetUtility<SaveDataUtility>().SaveSelectLanguage(languageType);

        ReadTextCfg(languageType.ToString());

        this.SendEvent<RefreshUITextEvent>();
    }

    public string GetLanguage()
    {
        var languageStr = this.GetUtility<SaveDataUtility>().GetSelectLanguage();
        return languageStr.ToUpper();
    }

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }

    /*void Update()
    {
        if(Input.GetKeyUp(KeyCode.K))
        {
            ChangeLanguage(GameDefine.LanguageType.en);
        }
        else if(Input.GetKeyUp(KeyCode.J))
        {
            ChangeLanguage(GameDefine.LanguageType.zh);
        }
    }*/
}
