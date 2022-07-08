namespace TagGame.Common
{
    /// <summary> Константы для программы </summary>
    public static class Const
    {
        /// <summary> Кол-во строк </summary>
        public static readonly int Row = 4;

        /// <summary> Кол-во столбцов </summary>
        public static readonly int Col = 4;

        /// <summary> Размер ячейки </summary>
        public static readonly int CellSize = 80;

        /// <summary> Расстояние между ячейками </summary>
        public static readonly int GapBetweenCells = 10;

        /// <summary> Расстояние между левыми углами смежных ячеек </summary>
        public static readonly int Offset = CellSize + GapBetweenCells;

        /// <summary> Ширина поля </summary>м
        public static readonly int WidthMap = Col * Offset - GapBetweenCells;

        /// <summary> Выстоа поля </summary>м
        public static readonly int HeightMap = Row * Offset - GapBetweenCells;
    }
}
