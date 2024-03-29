﻿using TagGame.Store;
using TagGame.ViewModels.Base;

namespace TagGame.ViewModels
{
    public class MainViewModel : NotifyingObject
    {
        public NotifyingObject CurrentViewModel => Navigator.Instance.CurrentViewModel;

        public MainViewModel()
        {
            Navigator.Instance.StateChanged += () 
                => OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
