using System;
using TagGame.Commands.Base;

namespace TagGame.Commands
{
    public class LambdaCommand : Command
    {
        private readonly Action<object?> execute;
        private readonly Func<object?, bool> canExecute;

        public LambdaCommand(Action<object?> execute,
            Func<object?, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public override bool CanExecute(object? parameter) 
            => canExecute?.Invoke(parameter) ?? true;

        public override void Execute(object? parameter) 
            => execute?.Invoke(parameter);
    }
}
