using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;

namespace AutoClicker
{
    class Mouse
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

        public void Move(int x, int y)
        {
            Point point = new Point(x, y);

            ClientToScreen(Handle, ref point);
            SetCursorPos(point.X, point.Y);
        }

        [DllImport("user32.dll")]
        public static extern long SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref Point point);
    }
}
