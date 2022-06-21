using MG.MDV;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Es_WorkFlowGuidanceEditorWindow : EditorWindow
{
    public static readonly string PrefStartUp = "ResourceStandardLastSession";

    private bool mShow;
    private const float DOCSPANEL_WIDTH = 150f;
    private const float TOOLBAR_HEIGHT = 35f;

    private Es_WorkFlowGuidanceEditorWindowRes EditorWindowRes = null;

    private Es_WorkFlowGuidanceEditor_Panel mDocsPanel = null;

    [MenuItem("Window/项目工作流引导")]
    public static void Init()
    {
        Es_WorkFlowGuidanceEditorWindow window = (Es_WorkFlowGuidanceEditorWindow)EditorWindow.GetWindow(typeof(Es_WorkFlowGuidanceEditorWindow));
        window.position = new Rect(400, 100, 1000, 800);
        window.Show();
    }

    private void UpdateRequests()
    {
        MarkdownViewer viewer = mDocsPanel.GetCurrentViewer();
        if (viewer != null && viewer.Update())
            Repaint();
    }
    private void OnEnable()
    {
        //用于判断“下次不显示”是否已经被勾上
        mShow = EditorPrefs.GetBool(PrefStartUp, true);
        mDocsPanel = new Es_WorkFlowGuidanceEditor_Panel();
        EditorWindowRes = new Es_WorkFlowGuidanceEditorWindowRes();
        EditorWindowRes.LoadTextures();
        EditorApplication.update += UpdateRequests;
        base.titleContent = new GUIContent("项目工作流引导", EditorWindowRes.TitleImage);
    }
    private void OnGUI()
    {
        Rect docsPanelArea = new Rect(0, 0, DOCSPANEL_WIDTH, base.position.height);
        Rect toolBarArea = new Rect(DOCSPANEL_WIDTH, 0, base.position.width - DOCSPANEL_WIDTH, TOOLBAR_HEIGHT);
        Rect mainArea = new Rect(DOCSPANEL_WIDTH, TOOLBAR_HEIGHT, base.position.width - DOCSPANEL_WIDTH, base.position.height - TOOLBAR_HEIGHT);

        mDocsPanel.DrawPanel(docsPanelArea);
        mDocsPanel.DrawMainArea(toolBarArea, mainArea);

        GUILayout.BeginArea(toolBarArea);
        EditorGUI.BeginChangeCheck();
        bool doNotShow = !mShow;
        EditorGUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        mShow = !EditorGUILayout.ToggleLeft("下次不再显示", doNotShow, GUILayout.Width(100));
        EditorGUILayout.LabelField("可在\"Window/工程文档\"重新打开", GUILayout.Width(210));
        EditorGUILayout.Space(10);
        EditorGUILayout.EndHorizontal();
        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetBool(PrefStartUp, mShow);
        }
        GUILayout.EndArea();
        return;
    }
}
