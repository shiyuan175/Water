using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Game.Water;

/// <summary>
/// 
/// </summary>
public class LevelObjectCreator : EditorWindow
{
    #region SO
    private string baseName = "Level";
    private int soStartNum = 1;
    private int soEndNum = 1;
    private string folderPath = "Assets/Scripts/Level/1-1000";
    #endregion

    #region LevelManager
    private LevelManager levelManager;
    private int managerStartIndex;
    private int managerEndIndex;    
    #endregion

    [MenuItem("Tools/Create Level Object Assets")]
    public static void ShowWindow()
    {
        GetWindow<LevelObjectCreator>("Level Object Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Level Object Creation Settings", EditorStyles.boldLabel);

        EditorGUILayout.Space();
        GUILayout.Label("SO Object Generation Settings", EditorStyles.boldLabel);
        baseName = EditorGUILayout.TextField("Base Name:", baseName);
        soStartNum = EditorGUILayout.IntField("SO Start Number:", soStartNum);
        soEndNum = EditorGUILayout.IntField("SO End Number:", soEndNum);
        folderPath = EditorGUILayout.TextField("Folder Path:", folderPath);

        EditorGUILayout.Space();
        GUILayout.Label("Level Manager Assignment Settings", EditorStyles.boldLabel);
        levelManager = (LevelManager)EditorGUILayout.ObjectField(
            "Level Manager:",
            levelManager,
            typeof(LevelManager),
            true);
        managerStartIndex = EditorGUILayout.IntField("Manager Start Index:", managerStartIndex);
        managerEndIndex = EditorGUILayout.IntField("Manager End Index:", managerEndIndex);

        EditorGUILayout.Space();
        if (GUILayout.Button("Create Level Data Assets"))
        {
            CreateLevelObject();
        }

        if (GUILayout.Button("Batch Level Object to LevelManager"))
        {
            BatchLevelToManager();
        }

        if (GUILayout.Button("Batch Rename Level Objects"))
        {
            BatchRenameLevelObjects();
        }

        // ????Щ???
        EditorGUILayout.HelpBox(
            "SO Generation: Creates level asset files based on number range\n" +
            "Manager Assignment: Assigns level objects to specified indices in LevelManager\n" +
            "Note: If SO range is 1-10 and Manager range is 5-14, then Level1.asset will be assigned to levels[5], Level2.asset to levels[6], and so on.",
            MessageType.Info);
    }

