using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class Es_ResourcesCheckTools
{
    private static GUIStyle style;
    //private int mToolbarInt = 0;
    //private string[] mToolbarStrings = { "模型面数检查", "特效粒子数检查" };
    private int mCurrentPage = 1;
    private bool mClickedSearchButton = false;

    private int mMeshTris = 10000;
    private int mMaxParticles = 1000;
    private bool mCheckSingleTextureResolution = false;
    private bool mCheckTexturesResolution = false;
    private bool mCheckParticleCount = true;
    private int mMaxTexResolution = 256;
    private string mPath = "Assets/";
    private List<Es_ModelWarningInfo> mModelInfoList;
    private List<Es_ParticleWarningInfo> mParticleInfoList;
    public void DrawModelCheckTool(Rect toolBarArea, Rect checkToolArea)
    {
        GUI.Box(toolBarArea, GUIContent.none, "RL Background");
        GUILayout.BeginArea(toolBarArea);

        EditorGUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        style = new GUIStyle(EditorStyles.boldLabel);
        style.fontSize = 15;
        EditorGUILayout.LabelField("模型面数检查工具", style, GUILayout.Width(300));
        EditorGUILayout.EndHorizontal();
        GUILayout.EndArea();

        GUI.Box(checkToolArea, GUIContent.none, "RL Background");
        GUILayout.BeginArea(checkToolArea);
        modelCheck();
        GUILayout.EndArea();
    }

    public void DrawParticleCheckTool(Rect toolBarArea, Rect checkToolArea)
    {
        GUI.Box(toolBarArea, GUIContent.none, "RL Background");
        GUILayout.BeginArea(toolBarArea);

        EditorGUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        style = new GUIStyle(EditorStyles.boldLabel);
        style.fontSize = 15;
        EditorGUILayout.LabelField("特效粒子数检查工具", style, GUILayout.Width(300));
        EditorGUILayout.EndHorizontal();
        GUILayout.EndArea();

        GUI.Box(checkToolArea, GUIContent.none, "RL Background");
        GUILayout.BeginArea(checkToolArea);
        particleCheck();
        GUILayout.EndArea();
    }
    private void modelCheck()
    {
        //EditorGUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        mMeshTris = EditorGUILayout.IntField("面数阈值", mMeshTris, GUILayout.Width(300));
        EditorGUILayout.EndHorizontal();

        Rect rect = EditorGUILayout.GetControlRect();
        EditorGUILayout.BeginHorizontal();
        style.normal.textColor = Color.red;
        rect.width = 300;
        mPath = EditorGUI.TextField(rect, "搜索文件夹", mPath);
        EditorGUILayout.LabelField("注意不要选择包含大量文件的根目录", style);
        EditorGUILayout.EndHorizontal();

        //检测鼠标拖入文件夹
        if ((Event.current.type == EventType.DragUpdated ||
            Event.current.type == EventType.DragExited) &&
            rect.Contains(Event.current.mousePosition))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
            {
                mPath = DragAndDrop.paths[0];
            }
        }

        EditorGUILayout.Space(30);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(50);
        if (GUILayout.Button("搜索", GUILayout.Width(50), GUILayout.Height(50)))
        {
            clear();
            mClickedSearchButton = true;
            Es_RseourcesCheckUtility.checkModelAboveThreshold(mPath, mMeshTris, out mModelInfoList);
        }
        EditorGUILayout.EndHorizontal();

        style.normal.textColor = Color.green;
        style.fontSize = 15;
        if (mModelInfoList == null || mModelInfoList.Count == 0)
        {
            if (mClickedSearchButton)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("未搜索到超出面数阈值的模型", style);
                EditorGUILayout.EndHorizontal();
            }
            return;
        }

        //每页18项，列出搜索结果
        int len = mModelInfoList.Count;
        int itemsPerPage = 18;
        style.normal.textColor = new Color(0.8f, 0, 0, 1);
        style.fontSize = 15;
        for (int i = (mCurrentPage - 1) * itemsPerPage; i < len && i < mCurrentPage * itemsPerPage; ++i)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(mModelInfoList[i].Model, typeof(GameObject), true, GUILayout.Width(400));
            EditorGUILayout.LabelField("[" + mModelInfoList[i].AllModelTrisCount.ToString() + "]", style);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        //计算总页数
        int pages = mModelInfoList.Count / itemsPerPage + ((mModelInfoList.Count % itemsPerPage) == 0 ? 0 : 1);
        pageReference(pages, ref mCurrentPage);
    }

    private void particleCheck()
    {
        EditorGUILayout.Space(10);
        if (mCheckParticleCount)
        {
            EditorGUILayout.BeginHorizontal();
            mMaxParticles = EditorGUILayout.IntField("粒子数阈值", mMaxParticles, GUILayout.Width(300));
            EditorGUILayout.EndHorizontal();
        }

        if (mCheckTexturesResolution)
        {
            EditorGUILayout.BeginHorizontal();
            mMaxTexResolution = Mathf.Min(4096, EditorGUILayout.IntField("贴图分辨率阈值", mMaxTexResolution, GUILayout.Width(300)));
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.EndHorizontal();

        Rect rect = EditorGUILayout.GetControlRect(true);

        EditorGUILayout.BeginHorizontal();
        rect.width = 300;
        mPath = EditorGUI.TextField(rect, "搜索文件夹", mPath);
        style.normal.textColor = Color.red;
        EditorGUILayout.LabelField("注意不要选择包含大量文件的根目录", style);
        EditorGUILayout.EndHorizontal();

        if ((Event.current.type == EventType.DragUpdated ||
            Event.current.type == EventType.DragExited) &&
            rect.Contains(Event.current.mousePosition))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
            {
                mPath = DragAndDrop.paths[0];
            }
        }

        EditorGUILayout.Space(20);

        EditorGUILayout.BeginHorizontal();
        mCheckParticleCount = EditorGUILayout.ToggleLeft("检查粒子数", mCheckParticleCount);

        EditorGUI.BeginChangeCheck();
        mCheckTexturesResolution = EditorGUILayout.ToggleLeft("检查材质贴图分辨率总和", mCheckTexturesResolution);
        if (!mCheckTexturesResolution) mCheckSingleTextureResolution = false;
        mCheckSingleTextureResolution = EditorGUILayout.ToggleLeft("检查单张贴图分辨率", mCheckSingleTextureResolution);
        if (mCheckSingleTextureResolution) mCheckTexturesResolution = true;

        if (EditorGUI.EndChangeCheck())
        {
            //设置新搜索模式以后清空搜索结果列表
            clear();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(50);
        if (GUILayout.Button("搜索", GUILayout.Width(50), GUILayout.Height(50)))
        {
            clear();
            mClickedSearchButton = true;
            Es_RseourcesCheckUtility.checkParticleAboveThreshold(mCheckParticleCount, mCheckTexturesResolution, mCheckSingleTextureResolution, mPath, mMaxParticles, mMaxTexResolution, out mParticleInfoList);
        }
        EditorGUILayout.EndHorizontal();

        style.normal.textColor = Color.green;
        style.fontSize = 15;
        if (mParticleInfoList == null || mParticleInfoList.Count == 0)
        {
            if (mClickedSearchButton)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("未搜索到超出规定阈值的特效", style);
                EditorGUILayout.EndHorizontal();
            }
            return;
        }

        int len = mParticleInfoList.Count;
        //每页的项数
        int itemsPerPage = 5;
        EditorGUILayout.BeginVertical(GUILayout.Height(500));
        for (int i = (mCurrentPage - 1) * itemsPerPage; i < len && i < mCurrentPage * itemsPerPage; ++i)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField("特效预制体：", mParticleInfoList[i].RootResourceGameObject, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
            style.normal.textColor = new Color(0.8f, 0, 0, 1);
            style.fontSize = 15;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.ObjectField("Child Object：", mParticleInfoList[i].ResourceGameObject, typeof(GameObject), true, GUILayout.Width(400));
            EditorGUILayout.Space(10);
            if (mParticleInfoList[i].MaxParticlesWarning)
            {
                EditorGUILayout.LabelField("粒子数[" + mParticleInfoList[i].ParticleSystem.main.maxParticles.ToString() + "]", style);
            }
            else
            {
                style.normal.textColor = Color.green;
                EditorGUILayout.LabelField("粒子数未超阈值", style);
                style.normal.textColor = new Color(0.8f, 0, 0, 1);
            }
            EditorGUILayout.EndHorizontal();
            if (mCheckSingleTextureResolution && mParticleInfoList[i].TextureSizeWarning)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space(15);
                EditorGUILayout.ObjectField("Texture：", mParticleInfoList[i].ParticleTexture, typeof(GameObject), true, GUILayout.Width(400));
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("分辨率[" + mParticleInfoList[i].ParticleTexture.width.ToString() + "] " + "[" + mParticleInfoList[i].ParticleTexture.height.ToString() + "]", style);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (!mCheckSingleTextureResolution && mParticleInfoList[i].TextureSizeWarning)
                EditorGUILayout.LabelField("材质所有贴图合计分辨率超出阈值", style);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(20);
        }
        EditorGUILayout.EndHorizontal();

        int pages = mParticleInfoList.Count / itemsPerPage + ((mParticleInfoList.Count % itemsPerPage) == 0 ? 0 : 1);
        pageReference(pages, ref mCurrentPage);
    }
    private void pageReference(int totalPageNumbers, ref int mCurrentPage)
    {
        EditorGUILayout.BeginHorizontal();

        if (mCurrentPage > 1)
            GUI.enabled = true;
        else
            GUI.enabled = false;

        if (GUILayout.Button("上一页", GUILayout.Width(50), GUILayout.Height(50)))
            --mCurrentPage;
        GUI.enabled = true;

        EditorGUILayout.Space(150);
        EditorGUILayout.LabelField(mCurrentPage + "/" + totalPageNumbers);

        if (mCurrentPage < totalPageNumbers)
            GUI.enabled = true;
        else
            GUI.enabled = false;

        if (GUILayout.Button("下一页", GUILayout.Width(50), GUILayout.Height(50)))
            ++mCurrentPage;

        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
    }
    private void clear()
    {
        mClickedSearchButton = false;
        mCurrentPage = 1;
        if (mModelInfoList != null)
            mModelInfoList.Clear();
        if (mParticleInfoList != null)
            mParticleInfoList.Clear();
        return;
    }
}