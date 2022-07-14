using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SfxEditor : EditorWindow
{
    private Vector2 scrollPos;
    private SfxContainer selected;
    private SerializedProperty projectile_Prop;

    private SfxContainer[] GetAllSfx()
    {
        string[] guids = AssetDatabase.FindAssets("t:"+ typeof(SfxContainer).Name);
        SfxContainer[] a = new SfxContainer[guids.Length];
        for(int i =0; i<guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath<SfxContainer>(path);
        }
 
        return a;
    }

    [MenuItem("Window/Sfx Editor")]
    public static void ShowWindow () {
        GetWindow<SfxEditor>().position = new Rect(150, 150, 250, 500);
    }

    void OnGUI() {
        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.ExpandHeight(true));
        
        if (selected != null) {
            EditorApplication.ExecuteMenuItem("Window/General/Inspector");
            AssetDatabase.OpenAsset(selected);
        }
        else {
            EditorApplication.ExecuteMenuItem("Window/General/Inspector");
        }

        foreach (SfxContainer container in GetAllSfx()) {
            if (container == selected) {
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                if (!GUILayout.Toggle(true, container.name, EditorStyles.toolbarButton)) {
                    selected = null;
                }
            }
            else {
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                if (GUILayout.Toggle(false, container.name, EditorStyles.toolbarButton)) {
                    selected = container;
                }
            }
        }

        EditorGUILayout.EndScrollView();
    }
}
