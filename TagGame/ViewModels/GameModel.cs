﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using TagGame.Commands;
using TagGame.ViewModels.Base;

namespace TagGame.ViewModels
{
    /// <summary> Игра </summary>
    public class GameModel : NotifyingObject
    {
        /// <summary> Рандомайзер для перемешивания ячеек </summary>
        private readonly Random rnd = new();

        /// <summary> Таймер </summary>
        private readonly DispatcherTimer dispatcherTimer = new();

        /// <summary> Координаты пустой ячейки </summary>
        private (int r, int c) gap = (Const.Row - 1, Const.Col - 1);
        
        /// <summary> Состояние игры </summary>
        private GameStatus status = GameStatus.Prepare;

        /// <summary> Время игры </summary>
        private TimeOnly time = new(0, 0, 0);
        
        /// <summary> Кол-во перемещений </summary>
        private int step = 0;

        #region props
        /// <summary> Ячейки </summary>
        public ObservableCollection<Cell> Cells { get; private set; }

        /// <summary> Свойство для статуса игры </summary>
        public GameStatus Status
        {
            get => status;
            set => Set(ref status, value);
        }

        /// <summary> Свойство для кол-ва перемещений </summary>
        public int Steps
        {
            get => step;
            set => Set(ref step, value);
        }

        /// <summary> Свойство для времени </summary>
        public string Time => time.ToString("mm:ss");
        #endregion

        #region props for ui
        public int Width => Const.WidthMap;
        public int Height => Const.HeightMap;
        #endregion

        /// <summary> Конструктор </summary>
        public GameModel()
        {
            #region creating commands
            MoveCommand = new LambdaCommand(OnMoveCommandExecuted, CanMoveCommandExecute);
            StartGameCommand = new LambdaCommand(OnStartGameCommandExecuted, CanStartGameCommandExecute);
            ChangeStatusGameCommand = new LambdaCommand(OnChangeStatusGameCommandExecuted, CanChangeStatusGameCommandExecute);
            ReplayCommand = new LambdaCommand(OnReplayCommandExecuted, CanReplayCommandExecute);
            RestartGameCommand = new LambdaCommand(OnRestartGameCommandExecuted, CanRestartGameCommandExecute);
            #endregion

            SetSettingsTimer();

            PrepareCells();
        }

        /// <summary> Настройка таймера </summary>
        void SetSettingsTimer()
        {
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += (s, e) => 
            {
                time = time.Add(TimeSpan.FromSeconds(1));
                OnPropertyChanged(nameof(Time));
            };
        }

        /// <summary>
        /// Переместить ячейку на пустое место
        /// </summary>
        /// <param name="cell">перемещаемая ячейка</param>
        /// <param name="dir">направление, в котором расположена пустая ячейка </param>
        private void SwapWithGap(Cell cell, Direction dir)
        {
            (int row, int col) = cell;
            gap = (row, col);
            cell.Move(dir);
            Steps++;
        }

        /// <summary>
        /// Перемещение ячейки в направлении
        /// </summary>
        /// <param name="dir">Направление, в котором будет перемещаться ячейка</param>
        public void MoveInDiraction(Direction dir)
        {
            (int r, int c) pos = dir switch
            {
                Direction.Up => (gap.r + 1, gap.c),
                Direction.Down => (gap.r - 1, gap.c),
                Direction.Left => (gap.r, gap.c + 1),
                Direction.Right => (gap.r, gap.c - 1)
            };

            var cell = Cells.FirstOrDefault(c => c.IsHere(pos.r, pos.c));

            if (cell != null)
                SwapWithGap(cell, dir);
        }

        /// <summary>
        /// Перемещение ячейки в доступное место
        /// </summary>
        /// <param name="cell">перемещаемая ячейка</param>
        public void MoveCell(Cell cell)
        {
            (int row, int col) = cell;
            
            Direction? dir = null;
            if ((row - 1, col) == gap) dir = Direction.Up;
            if ((row + 1, col) == gap) dir = Direction.Down;
            if ((row, col - 1) == gap) dir = Direction.Left;
            if ((row, col + 1) == gap) dir = Direction.Right;

            if(dir != null)
                SwapWithGap(cell, dir.Value);
        }

