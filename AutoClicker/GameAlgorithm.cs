using System;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Emgu;
using Emgu.Util;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Threading;
using System.Threading.Tasks; 

namespace AutoClicker
{
    class GameAlgorithm
    {
        private readonly string processName = "WindowsFormsAppPechenka";
        private readonly ApplicationForm _form;
        private Image<Bgr, byte> inputImage = null;
        private Image<Bgr, byte>[,] images = new Image<Bgr, byte>[8, 8];

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        public GameAlgorithm(ApplicationForm applicationForm)
        {
            _form = applicationForm;
        }

        public void StartAsync()
        {
            LoodingDefoultImage();

            Image<Gray, byte> grayImage = inputImage.SmoothGaussian(3).Convert<Gray, byte>().ThresholdBinaryInv(new Gray(230), new Gray(255));

            VectorOfVectorOfPoint coutours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();

            CvInvoke.FindContours(grayImage, coutours, hierarchy, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.LinkRuns);

            for (int i = 0; i < coutours.Size; i++)
            {
                double perimeter = CvInvoke.ArcLength(coutours[i], true);
                VectorOfPoint approximation = new VectorOfPoint();
                CvInvoke.ApproxPolyDP(coutours[i], approximation, 0.01 * perimeter, true);
                CvInvoke.DrawContours(grayImage, coutours, i, new MCvScalar(0, 0, 255), 1);

                Moments moments = CvInvoke.Moments(coutours[i]);

                int x = (int)(moments.M10 / moments.M00);
                int y = (int)(moments.M01 / moments.M00);

                if (approximation.Size == 3)
                {
                    CvInvoke.PutText(grayImage, "T", new Point(x, y), Emgu.CV.CvEnum.FontFace.HersheyPlain, 1, new MCvScalar(255, 255, 255), 1);
                }

                if (approximation.Size == 4)
                {
                    CvInvoke.PutText(grayImage, "K", new Point(x, y), Emgu.CV.CvEnum.FontFace.HersheyPlain, 1, new MCvScalar(255, 255, 255), 1);
                }

                if (approximation.Size == 12)
                {
                    CvInvoke.PutText(grayImage, "Kr", new Point(x, y), Emgu.CV.CvEnum.FontFace.HersheyPlain, 1, new MCvScalar(255, 255, 255), 1);
                }

                if (approximation.Size > 12)
                {
                    CvInvoke.PutText(grayImage, "Kg", new Point(x, y), Emgu.CV.CvEnum.FontFace.HersheyPlain, 1, new MCvScalar(255, 255, 255), 1);
                }
                
                _form.pictureBoxContoursImage.Image = grayImage.ToBitmap();
                _form.pictureBoxContoursImage.Refresh();
            }
        }

        private void LoodingDefoultImage()
        {
            var process = Process.GetProcessesByName(processName).FirstOrDefault();
            process.Refresh();
            
            if(process == null)
            {
                _form.labelStateProcces.Text = "Выбранный вами процесс не запущен или введен неверно.";
                _form.labelStateProcces.Refresh();
            }
            else
            {
                _form.labelStateProcces.Text = "Выбранный вами процесс запущен.";
                _form.labelStateProcces.Refresh();
            }
            var hwnd = process.MainWindowHandle;
            GetWindowRect(hwnd, out var rect);

            var image = new Bitmap(rect.Right - rect.Left, rect.Bottom - rect.Top);

            using (var graphics = Graphics.FromImage(image))
            {
                var hdcBitmap = graphics.GetHdc();
                PrintWindow(hwnd, hdcBitmap, 0);
                graphics.ReleaseHdc(hdcBitmap);
            }

            var rec = new Rectangle(10, 32, rect.Right - rect.Left - 105, rect.Bottom - rect.Top - 32);
            image = image.Clone(rec, image.PixelFormat);
            _form.pictureBoxDefoultImage.Image = image;
            _form.pictureBoxDefoultImage.Refresh();

            GetImagesShapes(image);

            inputImage = new Image<Bgr, byte>(image);
            _form.pictureBoxDefoultImage.Image = image;
            _form.pictureBoxDefoultImage.Refresh();
        }

        private void GetImagesShapes(Bitmap image)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var rec = new Rectangle(40 * j, 40 * i, 36, 38);
                    var a = image.Clone(rec, image.PixelFormat);
                    images[i, j] = new Image<Bgr, byte>(a);
                    _form.pictureBoxDefoultImage.Image = a;
                    _form.pictureBoxDefoultImage.Refresh();
                }
            }
        }
    }
}
