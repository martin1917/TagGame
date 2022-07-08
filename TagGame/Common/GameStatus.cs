namespace TagGame.Common
{
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
}
