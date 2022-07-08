using System.Windows.Input;
using TagGame.Commands;
using TagGame.Store;
using TagGame.ViewModels.Base;

namespace TagGame.ViewModels
{
    public class MainViewModel : NotifyingObject
    {
        public NotifyingObject CurrentViewModel => Navigator.Instance.CurrentViewModel;

        public ICommand ChangeViewModel { get; }

        public MainViewModel()
        {
            Navigator.Instance.StateChanged += () => OnPropertyChanged(nameof(CurrentViewModel));
            ChangeViewModel = new ChangeViewModelCommand();
        }
    }
}
