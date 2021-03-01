using System.Drawing;

namespace Match3Bot.Service.Commands
{
    class BetweenTwoFiguresCommand: СombinationCommand
    {
        public override bool Contains(int[,] fruits)
        {
            for (int i = 0; i < base.SizePlayingFieldInFigures; i++)
            {
                for (int j = 0; j < base.SizePlayingFieldInFigures - 2; j++)
                {
                    //Horizontally
                    if (fruits[i, j] == fruits[i, j + 2])
                    {
                        if (i != 0)
                        {
                            if (fruits[i, j] == fruits[i - 1, j + 1])
                            {
                                Point1 = new Point(j * CellSize + 60, i * CellSize + 20);
                                Point2 = new Point(j * CellSize + 60, i * CellSize - 20);
                                return true;
                            }
                        }
                        else if (i <= 7)
                        {
                            if (fruits[i, j] == fruits[i + 1, j + 1])
                            {
                                Point1 = new Point(j * CellSize + 60, i * CellSize + 20);
                                Point2 = new Point(j * CellSize + 60, i * CellSize + 60);
                                return true;
                            }
                        }
                    }

                    //Vertically
                }
            }

            return false;
        }

        public override void Execute(int[,] fruits, Mouse mouse)
        {
            mouse.MoveShape(Point1, Point2);
        }
    }
}
