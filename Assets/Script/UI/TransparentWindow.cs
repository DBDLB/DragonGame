#if UNITY_EDITOR || UNITY_STANDALONE_WIN  // 确保只在 Unity 编辑器或 Windows 平台上运行以下代码
using System;
using System.Collections;
using UnityEngine;
using System.Runtime.InteropServices;
using TMPro;


public class TransparentWindow : MonoBehaviour
{
    // 引入外部 Windows 函数，用于弹出消息框
    [DllImport("user32.dll")]
    public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
    
    // 定义一个结构体，MARGINS，用于表示窗口的边距
    private struct MARGINS
    {
        public int cxLeftWidth;   // 左边距宽度
        public int cxRightWidth;  // 右边距宽度
        public int cyTopHeight;   // 上边距高度
        public int cyBottomHeight; // 下边距高度
    }
    
    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left; //最左坐标
        public int Top; //最上坐标
        public int Right; //最右坐标
        public int Bottom; //最下坐标
    }

    /// 使用查找任务栏
    /// </summary>
    /// <param name="strClassName"></param>
    /// <param name="nptWindowName"></param>
    /// <returns></returns>
    [DllImport("user32.dll")]
    static extern IntPtr FindWindow(string strClassName, int nptWindowName);
    
    /// <summary>
    /// 获取窗口位置以及大小
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="lpRect"></param>
    /// <returns></returns>
    [DllImport("user32.dll")] static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

    // 引入 Windows API，获取当前活动窗口的句柄
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();
    
    // 引入 Windows API，扩展窗口的客户区（用于无边框窗口）
    [DllImport("Dwmapi.dll")]
    private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);
    
    // 引入 Windows API，用于修改窗口的样式
    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowLong(IntPtr hwnd, int index, uint dwNewLong);
    
    // 引入 Windows API，用于设置窗口位置、大小等
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    
    // 引入 Windows API，设置窗口的透明度和其他属性
    [DllImport("user32.dll")]
    static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
    
    // 定义一些常量，用于窗口样式和透明度设置
    const int GWL_EXSTYLE = -20;  // 获取或设置扩展窗口样式
    const uint WS_EX_LAYERED = 0x00080000;  // 层叠窗口样式，允许窗口透明
    const uint WS_EX_TRANSPARENT = 0x00000020;  // 透明窗口样式
    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);  // 设置窗口为最上层窗口
    const uint LWA_COLORKEY = 0x00000001;  // 设置颜色键透明，指定透明的颜色
    private IntPtr hwnd;  // 当前活动窗口的句柄
    
    /// <summary>
    /// 最终的屏幕的位置和长宽
    /// </summary>
    private Rect m_ScreenPosition;
    
    // Awake() 方法在场景启动时调用
    private void Awake()
    {
        // 在非 Unity 编辑器环境中执行以下操作
#if !UNITY_EDITOR
        // 获取当前活动窗口的句柄
        hwnd = GetActiveWindow();
        Rect workArea = new Rect(Screen.safeArea.x, Screen.safeArea.y, Screen.safeArea.width, Screen.safeArea.height);


        
        // 设置窗口的边距，负值表示扩展窗口的客户区
        MARGINS margins = new MARGINS() { cxLeftWidth = -1 };  // -1表示完全扩展至客户区
        DwmExtendFrameIntoClientArea(hwnd, ref margins);  // 扩展窗口的客户区，使窗口没有边框
        
        // 设置窗口的扩展样式，使其支持透明背景
        SetWindowLong(hwnd, GWL_EXSTYLE, WS_EX_LAYERED);
        
        // 设置窗口的透明度，使用颜色键来实现透明（此处是完全透明）
        SetLayeredWindowAttributes(hwnd, 0, 0, LWA_COLORKEY);  // 0 表示使用完全透明的颜色键
        
        // 设置窗口为最上层显示，设置窗口为最上层，位置为屏幕顶部，宽度与高度保持不变
         SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, (int)workArea.width, (int)workArea.height-GetTaskBarHeight(), 0);
#endif
        
        // 让应用程序在后台运行，确保即使窗口不在前台也能保持运行
        Application.runInBackground = true;
        
        Vector2 newPosition = this.GetComponent<RectTransform>().anchoredPosition;
        newPosition.y += GetTaskBarHeight();
        this.GetComponent<RectTransform>().anchoredPosition = newPosition;
    }
    
    
    /// <summary>
    /// 获取任务栏高度
    /// </summary>
    /// <returns>任务栏高度</returns>
    private int GetTaskBarHeight()
    {
        int taskbarHeight = 10;
        IntPtr hWnd = FindWindow("Shell_TrayWnd", 0);       //找到任务栏窗口
        RECT rect = new RECT();
        GetWindowRect(hWnd, ref rect);                      //获取任务栏的窗口位置及大小
        
        float screenHeight = Screen.currentResolution.height;
        float scaleFactorY = 1080f/screenHeight;
        taskbarHeight = (int)(rect.Bottom - rect.Top);      //得到任务栏的高度
        taskbarHeight = (int)(taskbarHeight * scaleFactorY);
        return taskbarHeight;
    }
}
#endif
