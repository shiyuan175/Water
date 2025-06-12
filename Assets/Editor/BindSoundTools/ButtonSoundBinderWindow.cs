using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEditor.Events;
using System;

public class ButtonSoundBinderWindow : EditorWindow
{
    private static List<MethodInfo> methods;
    private static GameObject selectedPrefab;
    private static GameObject prefabRoot;
    private static List<Button> buttons;
    private static int[] selectedMethodIndexes;

    public static void ShowWindow()
    {
        if (!(Selection.activeObject is GameObject prefab))
        {
            Debug.LogWarning("��ѡ��һ��Prefab");
            return;
        }

        selectedPrefab = prefab;
        methods = ButtonSoundBinder.GetCachedMethods();
        if (methods == null || methods.Count == 0)
        {
            Debug.LogWarning("δ�ҵ���̬�޲η���");
            return;
        }

        string prefabPath = AssetDatabase.GetAssetPath(prefab);
        prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
        buttons = new List<Button>(prefabRoot.GetComponentsInChildren<Button>(true));
        selectedMethodIndexes = new int[buttons.Count]; // Ĭ�϶�Ϊ0

        GetWindow<ButtonSoundBinderWindow>("��ť��Ч�󶨹���").Show();
    }

    private void OnGUI()
    {
        if (buttons == null || buttons.Count == 0)
        {
            EditorGUILayout.LabelField("δ�ҵ��κ� Button");
            return;
        }

        EditorGUILayout.LabelField("Ϊÿ����ťѡ��һ����������Ч��", EditorStyles.boldLabel);

        for (int i = 0; i < buttons.Count; i++)
        {
            var btn = buttons[i];
            if (btn == null) continue;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{btn.name}", GUILayout.Width(200));
            List<string> methodNames = methods.ConvertAll(m => m.Name);
            methodNames.Add("<Null>");
            selectedMethodIndexes[i] = EditorGUILayout.Popup(selectedMethodIndexes[i], methodNames.ToArray());

            if (GUILayout.Button("��", GUILayout.Width(80)))
            {
                BindMethodToButton(btn, selectedMethodIndexes[i]);

                SavePrefabOnly();
            }

            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(20);

        if (GUILayout.Button("���������а�ť"))
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                BindMethodToButton(buttons[i], selectedMethodIndexes[i]);
            }

            SavePrefabOnly();
        }
    }

    private void BindMethodToButton(Button button, int methodIndex)
    {
        if (button == null) return;

        // �Ƴ�ͬ�෽��
        int count = button.onClick.GetPersistentEventCount();
        for (int i = count - 1; i >= 0; i--)
        {
            var m = button.onClick.GetPersistentMethodName(i);
            var target = button.onClick.GetPersistentTarget(i);
            if (target == null && methods.Exists(c => c.Name == m))
            {
                UnityEventTools.RemovePersistentListener(button.onClick, i);
            }
        }

        if (methodIndex >= methods.Count)
        {
            Debug.Log($"��ť {button.name} δ���κη���");
            return;
        }

        var method = methods[methodIndex];
        if (method == null) return;

        // ���
        UnityEventTools.AddPersistentListener(button.onClick, Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction), method) as UnityEngine.Events.UnityAction);
        Debug.Log($"�Ѱ󶨷��� {method.Name} ����ť {button.name}");
    }

    private void OnDestroy()
    {
        SaveAndCleanup();
    }

    private void SavePrefabOnly()
    {
        if (prefabRoot != null && selectedPrefab != null)
        {
            string path = AssetDatabase.GetAssetPath(selectedPrefab);
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
            Debug.Log("Prefab �������");
        }
    }

    private void SaveAndCleanup()
    {
        if (prefabRoot != null && selectedPrefab != null)
        {
            //string path = AssetDatabase.GetAssetPath(selectedPrefab);
            //PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
            PrefabUtility.UnloadPrefabContents(prefabRoot);
            Debug.Log("Prefab �ͷ���Դ");
        }
    }
}

