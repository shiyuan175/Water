using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// 一个编译器类，用来快速的生成大量符合命名要求的scriptsObject
/// </summary>
public class LevelObjectCreator : EditorWindow
{
    #region 生成的物品的基本配置
    private string baseName = "Level";
    private int startNum = 1;
    private int endNum = 1;
    private string folderPath = "Assets/Scripts/Level/";
    private string objectType = null;
    #endregion

    private LevelManager levelManager;

    [MenuItem("Tools/Create Level Object Assets")]
    public static void ShowWindow()
    {
        GetWindow<LevelObjectCreator>("Level Object Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Level Object Creation Settings", EditorStyles.boldLabel);

        baseName = EditorGUILayout.TextField("Base Name:", baseName);
        startNum = EditorGUILayout.IntField("Start Number:", startNum);
        endNum = EditorGUILayout.IntField("End Number:", endNum);
        folderPath = EditorGUILayout.TextField("Folder Path:", folderPath);
        objectType = EditorGUILayout.TextField("Object Type:", objectType);
        levelManager = (LevelManager)EditorGUILayout.ObjectField(
            "Level Manager:",
            levelManager,
            typeof(LevelManager),
            true);

        if (GUILayout.Button("Create Level Data Assets"))
        {
            CreateLevelObject();
        }

        if (GUILayout.Button("Batch Level Object to LevelManager"))
        {
            BatchLevelToManager();
        }

        // 新增的批量重命名按钮
        if (GUILayout.Button("Batch Rename Level Objects"))
        {
            BatchRenameLevelObjects();
        }
    }

    /// <summary>
    /// 生成数据物体
    /// </summary>
    private void CreateLevelObject()
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            AssetDatabase.Refresh();
        }

        for (int i = startNum; i <= endNum; i++)
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
        Debug.Log($"Successfully created {endNum - startNum + 1} LevelCreateCtrl assets in {folderPath}");
    }

    /// <summary>
    /// 批量给脚本赋值object
    /// </summary>
    private void BatchLevelToManager()
    {
        if (levelManager == null)
        {
            Debug.LogError("LevelManager is not assigned!");
            return;
        }

        while (levelManager.levels.Count < endNum)
            levelManager.levels.Add(new LevelCreateCtrl());

        for (int i = startNum; i <= endNum; i++)
        {
            string objectPath = $"{folderPath}/{baseName}{i}.asset";
            levelManager.levels[i - 1] = AssetDatabase.LoadAssetAtPath<LevelCreateCtrl>(objectPath);
        }

        EditorUtility.SetDirty(levelManager);
        AssetDatabase.SaveAssets();
        Debug.Log("赋值完成");
    }

    /// <summary>
    /// 新增方法：批量重命名指定文件夹中的Level对象
    /// </summary>
    private void BatchRenameLevelObjects()
    {
        if (!Directory.Exists(folderPath))
        {
            Debug.LogError($"Directory not found: {folderPath}");
            return;
        }

        // 获取文件夹中所有的Level对象
        var guids = AssetDatabase.FindAssets("t:LevelCreateCtrl", new[] { folderPath });
        if (guids.Length == 0)
        {
            Debug.LogWarning($"No LevelCreateCtrl assets found in {folderPath}");
            return;
        }

        // 排序文件，确保按正确顺序重命名
        var sortedPaths = guids
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .ToList();



        foreach (var path in sortedPaths)
        {

           
            Match match = Regex.Match(Path.GetFileNameWithoutExtension(path), @"\d+$");
          
            int currentNumber = match.Success ? int.Parse(match.Value) : 0;
            string newName = $"{baseName}{currentNumber}";
            string newPath = $"{folderPath}/{newName}.asset";
            
            
            if (Path.GetFileNameWithoutExtension(path) == newName)
            {
                Debug.LogError("重复");
            }

            string error = AssetDatabase.RenameAsset(path, newName);
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError($"Failed to rename {path}: {error}");
            }
            else
            {
                Debug.Log($"Renamed {Path.GetFileName(path)} to {newName}");
            }

            
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"批量重命名完成，共处理了{guids.Length}个文件");
    }
}