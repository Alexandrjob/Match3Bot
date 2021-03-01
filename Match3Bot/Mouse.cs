using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;

namespace Match3Bot
{
    public class Mouse
    {
        public int ScreenWight { get; set; }
        public int ScreenHeight { get; set; }
        public IntPtr Handle { get; set; }

        public Mouse(IntPtr handleGame)
        {
            Handle = handleGame;
            ScreenWight = SystemInformation.VirtualScreen.Width;
            ScreenHeight = SystemInformation.VirtualScreen.Height;
        }

        [DllImport("user32.dll")]
        public static extern long SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref Point point);

        [DllImport("user32.dll")]
        public static extern bool mouse_event(int dsFlags, int dx, int dy, int cButtons, int dsExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        public const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        public const int MOUSEEVENTF_RIGHTUD = 0x10;

        public void MoveShape(Point point1, Point point2)
        {
            DoMouseLeftClick(point1);
            DoMouseLeftClick(point2);
        }

        private void DoMouseLeftClick(Point point)
        {
            Move(point);
            mouse_event(MOUSEEVENTF_LEFTDOWN, point.X, point.Y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, point.X, point.Y, 0, 0);
        }
        public void Move(Point point)
        {
            ClientToScreen(Handle, ref point);
            SetCursorPos(point.X, point.Y);
        }
    }
}