    /// <summary>
    /// 创建脚本
    /// </summary>
    private void CreateLevelObject()
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            AssetDatabase.Refresh();
        }

        for (int i = soStartNum; i <= soEndNum; i++)
        {
            string assetName = $"{baseName}{i}";
            string path = $"{folderPath}/{assetName}.asset";

            if (File.Exists(path))
            {
                Debug.LogWarning($"Asset already exists at {path}, skipping...");
                continue;
            }

            LevelCreateCtrl levelObject = CreateInstance<LevelCreateCtrl>();
            levelObject.topNum = 7;
            levelObject.bottomNum = 7;
            levelObject.clearList = new List<int>();

            for (int j = 1; j <= 12; j++)
            {
                if (!(j == 12 || j == 9))
                    levelObject.clearList.Add(j);
            }

            AssetDatabase.CreateAsset(levelObject, path);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Successfully created {soEndNum - soStartNum + 1} LevelCreateCtrl assets in {folderPath}");
    }

    /// <summary>
    /// ????????????object
    /// </summary>
    private void BatchLevelToManager()
    {
      
        if (levelManager == null)
        {
            Debug.LogError("LevelManager is not assigned!");
            return;
        }

        // ?????????Χ
        if (managerStartIndex < 0 || managerEndIndex < 0)
        {
            Debug.LogError("Manager indices cannot be negative!");
            return;
        }

        if (managerStartIndex > managerEndIndex)
        {
            Debug.LogError("Manager start index cannot be greater than end index!");
            return;
        }

        // ?????????SO????????
        int requiredSOCount = managerEndIndex - managerStartIndex + 1;
        int availableSOCount = soEndNum - soStartNum + 1;

        if (availableSOCount < requiredSOCount)
        {
            Debug.LogError($"Not enough SO objects! Need {requiredSOCount}, but only have {availableSOCount} SO objects ({soStartNum}-{soEndNum})");
            return;
        }

        // ???LevelManager???б?????
        if (managerEndIndex >= levelManager.levels.Count)
        {
            int oldCount = levelManager.levels.Count;
            levelManager.levels.AddRange(
                Enumerable.Repeat<LevelCreateCtrl>(null, managerEndIndex - oldCount + 1));
            Debug.Log($"Expanded LevelManager.levels from {oldCount} to {levelManager.levels.Count}");
        }

        // ???????SO??????????LevelManager?????λ??
        int soIndex = soStartNum;
        for (int managerIndex = managerStartIndex;
             managerIndex <= managerEndIndex && soIndex <= soEndNum;
             managerIndex++, soIndex++)
        {
            string objectPath = $"{folderPath}/{baseName}{soIndex}.asset";
            LevelCreateCtrl soAsset = AssetDatabase.LoadAssetAtPath<LevelCreateCtrl>(objectPath);

            if (soAsset == null)
            {
                Debug.LogError($"SO asset not found at path: {objectPath}");
                continue;
            }

            levelManager.levels[managerIndex] = soAsset;
            Debug.Log($"Assigned {baseName}{soIndex}.asset to levels[{managerIndex}]");
        }

        EditorUtility.SetDirty(levelManager);
        AssetDatabase.SaveAssets();
        Debug.Log(
            $"??????: ??SO???? {soStartNum}-{soIndex - 1} ????? LevelManager.levels[{managerStartIndex}]-[{managerEndIndex}]");
    }

    /// <summary>
    /// ???????????????????
    /// </summary>
    private void BatchRenameLevelObjects()
    {
        if (!Directory.Exists(folderPath))
        {
            Debug.LogError($"Directory not found: {folderPath}");
            return;
        }

        // ?????????????е?Level????
        var guids = AssetDatabase.FindAssets("t:LevelCreateCtrl", new[] { folderPath });
        if (guids.Length == 0)
        {
            Debug.LogWarning($"No LevelCreateCtrl assets found in {folderPath}");
            return;
        }

        // ???????е??????????
        var fileInfos = new List<FileInfo>();
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(path);

            // ?????????????????
            Match match = Regex.Match(fileName, @"(\d+)$");
            if (match.Success)
            {
                int number = int.Parse(match.Value);
                fileInfos.Add(new FileInfo
                {
                    Path = path,
                    Number = number,
                    FileName = fileName
                });
            }
            else
            {
                Debug.LogWarning($"Could not extract number from filename: {fileName}");
            }
        }

        // ??????????
        fileInfos = fileInfos.OrderBy(f => f.Number).ToList();

        // ?????????????????
        var duplicates = fileInfos.GroupBy(f => f.Number)
                                  .Where(g => g.Count() > 1)
                                  .Select(g => g.Key)
                                  .ToList();

        if (duplicates.Count > 0)
        {
            Debug.LogError($"Found duplicate numbers: {string.Join(", ", duplicates)}");
            return;
        }

        // ?????????
        int renameCount = 0;
        foreach (var fileInfo in fileInfos)
        {
            string expectedName = $"{baseName}{fileInfo.Number}";

            if (fileInfo.FileName != expectedName)
            {
                string error = AssetDatabase.RenameAsset(fileInfo.Path, expectedName);
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError($"Failed to rename {fileInfo.FileName}: {error}");
                }
                else
                {
                    renameCount++;
                    Debug.Log($"Renamed {fileInfo.FileName} to {expectedName}");
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"??????????????????????{renameCount}?????");
    }

    /// <summary>
    /// ???????洢??????
    /// </summary>
    private class FileInfo
    {
        public string Path { get; set; }
        public int Number { get; set; }
        public string FileName { get; set; }
    }
}