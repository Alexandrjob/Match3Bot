using System.Drawing;

namespace Match3Bot.Service.Commands
{
    class BetweenTwoFiguresCommand: СombinationCommand
    {
        public override bool Contains(Box box)
        {
            var figures = box.Figures;
            var i = box.I;
            var j = box.J;

            //Horizontally
            if (figures[i, j] == figures[i, j + 2])
            {
                if (i != 0)
                {
                    if (figures[i, j] == figures[i - 1, j + 1])
                    {
                        Point1 = new Point(j * CellSize + 60, i * CellSize + 20);
                        Point2 = new Point(j * CellSize + 60, i * CellSize - 20);
                        return true;
                    }
                }
                else if (i <= 7)
                {
                    if (figures[i, j] == figures[i + 1, j + 1])
                    {
                        Point1 = new Point(j * CellSize + 60, i * CellSize + 20);
                        Point2 = new Point(j * CellSize + 60, i * CellSize + 60);
                        return true;
                    }
                }
            }

            //Vertically

            return false;
        }

        public override void Execute(Mouse mouse)
        {
            mouse.MoveShape(Point1, Point2);
        }
    }
}
