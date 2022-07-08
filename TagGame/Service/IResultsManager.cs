using System.Collections.Generic;
using TagGame.Data;

namespace TagGame.Service
{
    public interface IResultsManager
    {
        void Save(GameResult result);
        IEnumerable<GameResult>? GetAll();
    }
}
