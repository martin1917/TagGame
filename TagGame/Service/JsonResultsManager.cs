using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TagGame.Data;

namespace TagGame.Service
{
    public class JsonResultsManager : IResultsManager
    {
        private const string PATH = "history.json";

        /// <summary> Вернуть десериализованную коллекцию </summary>
        public IEnumerable<GameResult>? GetAll()
            => DeserializeJsons();

        /// <summary> Сохранить результат в json файл </summary>
        public void Save(GameResult result)
        {
            var jsons = DeserializeJsons()?.ToList() ?? new List<GameResult>();
            jsons.Add(result);
            jsons = jsons.OrderBy(r => r.Steps).ThenBy(r => r.Time).Take(10).ToList();
            using (var fs = new FileStream(PATH, FileMode.OpenOrCreate))
            JsonSerializer.Serialize(fs, jsons);
        }

        /// <summary> Десериализовать json в коллекцию </summary>
        private IEnumerable<GameResult>? DeserializeJsons()
        {
            IEnumerable<GameResult>? result;
            using (var fs = new FileStream(PATH, FileMode.OpenOrCreate))
            {
                try
                {
                    result = JsonSerializer.Deserialize<IEnumerable<GameResult>>(fs);
                }
                catch (JsonException e)
                {
                    result = null;
                }
            }

            return result;
        }
    }
}