        /// <summary>
        /// Подготовить ячейки
        /// </summary>
        private void PrepareCells()
        {
            var preparingCells =
                from k in Enumerable.Range(0, Const.Row * Const.Col - 1)
                select new Cell(k / Const.Row, k % Const.Col, k + 1);

            Cells = new ObservableCollection<Cell>(preparingCells);
            OnPropertyChanged(nameof(Cells));
        }

        /// <summary>
        /// Перемешать ячейки
        /// </summary>
        private void Mix()
        {
            var dirs = Enum.GetValues<Direction>();
            for (int i = 0; i < 100; i++)
            {
                var index = rnd.Next(dirs.Length);
                var dir = dirs[index];
                MoveInDiraction(dir);
            }
        }

        /// <summary>
        /// Проверить, что все ячейки стоят на своих местах
        /// </summary>
        /// <returns></returns>
        private bool CheckWin() => Cells.All(c => c.IsCorrect());

        /// <summary> Переводим игровые параметры в начальные состояния </summary>
        private void ResetGameParameters()
        {
            gap = (Const.Row - 1, Const.Col - 1);
            time = new TimeOnly(0, 0, 0);
            OnPropertyChanged(nameof(Time));
            Steps = 0;
        }

        #region Commands

        #region MoveGapCommand
        public ICommand MoveCommand { get; }  

        private bool CanMoveCommandExecute(object? p)
            => status is GameStatus.Play;


        private void OnMoveCommandExecuted(object? p)
        {
            if(p is Direction dir)
                MoveInDiraction(dir);

            else if(p is string numStr && int.TryParse(numStr, out int num))
                MoveCell(Cells.First(c => c.Num == num));
                
            if (CheckWin())
            {
                Status = GameStatus.Win;
                dispatcherTimer.Stop();
            }
        }
        #endregion 

        #region StartGameCommand
        public ICommand StartGameCommand { get; }

        private bool CanStartGameCommandExecute(object? p)
            => status is GameStatus.Prepare;

        private void OnStartGameCommandExecuted(object? p)
        {
            Mix();
            Steps = 0;
            Status = GameStatus.Play;
            dispatcherTimer.Start();
        }
        #endregion

        #region ChangeStatusGameCommand
        public ICommand ChangeStatusGameCommand { get; }

        private bool CanChangeStatusGameCommandExecute(object? p)
            => status is GameStatus.Play || status is GameStatus.Pause;

        private void OnChangeStatusGameCommandExecuted(object? p)
        {
            if (status is GameStatus.Play)
            {
                Status = GameStatus.Pause;
                dispatcherTimer.Stop();
            }

            else if (status is GameStatus.Pause)
            {
                Status = GameStatus.Play;
                dispatcherTimer.Start();
            }
        }
        #endregion 

        #region ReplayCommand
        public ICommand ReplayCommand { get; }

        private bool CanReplayCommandExecute(object? p) 
            => status is GameStatus.Win;

        private void OnReplayCommandExecuted(object? p)
        {
            Status = GameStatus.Prepare;
            ResetGameParameters();
        }
        #endregion

        #region RestartGameCommand
        public ICommand RestartGameCommand { get; }

        private bool CanRestartGameCommandExecute(object? p)
            => status is GameStatus.Pause;

        private void OnRestartGameCommandExecuted(object? p)
        {
            dispatcherTimer.Stop();
            Status = GameStatus.Prepare;
            PrepareCells();
            ResetGameParameters();
        }
        #endregion

        #endregion
    }

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

    /// <summary> Состояние игры </summary>
    public enum GameStatus
    {
        /// <summary> Игра еще не началась </summary>
        Prepare,

        /// <summary> Игра в процессе </summary>
        Play,

        /// <summary> Игра на паузе </summary>
        Pause,

        /// <summary> Победа </summary>
        Win,
    }

    /// <summary> Направления </summary>
    public enum Direction
    {
        /// <summary> Вверх </summary>
        Up,

        /// <summary> Вниз </summary>
        Down,

        /// <summary> Влево </summary>
        Left, 

        /// <summary> Вправо </summary>
        Right
    }

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