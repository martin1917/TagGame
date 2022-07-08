using System;
using TagGame.Common;
using TagGame.ViewModels;
using TagGame.ViewModels.Base;

namespace TagGame.Store
{
    public class Navigator
    {
        private static Navigator instance;
        public static Navigator Instance => instance ??= new Navigator();

        private NotifyingObject currentViewModel = new MenuViewModel();
        
        public event Action? StateChanged;

        public NotifyingObject CurrentViewModel
        {
            get => currentViewModel;
            set
            {
                currentViewModel = value;
                StateChanged?.Invoke();
            }   
        }

        public void ChangeViewModel(ViewModelTypes type)
        {
            CurrentViewModel = type switch
            {
                ViewModelTypes.Game => new GameModel(),
                ViewModelTypes.History => new HistoryViewModel(),
                ViewModelTypes.Menu => new MenuViewModel()
            };
        }
    }
}
