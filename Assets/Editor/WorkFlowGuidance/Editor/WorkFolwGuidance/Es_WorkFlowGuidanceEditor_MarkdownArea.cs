using MG.MDV;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class Es_FolderData
{
    public Es_FolderData(string folderName, List<string> docName, List<MarkdownViewer> markdownViewerList)
    {
        FolderName = folderName;
        mDocsName = docName;
        Viewers = markdownViewerList;
    }
    public string FolderName;
    public List<string> mDocsName;
    public List<MarkdownViewer> Viewers;
}
public class Es_WorkFlowGuidanceEditor_MarkdownArea
{
    public int CurrentPage { get => mCurrentPage; }
    public MarkdownViewer CurrentViewer { get => mDocfolder != null ? mDocfolder.Viewers[mCurrentPage] : null; }
    public List<Es_FolderData> FoldersData { get => mFoldersData; }

    private Es_WorkFlowGuidanceEditorWindowRes EditorWindowRes = null;
    private List<Es_FolderData> mFoldersData;
    private Es_FolderData mDocfolder;
    private TextAsset[] Markdowns;
    private GUISkin Skin;
    private const string markdownPath = "Assets/~Config/Docs/";

    private Vector2 mScrollPos;
    private List<string> mDocsName;
    private int mCurrentPage = 0;

    public Es_WorkFlowGuidanceEditor_MarkdownArea()
    {
        EditorWindowRes = new Es_WorkFlowGuidanceEditorWindowRes();
        EditorWindowRes.LoadTextures();
        LoadAllMarkdown();
    }
    private void LoadAllMarkdown()
    {
        if (Preferences.DarkSkin)
            Skin = Resources.Load<GUISkin>("MarkdownSkinQS");
        else
            Skin = Resources.Load<GUISkin>("MarkdownViewerSkin");
        if (!configExist(markdownPath))
        {
            CopyFilesFormDefaultFolder(markdownPath);
            AssetDatabase.Refresh();
        }
        string[] docRootPath = Directory.GetFiles(markdownPath, "*.md", SearchOption.AllDirectories);
        int len = docRootPath.Length;

        if (len == null)
        {
            Debug.LogError("没有Markdown文档");
            return;
        }
        else
        {
            if(Markdowns == null)
                Markdowns = new TextAsset[len];
            for (int i = 0; i < len; ++i)
                Markdowns[i] = AssetDatabase.LoadAssetAtPath<TextAsset>(docRootPath[i]);
            if(mFoldersData == null)
                mFoldersData = new List<Es_FolderData>();
            string preFolderName = "";
            int currentFolderIndex = -1;

            for (int i = 0; i < len; ++i)
            {
                string mdcontent = Markdowns[i].text;
                string mdName = Markdowns[i].name;
                string mdPath = AssetDatabase.GetAssetPath(Markdowns[i]);

                //获取该文档的分类
                string folderName = getFolderName(mdPath);
                Debug.Log(folderName);
                MarkdownViewer viewer = new MarkdownViewer(Skin, mdPath, mdcontent);
                if (preFolderName == "" || preFolderName != folderName)
                {
                    mFoldersData.Add(new Es_FolderData(folderName, new List<string>(), new List<MarkdownViewer>()));
                    mFoldersData[++currentFolderIndex].Viewers.Add(viewer);
                    mFoldersData[currentFolderIndex].mDocsName.Add(mdName);
                    preFolderName = folderName;
                }
                else
                {
                    mFoldersData[currentFolderIndex].Viewers.Add(viewer);
                    mFoldersData[currentFolderIndex].mDocsName.Add(mdName);
                }
            }
            SetCurrentDocFolder(0);
        }
    }
    public void DrawMarkdownArea(Rect toolBarArea, Rect markdownArea)
    {
        GUI.Box(toolBarArea, GUIContent.none, "RL Background");
        GUILayout.BeginArea(toolBarArea);

        EditorGUILayout.BeginHorizontal();
        GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
        style.fontSize = 15;
        style.normal.background = (Texture2D)EditorWindowRes.MarkdownFileImage;
        style.border = new RectOffset(5, 5, 5, 5);
        EditorGUI.LabelField(new Rect(15, 7, 18, 18), "", style);

        style = new GUIStyle(EditorStyles.popup);
        style.fontSize = 15;
        if (mCurrentPage >= mDocsName.Count)
            mCurrentPage = 0;
        mCurrentPage = EditorGUI.Popup(new Rect(40, 7, 150, 20), mCurrentPage, mDocsName.ToArray(), style);

        EditorGUILayout.EndHorizontal();
        GUILayout.EndArea();

        GUILayout.BeginArea(markdownArea);
        if (mDocfolder != null && mDocfolder.Viewers[mCurrentPage] != null)
        if (mDocfolder != null)
        {
            mScrollPos = EditorGUILayout.BeginScrollView(mScrollPos, GUILayout.Width(markdownArea.width), GUILayout.Height(markdownArea.height - 30));
            mDocfolder.Viewers[mCurrentPage].Draw();
        //还原GUI的默认skin，否则会将md的skin应用在普通GUI上
            GUI.skin = null;
            EditorGUILayout.EndScrollView();
        }
        GUILayout.EndArea();
    }

    public void SetCurrentDocFolder(int currentDocFolder)
    {
        mCurrentPage = 0;
        mDocsName = new List<string>();
        mDocfolder = mFoldersData[currentDocFolder];
        foreach(string name in mDocfolder.mDocsName)
            mDocsName.Add(getDocName(name));
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
    private string getDocName(string docName)
    {
        string name;
            string[] splitname = docName.Split('.');
        if (splitname.Length > 1)
            name = splitname[1];
        else
            name = splitname[0];
        return name;
    }
    private bool configExist(string targetPath)
    {
        if (!Directory.Exists(targetPath))
            return false;
        return true;
    }

    private string generateResourcePath(string filePath)
    {
        List<string> resourcePath = new List<string>();
        string[] splitPath = filePath.Split('/');
        int len = splitPath.Length;
        for(int i = 0; i < len - 1; ++i)
        {
            resourcePath.Add(splitPath[i]);
        }
        var result = String.Join("/", resourcePath.ToArray())+"/DefaultDocs/";
        return result;
    }
    private void CopyFilesFormDefaultFolder(string targetPath)
    {
        TextAsset templateMD = Resources.Load<TextAsset>("MarkDownTemplate");
        string sourcePath = generateResourcePath(AssetDatabase.GetAssetPath(templateMD));
        Debug.Log(sourcePath);
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Debug.Log(dirPath);
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }

        //Copy all the files & Replaces any files with the same name
        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
    }
}
