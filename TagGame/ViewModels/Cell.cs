using TagGame.Common;
using TagGame.ViewModels.Base;

namespace TagGame.ViewModels
{
    /// <summary> Ячейка для игры в пятнашки </summary>
    public class Cell : NotifyingObject
    {
        int row, col;

        public int Num { get; }
        public int Y => Const.Offset * row;
        public int X => Const.Offset * col;

        #region props for ui
        public int Size => Const.CellSize;
        #endregion

        public Cell(int row, int col, int num)
        {
            this.row = row;
            this.col = col;
            Num = num;
        }

        public void Deconstruct(out int r, out int c)
        {
            r = row;
            c = col;
        }

        public void Move(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up: Up(); break;
                case Direction.Down: Down(); break;
                case Direction.Left: Left(); break;
                case Direction.Right: Right(); break;
            }
        }

        public bool IsHere(int row, int col)
            => this.row == row && this.col == col;

        public bool IsCorrect()
            => row * Const.Col + col + 1 == Num;

        void Up()
        {
            row--;
            OnPropertyChanged(nameof(Y));
        }

        void Down()
        {
            row++;
            OnPropertyChanged(nameof(Y));
        }

        void Left()
        {
            col--;
            OnPropertyChanged(nameof(X));
        }

        void Right()
        {
            col++;
            OnPropertyChanged(nameof(X));
        }
    }
}
