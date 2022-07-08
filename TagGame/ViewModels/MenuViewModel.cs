using System.Windows.Input;
using TagGame.Commands;
using TagGame.ViewModels.Base;

namespace TagGame.ViewModels
{
    public class MenuViewModel : NotifyingObject
    {
        public ICommand ChangeViewModelCommand { get; }

        public MenuViewModel()
        {
            ChangeViewModelCommand = new ChangeViewModelCommand();
        }
    }
}
