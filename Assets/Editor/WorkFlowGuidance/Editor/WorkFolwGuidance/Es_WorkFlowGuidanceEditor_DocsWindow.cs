using MG.MDV;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Es_WorkFlowGuidanceEditor_DocsWindow
{
    public List<Es_FolderData> FoldersData { get => mFoldersData; }
    public int CurrentFolder { get => mCurrentFolder; }


    private TextAsset[] Markdowns;
    private List<Es_FolderData> mFoldersData;
    private GUISkin Skin;
    private int mCurrentFolder = 0;
    public void DrawDocsWindow(Rect docsPanelArea)
    {
        GUI.Box(docsPanelArea, GUIContent.none, "RL Background");
        GUILayout.BeginArea(docsPanelArea);
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 12;
        buttonStyle.fixedWidth = 125;
        buttonStyle.fixedHeight = 20;
        int len = mFoldersData.Count;

        string[] name = new string[len];

        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space(10);
        for (int i = 0; i < len; ++i)
            name[i] = mFoldersData[i].FolderName;
        EditorGUI.BeginChangeCheck();
        mCurrentFolder = EditorGUILayout.Popup(mCurrentFolder, name);
        if(EditorGUI.EndChangeCheck())
        {

        }
        GUILayout.EndArea();
    }

    public Es_FolderData GetCurrentFolder()
    {
        return mFoldersData[mCurrentFolder];
    }
    private string getFolderName(string path)
    {
        string folderName;
        string[] folderPath = path.Split('/');
        string[] folderNames = folderPath[folderPath.Length - 2].Split('.');
        if (folderNames.Length > 1)
            folderName = folderNames[1];
        else
            folderName = folderNames[0];
        return folderName;
    }
}
