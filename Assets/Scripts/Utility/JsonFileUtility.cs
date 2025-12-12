using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JsonFileData;
using Newtonsoft.Json;
using QFramework;
using UnityEngine;
using UnityEngine.Networking;

namespace JsonFileData
{
    /// <summary>
    /// ���ڰ汾�Ա�
    /// </summary>
    public class VersionWrapper
    {
        public int Version;
    }

    /// <summary>
    /// ���������ļ���Ϣ�Ͱ汾
    /// </summary>
    public class JsonFileInfo
    {
        public string FileName;
        public int TargetVersion;
    }

    #region Magic Streak Activity Data
    public class MSActivityData
    {
        public MSAPlayer Player;
        public List<MSARobotsData> MSARobots;
    }

    public class MSAPlayer
    {
        public string PlayerName;
        public int Score;
    }

    public class MSARobotsData
    {
        public int ID;
        public string Name;
        public int Avatar;
        public int AvatarFrame;
        public int MinInitScore;
        public int MaxInitScore;
        public int LimitScore;
        public int Score;
    }

    #endregion
    
    public class RewardItem
    {
        public string itemType;
        public int itemQuantity;
    }
    #region Tier Rank Activity Data

    public class TRActivityData
    {
        public TRAPlayer Player;
        public List<TRARobotsData> TRARobots;
    }

    public class TRAPlayer
    {
        public string PlayerName;
        public int StreamWinNum;
        public bool IsRewardSettled;
    }

    public class TRARobotsData
    {
        public int ID;
        public string Name;
        public int Avatar;
        public int AvatarFrame;
        public int StreamWinNum;
    }
    #endregion


    #region BattlePass Data

    public class BattlePassData
    {
        public int BattlePassVersion;
        public BPReward[] Rewards;
    }
    public class BPReward
    {
        public int GetConditions;
        public RewardItem[] Free;
        public RewardItem[] Vip;
        public bool FreeIsBox;
        public bool VipIsBox;
    }



    #endregion

    #region PrograssGiftADActivityModel
    public class PGData
    {
        public int PGVersion;
        public PGReward[] Rewards;

    }
    public class PGReward
    {
        public float Price;
        public RewardItem[] RewardItem;
    }

    #endregion



}

public class JsonFileUtility : IUtility
{
    // Ĭ�ϵ�json������ͬ���汾��current
    private readonly JsonFileInfo[] mJsonFileData = new JsonFileInfo[]
    {
        GameDefine.GameConst.MSADefaultJson,
        GameDefine.GameConst.TRADefaultJson,
        GameDefine.GameConst.BPDefaultJson,
        GameDefine.GameConst.PGDefaultJson
    };

    /// <summary>
    /// �� JSON �ļ���ȡ����
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="action"></param>
    public void LoadFromJson(string filePath, Action<string> action)
    {
        if (!File.Exists(filePath))
        {
            /*   Debug.Log($"�ļ�������: {filePath}");*/
            return;
        }

        string json = File.ReadAllText(filePath);
        action?.Invoke(json);
    }

    /// <summary>
    /// �������Ϊ JSON �ļ�
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filePath"></param>
    /// <param name="data"></param>
    public void SaveToJson<T>(string filePath, T data)
    {
        //ȷ��·������
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        string _json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(filePath, _json);
        //Debug.Log($"�����ѱ���: {filePath}");
    }

    #region �����ļ����־û�·��
    /// ע�����
    /// ÿ��Json�ļ�Ӧ�� Version �ֶ� ,�ļ���Ϣ������Ŀ��汾
    /// StreamingAssets �µ� Json ӦΪ���°汾

    /*public IEnumerator UpdateJsonFiles()
    {
        bool _needUpdate;

        for (int i = 0; i < mJsonFileData.Length; i++)
        {
            _needUpdate = true;

            var _perFilePath = Path.Combine(Application.persistentDataPath, mJsonFileData[i].FileName);
            if (File.Exists(_perFilePath))
            {
                Debug.Log($"�ļ�:{mJsonFileData[i].FileName} �Ѵ���");
                int _localVersion = GetFileVersion(_perFilePath);
                Debug.Log("��ǰJson�汾��" + _localVersion);

                if (_localVersion >= mJsonFileData[i].TargetVersion)
                    _needUpdate = false;

                yield return null;
            }

            if (_needUpdate)
            {
                Debug.Log($"�ļ�:{mJsonFileData[i]} �����ڻ�汾���ͣ�������...");
#if UNITY_ANDROID && !UNITY_EDITOR
                var streamingAssetsFilePath = Path.Combine(Application.streamingAssetsPath, mJsonFileData[i].FileName);
                UnityWebRequest request = UnityWebRequest.Get(streamingAssetsFilePath);
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.Success)
                {
                    File.WriteAllBytes(_perFilePath, request.downloadHandler.data);
                }
                else
                {
                    Debug.LogError("����ʧ��: " + request.error);
                }
#else
                //�ǰ�׿
                File.Copy(Path.Combine(Application.streamingAssetsPath, mJsonFileData[i].FileName), _perFilePath, true);
                yield return null;
#endif
            }
        }
    }*/

    public async Task UpdateJsonFiles()
    {
        bool _needUpdate;
        for (int i = 0; i < mJsonFileData.Length; i++)
        {
            _needUpdate = true;

            var _perFilePath = Path.Combine(Application.persistentDataPath, mJsonFileData[i].FileName);
            if (File.Exists(_perFilePath))
            {
                //Debug.Log($"�ļ�:{mJsonFileData[i].FileName} �Ѵ���");
                int _localVersion = GetFileVersion(_perFilePath);
                //Debug.Log($"{mJsonFileData[i].FileName} ��ǰ�汾��" + _localVersion);
                // ��ȡĬ�ϵ�json�ȶ�json��version��targeversion
                if (_localVersion >= mJsonFileData[i].TargetVersion)
                    _needUpdate = false;
            }

            if (_needUpdate)
            {
                //Debug.Log($"�ļ�:{mJsonFileData[i]} �����ڻ�汾���ͣ�������...");
#if UNITY_ANDROID && !UNITY_EDITOR
           var streamingAssetsFilePath = Path.Combine(Application.streamingAssetsPath, mJsonFileData[i].FileName);
           using (UnityWebRequest request = UnityWebRequest.Get(streamingAssetsFilePath))
           {
               var operation = request.SendWebRequest();
               while (!operation.isDone)
                   await Task.Yield();

               if (request.result == UnityWebRequest.Result.Success)
               {
                   File.WriteAllBytes(_perFilePath, request.downloadHandler.data);
               }
               else
               {
                   //Debug.LogError("����ʧ��: " + request.error);
               }
           }
#else
                // �ǰ�׿
                await Task.Run(() =>
                {
                    File.Copy(Path.Combine(Application.streamingAssetsPath, mJsonFileData[i].FileName), _perFilePath, overwrite: true);

                });
#endif
            }
        }
    }

    /// <summary>
    /// ��ȡJson�汾��
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public int GetFileVersion(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            var versionData = JsonConvert.DeserializeObject<VersionWrapper>(json);
            return versionData.Version;
        }
        catch
        {
            return 0;
        }
    }

    #endregion
}
