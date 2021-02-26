using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace Match3Bot
{
    class WindowGame
    {
        private readonly string processName = "WindowsFormsAppPechenka";
        private readonly Process _process;
        private readonly IntPtr _handleGame;

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        public WindowGame()
        {
            _process = Process.GetProcessesByName(processName).FirstOrDefault();
            if (!ProcessValid())
            {
                throw new InvalidOperationException("Процесс не запущен");
            }
            _handleGame = _process.MainWindowHandle;
        }

        public bool ProcessValid()
        {
            if (_process == null)
            {
                return false;
            }
            return true;
        }

        public IntPtr GetHandleGame()
        {
            return _handleGame;
        }

        public Bitmap GetImage(out RECT rect)
        {
            _process.Refresh();
            var hwnd = _process.MainWindowHandle;
            GetWindowRect(hwnd, out rect);

            var image = new Bitmap(rect.Right - rect.Left, rect.Bottom - rect.Top);

            using (var graphics = Graphics.FromImage(image))
            {
                var hdcBitmap = graphics.GetHdc();
                PrintWindow(hwnd, hdcBitmap, 2);
                graphics.ReleaseHdc(hdcBitmap);
            }

            return image;
        }
    }
}