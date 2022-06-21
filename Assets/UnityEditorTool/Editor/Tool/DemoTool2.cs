
using UnityEditor;

namespace UET
{
    [Tool("Demo/Demo2", 2)]
    public class DemoTool2 : BaseTool
    {

        protected override void OnShow()
        {
            EditorGUILayout.HelpBox("Demo2", MessageType.Info);
        }
    }
}