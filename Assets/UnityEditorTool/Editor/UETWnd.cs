
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UET
{
    public class UETWnd : EditorWindow
    {
        readonly float MEUN_WIDTH = 0.2f;

        UETData mRoot;

        BaseTool mSelectedTool;

        public UETWnd()
        {
            titleContent = new GUIContent("工具总览");

            // 临时根目录
            var tempRoot = new UETTempData(null);

            Type toolType = typeof(BaseTool);
            var types = Assembly.GetAssembly(toolType).GetTypes();
            foreach (var type in types)
            {
                if (type.BaseType != toolType) continue;

                var attr = type.GetCustomAttribute<ToolAttribute>();
                if (attr == null) continue;

                // 去除第一个 "/"，保证路径一致性
                var path = attr.Path;
                if (path.StartsWith("/")) path = path.Substring(1);

                var root = tempRoot;
                var nameList = path.Split('/');
                int index = 0;

                // 遍历 (除了最后一个元素)
                for (int len = nameList.Length - 1; index < len; index++)
                {
                    var name = nameList[index];
                    if (!root.Children.ContainsKey(name))
                    {
                        root.Children[name] = new UETTempData(name);
                    }

                    if (root.Children[name].Children == null)
                    {
                        Debug.LogError($"[URTWnd] => Overlap Tool: {root.Children[name].Tool}");
                        root.Children[name] = new UETTempData(name);
                    }

                    root = root.Children[name];
                }

                // 处理最后一个元素 (即有 Tool 实例的元素)
                {
                    var name = nameList[index];
                    root.Children[name] = new UETTempData(attr.Index, name, Activator.CreateInstance(type) as BaseTool);
                }
            }

            // 通过 TempRoot 生成界面显示可用的 Root
            BuildRoot(tempRoot, out mRoot);
        }

        void OnEnable()
        {
            // 清空当前选中 Tool
            mSelectedTool = null;
        }

        void OnDisable()
        {
            // 清空当前选中 Tool
            mSelectedTool = null;
        }

        void OnGUI()
        {
            // 侧边栏
            var meunWidth = position.width * MEUN_WIDTH;
            GUILayout.BeginArea(new Rect(0, 0, meunWidth, position.height));
            ShowMenu(mRoot.Children);
            GUILayout.EndArea();

            // 选中内容
            GUILayout.BeginArea(new Rect(meunWidth, 0, position.width - meunWidth, position.height));
            ShowContent(mSelectedTool);
            GUILayout.EndArea();
        }

        // 显示左边的侧边栏
        void ShowMenu(List<UETData> toolList)
        {
            foreach (var tool in toolList)
            {
                // 显示父级功能按钮
                if (tool.Children != null)
                {
                    tool.Foldout = EditorGUILayout.BeginFoldoutHeaderGroup(tool.Foldout, new GUIContent(tool.Name));
                    if (tool.Foldout)
                    {
                        ShowMenu(tool.Children);
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();

                    if (tool.Tool != null)
                    {
                        Debug.LogError($"[URTWnd] => Overlap Tool: {tool.Tool}");
                    }
                    continue;
                }

                // 是个实例
                if (tool.Tool != null)
                {
                    if (GUILayout.Button(tool.Name))
                    {
                        mSelectedTool = tool.Tool;
                    }
                }
            }
        }

        // 显示右侧的内容栏
        void ShowContent(BaseTool tool)
        {
            tool?.Show();
        }

        // 通过 TempRoot 生成界面显示可用的 Root
        void BuildRoot(UETTempData tempRoot, out UETData root)
        {
            root = new UETData();

            // 优先设置实例
            root.Tool = tempRoot.Tool;
            root.Index = tempRoot.Index;
            root.Name = tempRoot.Name;

            // 递归子集
            var children = tempRoot.Children;
            if (children != null)
            {
                root.Children = new List<UETData>(children.Count);

                foreach (var pairChild in children)
                {
                    BuildRoot(pairChild.Value, out var child);
                    root.Children.Add(child);
                }

                // 对 root 的 children 排序
                root.Children.Sort((UETData child1, UETData child2) =>
                {
                    return child1.Index - child2.Index;
                });
            }
        }
    }

    class UETTempData
    {
        // Tool 的实例
        public BaseTool Tool { get; private set; }

        // 下标
        public int Index { get; private set; }

        // 名称
        public string Name { get; private set; }

        // name => UETTempData
        public Dictionary<string, UETTempData> Children { get; private set; }

        public UETTempData(string name)
        {
            Name = name;
            Index = ToolAttribute.DEFAULT_INDEX;
            Children = new Dictionary<string, UETTempData>();
        }

        public UETTempData(int index, string name, BaseTool tool)
        {
            Name = name;
            Index = index;
            Tool = tool;
        }
    }

    class UETData
    {
        // Tool 的名称
        public string Name { get; set; }

        public int Index { get; set; }

        // Tool 的实例
        public BaseTool Tool { get; set; }

        // Children
        public List<UETData> Children { get; set; }

        // 默认不展开
        public bool Foldout { get; set; } = false;
    }
}