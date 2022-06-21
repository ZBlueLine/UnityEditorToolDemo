
using System;

namespace UET
{
    public class ToolAttribute : Attribute
    {
        public const int DEFAULT_INDEX = -1;

        public string Path { get; private set; }

        public int Index { get; private set; }

        public ToolAttribute(string path, int index = DEFAULT_INDEX)
        {
            Path = path;
            Index = index;
        }
    }
}