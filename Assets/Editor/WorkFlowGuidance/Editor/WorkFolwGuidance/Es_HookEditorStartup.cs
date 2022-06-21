using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class Es_HookEditorStartup
{

    static Es_HookEditorStartup()
    {
        EditorApplication.update += Update;
    }

    private static void Update()
    {
        EditorApplication.update -= Update;

        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            bool show = false;
            if (!EditorPrefs.HasKey(Es_WorkFlowGuidanceEditorWindow.PrefStartUp))
            {
                show = true;
                EditorPrefs.SetBool(Es_WorkFlowGuidanceEditorWindow.PrefStartUp, show);
            }
            else
            {
                if (Time.realtimeSinceStartup < 10)
                {
                    show = EditorPrefs.GetBool(Es_WorkFlowGuidanceEditorWindow.PrefStartUp, true);
                }
            }
            if (show == true)
                Es_WorkFlowGuidanceEditorWindow.Init();
        }
    }

}