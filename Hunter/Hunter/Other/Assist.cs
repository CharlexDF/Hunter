using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace Hunter
{
    public class Assist
    {
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("User32.dll", EntryPoint = "FindWindowEx")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public extern static int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "WindowFromPoint")]//指定坐标处窗体句柄
        public static extern int WindowFromPoint(int xPoint, int yPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int X;
            public int Y;
            public Point(int x, int y) { X = x; Y = y; }
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point pt);

        [DllImport("user32.dll")]
        public static extern void SetCursorPos(int x, int y);
        /*
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("User32.dll", EntryPoint = "FindWindowEx")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);
        */

        public static void Test()
        {
            IntPtr hwnd = FindWindow(null, "大智慧");
            if (hwnd == IntPtr.Zero)
            {
                Utility.Log("FindWindow fail...");
                return;
            }
            Utility.Log("hwnd = " + hwnd.ToString());




            /*
            while (true)
            {
                Thread.Sleep(1000);

                Point pt = new Point(0, 0);
                GetCursorPos(out pt);
                
                Utility.Log(" ");
                //Utility.Log("X = " + pt.X + " Y = " + pt.Y);
                
                int ihwnd = WindowFromPoint(pt.X, pt.Y);

                StringBuilder sb = new StringBuilder(512);
                int i = GetWindowText(hwnd, sb, sb.Capacity);
                
                //Utility.Log("text = " + sb.ToString());
            }
            */
            
        }
    }
}
