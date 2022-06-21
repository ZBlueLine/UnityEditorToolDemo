using MG.MDV;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Es_WorkFlowGuidanceEditor_Panel
{
    public List<Es_FolderData> FoldersData { get => mFoldersData; }
    public int CurrentFolder { get => mCurrentFolder; }


    private Es_WorkFlowGuidanceEditorWindowRes EditorWindowRes = null;

    private Es_WorkFlowGuidanceEditor_MarkdownArea mMDArea = null;
    private Es_ResourcesCheckTools mCheckToolArea = null;
    private List<Es_FolderData> mFoldersData;
    private int mCurrentFolder = 0;
    private bool mShowDocFolders = true;
    private bool mShowCheckTool = true;
    private enum mMainAreaType
    {
        ShowDoc,
        ShowModelCheckTool,
        ShowParticleCheckTool
    }
    private mMainAreaType mMainArea = mMainAreaType.ShowDoc;
    public Es_WorkFlowGuidanceEditor_Panel()
    {
        mMDArea = new Es_WorkFlowGuidanceEditor_MarkdownArea();
        mCheckToolArea = new Es_ResourcesCheckTools();
        mFoldersData = mMDArea.FoldersData;
        EditorWindowRes = new Es_WorkFlowGuidanceEditorWindowRes();

        EditorWindowRes.LoadTextures();
    }
    public void DrawPanel(Rect docsPanelArea)
    {
        GUI.Box(docsPanelArea, GUIContent.none, "RL Background");
        GUILayout.BeginArea(docsPanelArea);

        mShowDocFolders = EditorGUILayout.BeginFoldoutHeaderGroup(mShowDocFolders, new GUIContent("工程文档"));
        if (mShowDocFolders)
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 12;
            buttonStyle.fixedWidth = 125;
            buttonStyle.fixedHeight = 20;
            int len = mFoldersData.Count;
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space(5);
            for (int i = 0; i < len; ++i)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button(mFoldersData[i].FolderName, buttonStyle))
                {
                    mCurrentFolder = i;
                    mMainArea = mMainAreaType.ShowDoc;
                    mMDArea.SetCurrentDocFolder(mCurrentFolder);
                }
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space(10);
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        mShowCheckTool = EditorGUILayout.BeginFoldoutHeaderGroup(mShowCheckTool, new GUIContent("资源检查工具"));
        if (mShowCheckTool)
        {
            EditorGUILayout.Space(5);
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 12;
            buttonStyle.fixedWidth = 125;
            buttonStyle.fixedHeight = 20;
            int len = mFoldersData.Count;

            EditorGUILayout.Space(10);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("模型检查工具", buttonStyle))
            {
                mMainArea = mMainAreaType.ShowModelCheckTool;
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("特效检查工具", buttonStyle))
            {
                mMainArea = mMainAreaType.ShowParticleCheckTool;
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        GUILayout.EndArea();
    }

    public void DrawMainArea(Rect toolBarArea, Rect mainArea)
    {
        if (mMainArea == mMainAreaType.ShowDoc)
        {
            mMDArea.DrawMarkdownArea(toolBarArea, mainArea);
        }
        else if (mMainArea == mMainAreaType.ShowModelCheckTool)
        {
            mCheckToolArea.DrawModelCheckTool(toolBarArea, mainArea);
            return;
        }
        else if (mMainArea == mMainAreaType.ShowParticleCheckTool)
        {
            mCheckToolArea.DrawParticleCheckTool(toolBarArea, mainArea);
            return;
        }
    }
    public MarkdownViewer GetCurrentViewer()
    {
        return mMDArea.CurrentViewer;
    }
}
