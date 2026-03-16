using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// 关卡重命名工具：专门用于将 L1-L100 重命名为 Level120-Level220
/// </summary>
public class LevelRenamer : EditorWindow
{
    #region 可配置参数

    private string sourceFolderPath = "Assets/Scripts/Water/Level/";
    private string originalPrefix = "L";
    private string newPrefix = "Level";
    private int startOffset = 119; // L1 -> Level120 (1 + 119 = 120)
    private int startNumber = 1;
    private int endNumber = 100;
    #endregion

    [MenuItem("Tools/Level Renamer")]
    public static void ShowWindow()
    {
        GetWindow<LevelRenamer>("Level Renamer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Level Renaming Settings", EditorStyles.boldLabel);

        sourceFolderPath = EditorGUILayout.TextField("Source Folder Path:", sourceFolderPath);
        originalPrefix = EditorGUILayout.TextField("Original Prefix:", originalPrefix);
        newPrefix = EditorGUILayout.TextField("New Prefix:", newPrefix);
        startOffset = EditorGUILayout.IntField("Start Offset:", startOffset);
        startNumber = EditorGUILayout.IntField("Start Number:", startNumber);
        endNumber = EditorGUILayout.IntField("End Number:", endNumber);

        EditorGUILayout.Space();
        GUILayout.Label($"重命名规则: {originalPrefix}[1-100] 转换为 {newPrefix}[{120}-{220}]", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox($"这将把 {originalPrefix}{startNumber}-{originalPrefix}{endNumber} 重命名为 {newPrefix}{startNumber + startOffset}-{newPrefix}{endNumber + startOffset}", MessageType.Info);

        if (GUILayout.Button("执行重命名", GUILayout.Height(30)))
        {
            ExecuteRename();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("预览重命名结果"))
        {
            PreviewRename();
        }
    }

    /// <summary>
    /// 预览重命名结果
    /// </summary>
    private void PreviewRename()
    {
        if (!Directory.Exists(sourceFolderPath))
        {
            Debug.LogError($"Directory not found: {sourceFolderPath}");
            return;
        }

        var guids = AssetDatabase.FindAssets("t:LevelCreateCtrl", new[] { sourceFolderPath });
        if (guids.Length == 0)
        {
            Debug.LogWarning($"No LevelCreateCtrl assets found in {sourceFolderPath}");
            return;
        }

        Debug.Log("=== 预览重命名结果 ===");

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(path);

            // 正则匹配原始格式 L + 数字
            Match match = Regex.Match(fileName, $@"^{Regex.Escape(originalPrefix)}(\d+)$");

            if (match.Success)
            {
                int currentNumber = int.Parse(match.Groups[1].Value);

                // 检查数字是否在指定范围内
                if (currentNumber >= startNumber && currentNumber <= endNumber)
                {
                    int newNumber = currentNumber + startOffset;
                    string newName = $"{newPrefix}{newNumber}";

                    Debug.Log($"将重命名: {fileName} 为 {newName}");
                }
                else
                {
                    Debug.Log($"跳过 {fileName} (数字不在范围 {startNumber}-{endNumber} 内)");
                }
            }
            else
            {
                Debug.Log($"跳过 {fileName} (格式不匹配)");
            }
        }

        Debug.Log("=== 预览结束 ===");
    }

    /// <summary>
    /// 执行重命名操作
    /// </summary>
    private void ExecuteRename()
    {
        if (!Directory.Exists(sourceFolderPath))
        {
            Debug.LogError($"Directory not found: {sourceFolderPath}");
            return;
        }

        var guids = AssetDatabase.FindAssets("t:LevelCreateCtrl", new[] { sourceFolderPath });
        if (guids.Length == 0)
        {
            Debug.LogWarning($"No LevelCreateCtrl assets found in {sourceFolderPath}");
            return;
        }

        int successCount = 0;
        int skipCount = 0;
        int errorCount = 0;

        // 收集所有需要重命名的文件信息
        var renameOperations = new List<RenameOperation>();

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(path);

            Match match = Regex.Match(fileName, $@"^{Regex.Escape(originalPrefix)}(\d+)$");

            if (match.Success)
            {
                int currentNumber = int.Parse(match.Groups[1].Value);

                if (currentNumber >= startNumber && currentNumber <= endNumber)
                {
                    int newNumber = currentNumber + startOffset;
                    string newName = $"{newPrefix}{newNumber}";
                    string newPath = $"{Path.GetDirectoryName(path)}/{newName}.asset";

                    renameOperations.Add(new RenameOperation
                    {
                        OriginalPath = path,
                        OriginalName = fileName,
                        NewName = newName,
                        NewPath = newPath,
                        Number = currentNumber
                    });
                }
                else
                {
                    skipCount++;
                    Debug.Log($"跳过 {fileName} (数字不在范围 {startNumber}-{endNumber} 内)");
                }
            }
            else
            {
                skipCount++;
                Debug.Log($"跳过 {fileName} (格式不匹配)");
            }
        }

        // 按数字排序以确保顺序
        renameOperations = renameOperations.OrderBy(op => op.Number).ToList();

        // 执行重命名
        foreach (var operation in renameOperations)
        {
            // 检查目标文件是否已存在
            if (File.Exists(operation.NewPath) && operation.OriginalPath != operation.NewPath)
            {
                Debug.LogError($"无法重命名 {operation.OriginalName} 为 {operation.NewName}，目标文件已存在");
                errorCount++;
                continue;
            }

            string error = AssetDatabase.RenameAsset(operation.OriginalPath, operation.NewName);
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError($"重命名失败 {operation.OriginalName}: {error}");
                errorCount++;
            }
            else
            {
                Debug.Log($"重命名成功: {operation.OriginalName} 为 {operation.NewName}");
                successCount++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"重命名完成，成功: {successCount}, 跳过: {skipCount}, 失败: {errorCount}");

        if (successCount > 0)
        {
            EditorUtility.DisplayDialog("重命名完成",
                $"重命名操作已完成！\n成功: {successCount} 个文件\n跳过: {skipCount} 个文件\n失败: {errorCount} 个文件",
                "确定");
        }
    }

    /// <summary>
    /// 重命名操作的数据结构
    /// </summary>
    private struct RenameOperation
    {
        public string OriginalPath;
        public string OriginalName;
        public string NewName;
        public string NewPath;
        public int Number;
    }
}