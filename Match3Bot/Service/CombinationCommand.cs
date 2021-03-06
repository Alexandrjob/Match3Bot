using System.Drawing;

namespace Match3Bot.Service
{
    public abstract class СombinationCommand
    {
        public virtual int SizePlayingFieldInFigures { get => 8; }
        public virtual int CellSize { get => 40; }
        public virtual Point Point1 { get; set; }
        public virtual Point Point2 { get; set; }

        public abstract void Execute(Mouse mouse);

        public abstract bool Contains(Box box);
    }
}
