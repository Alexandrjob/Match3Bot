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
        private readonly IntPtr _handleGame;

        private readonly int _indentLength = 40;
        private readonly int _imageWidthOneGameCell = 38;
        private readonly int _imageHeightOneGameCell = 38;

        private const int _sizePlayingFieldInFigures = 8;
        private readonly Image<Bgr, byte>[,] images = new Image<Bgr, byte>[_sizePlayingFieldInFigures, _sizePlayingFieldInFigures];
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
            _process = Process.GetProcessesByName(processName).FirstOrDefault();

            if (_process == null)
            {
                _form.labelStateProcces.Text = "Выбранный вами процесс не запущен или введен неверно.";
                _form.labelStateProcces.Refresh();
                return;
            }

            _handleGame = _process.MainWindowHandle;
            _form.labelStateProcces.Text = "Выбранный вами процесс запущен.";
            _form.labelStateProcces.Refresh();
        }

        public void StartAsync()
        {
            FindAllShapes();
            RecognizeAllShapes();

            Mouse mouse = new Mouse(_handleGame);


            int counterIdenticalАigures;

            for (int i = 0; i < _sizePlayingFieldInFigures; i++)
            {
                for (int j = 0; j < _sizePlayingFieldInFigures - 2; j++)
                {
                    if (fruits[i, j] == fruits[i, j + 2])
                    {
                        if (i != 0)
                        {
                            if (fruits[i, j] == fruits[i - 1, j + 1])
                            {
                                mouse.Move(j * 40 + 60, i * 40 - 20);
                            }
                        }
                        if (i <= 6)
                        {
                            if (fruits[i, j] == fruits[i + 1, j + 1])
                            {
                                mouse.Move(j * 40 + 60, i * 40 + 60);
                            }
                        }
                    }
                }
            }
        }

        private void FindAllShapes()
        {
            _process.Refresh();
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
        }

        private void GetImagesShapes(Bitmap image)
        {
            for (int i = 0; i < _sizePlayingFieldInFigures; i++)
            {
                for (int j = 0; j < _sizePlayingFieldInFigures; j++)
                {
                    var rec = new Rectangle(_indentLength * j, _indentLength * i, _imageWidthOneGameCell, _imageHeightOneGameCell);
                    var newImage = image.Clone(rec, image.PixelFormat);
                    images[i, j] = new Image<Bgr, byte>(newImage);
                }
            }
        }

        private void RecognizeAllShapes()
        {
            for (int i = 0; i <= _sizePlayingFieldInFigures - 1; i++)
            {
                for (int j = 0; j <= _sizePlayingFieldInFigures - 1; j++)
                {
                    Image<Gray, byte> grayImage = images[i, j].SmoothGaussian(3).Convert<Gray, byte>().ThresholdBinaryInv(new Gray(230), new Gray(255));
                    VectorOfVectorOfPoint coutours = new VectorOfVectorOfPoint();
                    Mat hierarchy = new Mat();
                    CvInvoke.FindContours(grayImage, coutours, hierarchy, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);

                    VectorOfPoint approximation = new VectorOfPoint();
                    double perimeter = CvInvoke.ArcLength(coutours[0], true);
                    CvInvoke.ApproxPolyDP(coutours[0], approximation, 0.04 * perimeter, true);
                    CvInvoke.DrawContours(grayImage, coutours, 0, new MCvScalar(0, 0, 255), 1);

                    if (approximation.Size == Convert.ToInt32(Fruits.Triangle))
                    {
                        if (approximation.ToArray()[1].Y == approximation.ToArray()[2].Y)
                        {
                            fruits[i, j] = Convert.ToInt32(Fruits.Triangle);
                        }
                        else
                        {
                            fruits[i, j] = Convert.ToInt32(Fruits.InvertedTriangle);
                        }
                    }
                    else if (approximation.Size == Convert.ToInt32(Fruits.Square))
                    {
                        fruits[i, j] = Convert.ToInt32(Fruits.Square);
                    }
                    else if (approximation.Size > Convert.ToInt32(Fruits.Square))
                    {
                        if (approximation.ToArray()[2].Y == approximation.ToArray()[4].Y)
                        {
                            fruits[i, j] = Convert.ToInt32(Fruits.Сross);
                        }
                        else
                        {
                            fruits[i, j] = Convert.ToInt32(Fruits.Сircle);
                        }
                    }

                    _form.pictureBoxContoursImage.Image = grayImage.ToBitmap();
                    _form.pictureBoxContoursImage.Refresh();
                }
            }
        }

        public void ShowArray()
        {
            for (int i = 0; i < _sizePlayingFieldInFigures; i++)
            {
                for (int j = 0; j < _sizePlayingFieldInFigures; j++)
                {
                    Console.Write(fruits[i, j]);
                }
                Console.WriteLine();
            }
        }
    }
}