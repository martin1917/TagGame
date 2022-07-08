using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using TagGame.Commands;
using TagGame.Data;
using TagGame.ViewModels.Base;

namespace TagGame.ViewModels
{
    public class HistoryViewModel : NotifyingObject
    {
        public List<GameResult> Results { get; private set; } = new();

        public HistoryViewModel()
        {
            ChangeViewModelCommand = new ChangeViewModelCommand();
            //tests
            Results.AddRange(Enumerable.Range(0, 10).Select(i => new GameResult(DateTime.Now.AddHours(i), "03:21", 10 * i)));
        }

        public ICommand ChangeViewModelCommand { get; }
    }
}
