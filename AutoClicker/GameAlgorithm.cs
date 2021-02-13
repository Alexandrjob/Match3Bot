using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace AutoClicker
{
    class GameAlgorithm
    {
        private readonly string processName = "WindowsFormsAppPechenka";
        private readonly ApplicationForm _form;
        private Process _process;

        private readonly int _indentLength = 40;
        private readonly int _imageWidthOneGameCell = 38;
        private readonly int _imageHeightOneGameCell = 38;

        private readonly Image<Bgr, byte>[,] images = new Image<Bgr, byte>[8, 8];
        private int[,] fruits = new int[8, 8];

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
            _process = Process.GetProcessesByName(processName).FirstOrDefault();

            if (_process == null)
            {
                _form.labelStateProcces.Text = "Выбранный вами процесс не запущен или введен неверно.";
                _form.labelStateProcces.Refresh();
                return;
            }
            else
            {
                _process.Refresh();
                _form.labelStateProcces.Text = "Выбранный вами процесс запущен.";
                _form.labelStateProcces.Refresh();
            }

            LoodingDefoultImage();

            //Image<Gray, byte> linkedImage = new Image<Gray, byte>("defoult");
            //Image<Gray, byte> a = new Image<Gray, byte>("defoult");

            //for (int i = 0; i < 8; i++)
            //{
            //    for (int j = 0; j < 8; j++)
            //    {
            //        var rec = new Rectangle(40 * j, 40 * i, 38, 38);
            //        linkedImage = linkedImage.Clone();
            //    }
            //}

            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    Image<Gray, byte> grayImage = images[i, j].SmoothGaussian(3).Convert<Gray, byte>().ThresholdBinaryInv(new Gray(230), new Gray(255));
                    VectorOfVectorOfPoint coutours = new VectorOfVectorOfPoint();
                    Mat hierarchy = new Mat();
                    CvInvoke.FindContours(grayImage, coutours, hierarchy, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);

                    VectorOfPoint approximation = new VectorOfPoint();
                    double perimeter = CvInvoke.ArcLength(coutours[0], true);
                    CvInvoke.ApproxPolyDP(coutours[0], approximation, 0.04 * perimeter, true);
                    CvInvoke.DrawContours(grayImage, coutours, 0, new MCvScalar(0, 0, 255), 1);
                    Moments moments = CvInvoke.Moments(coutours[0]);

                    int x = (int)(moments.M10 / moments.M00);
                    int y = (int)(moments.M01 / moments.M00);

                    if (approximation.Size == Convert.ToInt32(Fruits.Triangle))
                    {
                        if (approximation.ToArray()[1].Y == approximation.ToArray()[2].Y)
                        {
                            CvInvoke.PutText(grayImage, "T", new Point(x, y), Emgu.CV.CvEnum.FontFace.HersheyPlain, 1, new MCvScalar(255, 0, 0), 1);
                        }
                        else
                        {
                            CvInvoke.PutText(grayImage, "RT", new Point(x, y), Emgu.CV.CvEnum.FontFace.HersheyPlain, 1, new MCvScalar(255, 0, 0), 1);
                        }
                    }
                    else if (approximation.Size == Convert.ToInt32(Fruits.Square))
                    {
                        CvInvoke.PutText(grayImage, "K", new Point(x, y), Emgu.CV.CvEnum.FontFace.HersheyPlain, 1, new MCvScalar(255, 0, 0), 1);
                    }
                    else if (approximation.Size > Convert.ToInt32(Fruits.Square))
                    {
                        if (approximation.ToArray()[2].Y == approximation.ToArray()[4].Y)
                        {
                            CvInvoke.PutText(grayImage, "Kr", new Point(x, y), Emgu.CV.CvEnum.FontFace.HersheyPlain, 1, new MCvScalar(255, 0, 0), 1);
                        }
                        else
                        {
                            CvInvoke.PutText(grayImage, "Krg", new Point(x - 10, y), Emgu.CV.CvEnum.FontFace.HersheyPlain, 1, new MCvScalar(255, 0, 0), 1);
                        }
                    }

                    _form.pictureBoxContoursImage.Image = grayImage.ToBitmap();
                    _form.pictureBoxContoursImage.Refresh();
                }
            }
        }

        private void LoodingDefoultImage()
        {
            var hwnd = _process.MainWindowHandle;
            GetWindowRect(hwnd, out var rect);

            var image = new Bitmap(rect.Right - rect.Left, rect.Bottom - rect.Top);

            using (var graphics = Graphics.FromImage(image))
            {
                var hdcBitmap = graphics.GetHdc();
                PrintWindow(hwnd, hdcBitmap, 0);
                graphics.ReleaseHdc(hdcBitmap);
            }

            //Такие странные циферки потому-что надо было подкоректировать изображение.
            var rec = new Rectangle(9, 32, rect.Right - rect.Left - 105, rect.Bottom - rect.Top - 32);
            image = image.Clone(rec, image.PixelFormat);
            _form.pictureBoxDefoultImage.Image = image;
            _form.pictureBoxDefoultImage.Refresh();

            GetImagesShapes(image);

            _form.pictureBoxDefoultImage.Image = image;
            _form.pictureBoxDefoultImage.Refresh();
        }

        private void GetImagesShapes(Bitmap image)
        {
            for (int i = 0; i < images.Length; i++)
            {
                for (int j = 0; j < images.Length; j++)
                {
                    var rec = new Rectangle(_indentLength * j, _indentLength * i, _imageWidthOneGameCell, _imageHeightOneGameCell);
                    var newImage = image.Clone(rec, image.PixelFormat);
                    images[i, j] = new Image<Bgr, byte>(newImage);
                    _form.pictureBoxDefoultImage.Image = newImage;
                    _form.pictureBoxDefoultImage.Refresh();
                }
            }
        }
    }
}

