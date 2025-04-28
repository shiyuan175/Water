using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FindUnusedAnimationsWindow : EditorWindow
{
    private List<string> unusedClips = new List<string>();
    private List<string> unusedControllers = new List<string>();
    private Vector2 scrollPos;

    [MenuItem("Tools/Find Unused Animations")]
    public static void ShowWindow()
    {
        GetWindow<FindUnusedAnimationsWindow>("Find Unused Animations");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("��ʼɨ��"))
        {
            ScanUnusedAnimations();
        }

        GUILayout.Space(10);

        if (unusedClips.Count == 0 && unusedControllers.Count == 0)
        {
            GUILayout.Label("û��δʹ�õĶ���Ƭ�λ�Animator��������");
            return;
        }

        // �¼ӵ�ɾ����ť
        if (GUILayout.Button("һ��ɾ��δʹ�õĶ�����Animator"))
        {
            if (EditorUtility.DisplayDialog("ȷ��ɾ��", "ȷ��Ҫɾ������δʹ�õ� AnimationClip �� AnimatorController �𣿴˲������ɳ��أ�", "ɾ��", "ȡ��"))
            {
                DeleteUnusedAssets();
            }
        }

        GUILayout.Space(10);

        scrollPos = GUILayout.BeginScrollView(scrollPos);

        GUIStyle redStyle = new GUIStyle(GUI.skin.button);
        redStyle.normal.textColor = Color.red;

        GUILayout.Label("δʹ�õ� AnimationClips:", EditorStyles.boldLabel);
        foreach (var path in unusedClips)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(System.IO.Path.GetFileName(path), redStyle))
            {
                var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                Selection.activeObject = asset;
                EditorGUIUtility.PingObject(asset);
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);

        GUILayout.Label("δʹ�õ� AnimatorControllers:", EditorStyles.boldLabel);
        foreach (var path in unusedControllers)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(System.IO.Path.GetFileName(path), redStyle))
            {
                var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                Selection.activeObject = asset;
                EditorGUIUtility.PingObject(asset);
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }

    private void ScanUnusedAnimations()
    {
        unusedClips.Clear();
        unusedControllers.Clear();

        var clipGUIDs = AssetDatabase.FindAssets("t:AnimationClip");
        var controllerGUIDs = AssetDatabase.FindAssets("t:AnimatorController");

        HashSet<string> allDependencies = new HashSet<string>();
        var allAssetGUIDs = AssetDatabase.FindAssets("t:Prefab t:Scene t:ScriptableObject");

        foreach (var guid in allAssetGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string[] dependencies = AssetDatabase.GetDependencies(path, true);
            foreach (var dep in dependencies)
            {
                allDependencies.Add(dep);
            }
        }

        foreach (var guid in clipGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!allDependencies.Contains(path))
            {
                unusedClips.Add(path);
            }
        }

        foreach (var guid in controllerGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!allDependencies.Contains(path))
            {
                unusedControllers.Add(path);
            }
        }

        Debug.Log($"ɨ����ɣ��ҵ� {unusedClips.Count} ��δʹ��AnimationClip��{unusedControllers.Count} ��δʹ��AnimatorController");
    }

    private void DeleteUnusedAssets()
    {
        foreach (var path in unusedClips)
        {
            AssetDatabase.DeleteAsset(path);
        }
        foreach (var path in unusedControllers)
        {
            AssetDatabase.DeleteAsset(path);
        }

        AssetDatabase.Refresh();
        Debug.Log("ɾ����ɣ���Դ��ˢ�£�");

        // ����б�
        unusedClips.Clear();
        unusedControllers.Clear();
    }
}
