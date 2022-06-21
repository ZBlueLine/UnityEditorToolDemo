
using UnityEditor;
using UnityEngine;

namespace UET
{
    public class UnityEditorTool : Editor
    {
        static readonly Rect NORMAL_SIZE = new Rect(400, 100, 1000, 800);

        [MenuItem("UET/Overall")]
        public static void Overall()
        {
            UETWnd window = (UETWnd)EditorWindow.GetWindow(typeof(UETWnd));
            window.position = NORMAL_SIZE;
            window.Show();
        }


    }
}
