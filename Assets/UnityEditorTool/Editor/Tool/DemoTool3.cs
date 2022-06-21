
using UnityEditor;

namespace UET
{
    [Tool("Demo1/Demo3", 2)]
    public class DemoTool3 : BaseTool
    {

        protected override void OnShow()
        {
            EditorGUILayout.HelpBox("Demo3", MessageType.Info);
        }
    }
}