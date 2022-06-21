using UnityEditor;
using UnityEngine;

public class Es_WorkFlowGuidanceEditorWindowRes
{
    public Texture TitleImage = null;
    public Texture MarkdownFileImage = null;

    private const string TITLE_ICON = "Director_Icon";
    private const string MD_ICON = "Director_MultiGroupIcon";

    public void LoadTextures()
    {
        string suffix = EditorGUIUtility.isProSkin ? "_Light" : "_Dark";
        string missing = " is missing from Resources folder.";

        TitleImage = Resources.Load<Texture>(TITLE_ICON + suffix);
        if (TitleImage == null)
        {
            Debug.Log(TITLE_ICON + suffix + missing);
        }

        MarkdownFileImage = Resources.Load<Texture>(MD_ICON + suffix);
        if (MarkdownFileImage == null)
        {
            Debug.Log(MD_ICON + suffix + missing);
        }
    }
}
