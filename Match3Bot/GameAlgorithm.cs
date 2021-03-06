﻿using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Match3Bot.Service;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Match3Bot.Service.Commands;
using Match3Bot.Models;

namespace Match3Bot
{
    class GameAlgorithm: IDisposable
    {
        private readonly ApplicationForm _form;
        private readonly WindowGame windowGame;
        private readonly ICommandService commandService;

        private readonly int _cellSize = 40;
        private const int _sizePlayingFieldInFigures = 8;
        private readonly Image<Bgr, byte>[,] images = new Image<Bgr, byte>[_sizePlayingFieldInFigures, _sizePlayingFieldInFigures];
        private int[,] figures = new int[8, 8];
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
                commandService = new CommandService();
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
                //ShowArray();
                Thread.Sleep(500);
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
                    var rec = new Rectangle(_cellSize * j, _cellSize * i, _cellSize, _cellSize);
                    var newImage = image.Clone(rec, image.PixelFormat);
                    images[i, j] = new Image<Bgr, byte>(newImage);
                }
            }
        }

        private Image<Bgr, byte> GetImageShape(int i, int j)
        {
            var rec = new Rectangle(_cellSize * j, _cellSize * i, _cellSize - 2, _cellSize - 2);
            var newImage = image.Clone(rec, image.PixelFormat);
            return new Image<Bgr, byte>(newImage);
        }

        private async Task RecognizeAllShapesAsync()
        {
            await Task.Run(() => RecognizeAllShapes());
        }

        private void RecognizeAllShapes()
        {
            figures = new int[8, 8];
            isPlayingFieldEmptyFields = false;

            for (int i = 0; i <= _sizePlayingFieldInFigures - 1; i++)
            {
                for (int j = 0; j <= _sizePlayingFieldInFigures - 1; j++)
                {
                    //Image<Gray, byte> grayImage = images[i,j].SmoothGaussian(3).Convert<Gray, byte>().ThresholdBinaryInv(new Gray(230), new Gray(255));
                    Image<Gray, byte> grayImage = GetImageShape(i, j).SmoothGaussian(3).Convert<Gray, byte>().ThresholdBinaryInv(new Gray(230), new Gray(255));
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

                    if (approximation.Size == Convert.ToInt32(Figures.Triangle))
                    {
                        if (approximation.ToArray()[1].Y == approximation.ToArray()[2].Y)
                        {
                            figures[i, j] = Convert.ToInt32(Figures.Triangle);
                        }
                        else
                        {
                            figures[i, j] = Convert.ToInt32(Figures.InvertedTriangle);
                        }
                    }
                    else if (approximation.Size == Convert.ToInt32(Figures.Square))
                    {
                        figures[i, j] = Convert.ToInt32(Figures.Square);
                    }
                    else if (approximation.Size > Convert.ToInt32(Figures.Square))
                    {
                        if (approximation.ToArray()[2].Y == approximation.ToArray()[4].Y)
                        {
                            figures[i, j] = Convert.ToInt32(Figures.Сross);
                        }
                        else
                        {
                            figures[i, j] = Convert.ToInt32(Figures.Сircle);
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

            if (!isPlayingFieldEmptyFields)
            {
                for (int i = 0; i < _sizePlayingFieldInFigures; i++)
                {
                    for (int j = 0; j < _sizePlayingFieldInFigures; j++)
                    {
                        var box = new Box()
                        {
                            Figures = figures,
                            I = i,
                            J = j
                        };

                        foreach (var item in commandService.GetCommands())
                        {
                            if (item.Contains(box))
                            {
                                item.Execute(mouse);
                                return;
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
                    Console.Write(figures[i, j]);
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