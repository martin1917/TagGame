using TagGame.Commands.Base;
using TagGame.Common;
using TagGame.Store;

namespace TagGame.Commands
{
    public class ChangeViewModelCommand : Command
    {
        public override bool CanExecute(object? parameter)
            => true;

        public override void Execute(object? parameter)
        {
            if (parameter is ViewModelTypes type)
            {
                Navigator.Instance.ChangeViewModel(type);
            }
        }
    }
}
