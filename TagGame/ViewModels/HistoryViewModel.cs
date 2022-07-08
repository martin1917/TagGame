using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using TagGame.Commands;
using TagGame.Data;
using TagGame.ViewModels.Base;
using System.IO;
using TagGame.Service;

namespace TagGame.ViewModels
{
    public class HistoryViewModel : NotifyingObject
    {
        private IResultsManager service = new JsonResultsManager();
        private IEnumerable<GameResult>? results;

        public IEnumerable<GameResult>? Results 
        {
            get => results;
            private set
            {
                results = value;
                OnPropertyChanged();
            }
        }

        public HistoryViewModel()
        {
            Results = service.GetAll();
        }
    }
}
