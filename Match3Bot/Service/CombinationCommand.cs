using System.Drawing;

namespace Match3Bot
{
    public abstract class СombinationCommand
    {
        public virtual int SizePlayingFieldInFigures { get => 8; }
        public virtual int CellSize { get => 40; }
        public virtual Point Point1 { get; set; }
        public virtual Point Point2 { get; set; }

        public abstract void Execute(int[,] fruits, Mouse mouse);

        public abstract bool Contains(int[,] fruits);
    }
}
