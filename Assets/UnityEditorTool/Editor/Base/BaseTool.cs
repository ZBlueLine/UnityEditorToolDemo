
namespace UET
{
    // 工具基基类，负责左边的展示内容
    public abstract class BaseTool
    {
        // 显示工具
        protected abstract void OnShow();

        public void Show()
        {
            OnShow();
        }

        // 隐藏工具 (泛指切换工具)
        protected virtual void OnHide() { }

        // 关闭工具 (泛指关闭窗口)
        protected virtual void OnClose() { }
    }
}