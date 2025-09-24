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
    /// 用于版本对比
    /// </summary>
    public class VersionWrapper
    {
        public int Version;
    }

    /// <summary>
    /// 用于声明文件信息和版本
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

    #region DailyTask AD Activity Data
    public class Reward
    {
        // 待补充
    }
    public class TaskItem
    {
        public string TypeName;
        public List<Reward> Rewards;
    }
    
    public class TaskGroup
    {
        public int TaskId;
        public List<TaskItem> TaskItems;
    }

    public class DailyTaskActivityData
    {
        List<TaskGroup> DailyTaskData;
    }
    #endregion

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



}

public class JsonFileUtility : IUtility
{
    private readonly JsonFileInfo[] mJsonFileData = new JsonFileInfo[]
    {
        GameDefine.GameConst.MSADefaultJson,
        GameDefine.GameConst.TRADefaultJson
    };

    /// <summary>
    /// 从 JSON 文件读取对象
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="action"></param>
    public void LoadFromJson(string filePath, Action<string> action)
    {
        if (!File.Exists(filePath))
        {
            //Debug.Log($"文件不存在: {filePath}");
            return;
        }

        string json = File.ReadAllText(filePath);
        action?.Invoke(json);
    }

    /// <summary>
    /// 保存对象为 JSON 文件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filePath"></param>
    /// <param name="data"></param>
    public void SaveToJson<T>(string filePath, T data)
    {
        //确保路径存在
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        string _json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(filePath, _json);
        //Debug.Log($"数据已保存: {filePath}");
    }

    #region 拷贝文件到持久化路径
    /// 注意事项：
    /// 每个Json文件应有 Version 字段 ,文件信息需配置目标版本
    /// StreamingAssets 下的 Json 应为最新版本

    /*public IEnumerator UpdateJsonFiles()
    {
        bool _needUpdate;

        for (int i = 0; i < mJsonFileData.Length; i++)
        {
            _needUpdate = true;

            var _perFilePath = Path.Combine(Application.persistentDataPath, mJsonFileData[i].FileName);
            if (File.Exists(_perFilePath))
            {
                Debug.Log($"文件:{mJsonFileData[i].FileName} 已存在");
                int _localVersion = GetFileVersion(_perFilePath);
                Debug.Log("当前Json版本：" + _localVersion);

                if (_localVersion >= mJsonFileData[i].TargetVersion)
                    _needUpdate = false;

                yield return null;
            }

            if (_needUpdate)
            {
                Debug.Log($"文件:{mJsonFileData[i]} 不存在或版本过低，更新中...");
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
                    Debug.LogError("拷贝失败: " + request.error);
                }
#else
                //非安卓
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
               //Debug.Log($"文件:{mJsonFileData[i].FileName} 已存在");
               int _localVersion = GetFileVersion(_perFilePath);
               //Debug.Log($"{mJsonFileData[i].FileName} 当前版本：" + _localVersion);

               if (_localVersion >= mJsonFileData[i].TargetVersion)
                   _needUpdate = false;
           }

           if (_needUpdate)
           {
               //Debug.Log($"文件:{mJsonFileData[i]} 不存在或版本过低，更新中...");
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
                   //Debug.LogError("拷贝失败: " + request.error);
               }
           }
#else
               // 非安卓
               await Task.Run(() =>
               {
                   File.Copy(Path.Combine(Application.streamingAssetsPath, mJsonFileData[i].FileName), _perFilePath, overwrite: true);
               });
#endif
           }
       }
    }

    /// <summary>
    /// 获取Json版本号
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
