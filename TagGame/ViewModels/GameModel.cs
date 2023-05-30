using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using TagGame.Commands;
using TagGame.Common;
using TagGame.Data;
using TagGame.Service;
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

        /// <summary> Класс сохраняет данные в json </summary>
        private IResultsManager service = new JsonResultsManager();

        /// <summary> Координаты пустой ячейки </summary>
        private (int r, int c) gap = (Const.Row - 1, Const.Col - 1);

        /// <summary> Состояние игры </summary>
        private GameStatus status = GameStatus.Prepare;

        /// <summary> Время игры </summary>
        private TimeOnly time = new(0, 0, 0);

        /// <summary> Дата запуска партии </summary>
        private DateTime date;
        
        /// <summary> Кол-во перемещений </summary>
        private int step = 0;

        #region props
        /// <summary> Свойство для ячеек </summary>
        public Cell[] Cells { get; private set; }

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

        /// <summary> Подготовить ячейки </summary>
        private void PrepareCells()
        {
            Cells = new Cell[15];
            for(int k = 0; k < Cells.Length; k++)
            {
                int y = k / Const.Row;
                int x = k % Const.Col;
                Cells[k] = new Cell(y, x, k + 1);
            }

            OnPropertyChanged(nameof(Cells));
        }

        /// <summary> Перемешать ячейки </summary>
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

                GameResult res = new GameResult(date, Time, Steps);
                service.Save(res);
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
            date = DateTime.Now;
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
}