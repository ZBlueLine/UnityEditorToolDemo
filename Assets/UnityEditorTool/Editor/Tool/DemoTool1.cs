
using UnityEditor;

namespace UET
{
    [Tool("Demo/Demo1", 1)]
    public class DemoTool1 : BaseTool
    {
        protected override void OnShow()
        {
            EditorGUILayout.HelpBox("Demo1", MessageType.Info);
        }
    }
}