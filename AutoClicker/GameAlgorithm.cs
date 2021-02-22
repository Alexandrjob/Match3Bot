using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoClicker
{
    class GameAlgorithm: IDisposable
    {
        private readonly ApplicationForm _form;
        private readonly WindowGame windowGame;

        private readonly int _indentLength = 40;
        private readonly int _imageWidthOneGameCell = 38;
        private readonly int _imageHeightOneGameCell = 38;

        private const int _sizePlayingFieldInFigures = 8;
        private readonly Image<Bgr, byte>[,] images = new Image<Bgr, byte>[_sizePlayingFieldInFigures, _sizePlayingFieldInFigures];
        private int[,] fruits = new int[8, 8];
        private Bitmap image;

        private bool isPlayingFieldEmptyFields = false;
        private bool isStopGame = false;

        public GameAlgorithm(ApplicationForm applicationForm)
        {
            _form = applicationForm;
            try
            {
                windowGame = new WindowGame();
                DisplayProcessState("Выбранный вами процесс запущен.");
            }
            catch (Exception e)
            {
                DisplayProcessState(e.Message);
                isStopGame = true;
            }
        }

        public void DisplayProcessState(string message)
        {
            if (_form.labelStateProcces.InvokeRequired)
            {
                _form.labelStateProcces.Invoke(new Action<Bitmap>((b) =>
                {
                    _form.labelStateProcces.Text = message;
                    _form.labelStateProcces.Refresh();
                }
                ), image);
            }
        }

        public async Task StartAsync()
        {
            while (!isStopGame)
            {
                await FindAllShapesAsync();
                await RecognizeAllShapesAsync();
                await RunningGameAlgorithmAsync();
                if (Control.ModifierKeys == Keys.Shift)
{
                    isStopGame = true;
                }
                ShowArray();
            }
        }

        private async Task FindAllShapesAsync()
        {
            await Task.Run(() => FindAllShapes());
        }

        private void FindAllShapes()
        {
            image = windowGame.GetImage(out var rect);
            GetGameTime(image);

            if (_form.pictureBoxDefoultImage.InvokeRequired)
            {
                _form.pictureBoxDefoultImage.Invoke(new Action<Bitmap>((b) =>
                {
                    _form.pictureBoxDefoultImage.Image = image;
                    _form.pictureBoxDefoultImage.Refresh();
                }
                ), image);
            }
            ////Такие странные циферки потому-что надо подкоректировать изображение.
            var rec = new Rectangle(9, 32, rect.Right - rect.Left - 15, rect.Bottom - rect.Top - 32);
            image = image.Clone(rec, image.PixelFormat);
            //GetImagesShapes(image);
        }

        private void GetGameTime(Bitmap image)
        {
            var rec = new Rectangle(380, 60, 38, 38);
            var CvImage = new Image<Bgr, byte>(image.Clone(rec, image.PixelFormat));

            Image<Gray, byte> grayImage = CvImage.SmoothGaussian(1).Convert<Gray, byte>().ThresholdBinaryInv(new Gray(230), new Gray(255));

            if (_form.pictureBoxContoursImage.InvokeRequired)
            {
                _form.pictureBoxContoursImage.BeginInvoke(new Action<Bitmap>((b) =>
                {
                    _form.pictureBoxContoursImage.Image = b;
                    _form.pictureBoxContoursImage.Refresh();
                }
                ), grayImage.ToBitmap());
            }

            VectorOfVectorOfPoint coutours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            CvInvoke.FindContours(grayImage, coutours, hierarchy, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);

            VectorOfPoint approximation = new VectorOfPoint();
            double perimeter = CvInvoke.ArcLength(coutours[0], true);
            CvInvoke.ApproxPolyDP(coutours[0], approximation, 0.01 * perimeter, true);

            if (approximation.Size == 12 && coutours.Size == 1)
            {
                isStopGame = true;
            }
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

        private Image<Bgr, byte> GetImageShape(int i, int j)
        {
            var rec = new Rectangle(_indentLength * j, _indentLength * i, _imageWidthOneGameCell, _imageHeightOneGameCell);
            var newImage = image.Clone(rec, image.PixelFormat);
            return new Image<Bgr, byte>(newImage);
        }

        private async Task RecognizeAllShapesAsync()
        {
            await Task.Run(() => RecognizeAllShapes());
        }

        private void RecognizeAllShapes()
        {
            fruits = new int[8, 8];
            isPlayingFieldEmptyFields = false;

            for (int i = 0; i <= _sizePlayingFieldInFigures - 1; i++)
            {
                for (int j = 0; j <= _sizePlayingFieldInFigures - 1; j++)
                {
                    //Image<Gray, byte> grayImage = images[i,j].SmoothGaussian(3).Convert<Gray, byte>().ThresholdBinaryInv(new Gray(230), new Gray(255));
                    Image<Gray, byte> grayImage = GetImageShape(i,j).SmoothGaussian(3).Convert<Gray, byte>().ThresholdBinaryInv(new Gray(230), new Gray(255));
                    VectorOfVectorOfPoint coutours = new VectorOfVectorOfPoint();
                    Mat hierarchy = new Mat();
                    CvInvoke.FindContours(grayImage, coutours, hierarchy, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                    if (coutours.Size == 0)
                    {
                        isPlayingFieldEmptyFields = true;
                        return;
                    }

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
                }
            }
        }

        private async Task RunningGameAlgorithmAsync()
        {
            await Task.Run(() => RunningGameAlgorithm());
        }

        private void RunningGameAlgorithm()
        {
            Mouse mouse = new Mouse(windowGame.GetHandleGame());

            if(!isPlayingFieldEmptyFields)
            {
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
                                    Point point1 = new Point(j * 40 + 60, i * 40 + 20);
                                    Point point2 = new Point(j * 40 + 60, i * 40 - 20);
                                    mouse.MoveShape(point1, point2);
                                    return;
                                }
                            }
                            else if (i <= 7)
                            {
                                if (fruits[i, j] == fruits[i + 1, j + 1])
                                {
                                    Point point1 = new Point(j * 40 + 60, i * 40 + 20);
                                    Point point2 = new Point(j * 40 + 60, i * 40 + 60);
                                    mouse.MoveShape(point1, point2);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ShowArray()
        {
            for (int i = 0; i < _sizePlayingFieldInFigures; i++)
            {
                for (int j = 0; j < _sizePlayingFieldInFigures; j++)
                {
                    Console.Write(fruits[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        public void Dispose()
        {
            isStopGame = true;
        }
    }
}