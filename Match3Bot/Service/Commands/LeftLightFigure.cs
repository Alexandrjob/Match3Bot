using System.Drawing;

namespace Match3Bot.Service.Commands
{
    class LeftLightFigure: СombinationCommand
    {
        public override bool Contains(Box box)
        {
            var figures = box.Figures;
            var i = box.I;
            var j = box.J;

            if (figures[i, j] == figures[i, j + 1])
            {
                if (i < 7 & j > 0 & j < 7)
                {
                    if (figures[i, j] == figures[i + 1, j - 1])
                    {
                        Point1 = new Point(j * CellSize - 20, i * CellSize + 60);
                        Point2 = new Point(j * CellSize - 20, i * CellSize + 20);
                        return true;
                    }
                    else if (figures[i, j] == figures[i + 1, j + 2])
                    {
                        Point1 = new Point(j * CellSize + 100, i * CellSize + 60);
                        Point2 = new Point(j * CellSize + 100, i * CellSize + 20);
                        return true;
                    }
                }
                if (i > 0 & j > 0 & j < 7)
                {
                    if (figures[i, j] == figures[i - 1, j - 1])
                    {
                        Point1 = new Point(j * CellSize - 20, i * CellSize - 20);
                        Point2 = new Point(j * CellSize - 20, i * CellSize + 20);
                        return true;
                    }
                    else if (figures[i, j] == figures[i - 1, j + 2])
                    {
                        Point1 = new Point(j * CellSize + 100, i * CellSize - 20);
                        Point2 = new Point(j * CellSize + 100, i * CellSize + 20);
                        return true;
                    }
                }
            }

            return false;
        }

        public override void Execute(Mouse mouse)
        {
            mouse.MoveShape(Point1, Point2);
        }
    }
}
