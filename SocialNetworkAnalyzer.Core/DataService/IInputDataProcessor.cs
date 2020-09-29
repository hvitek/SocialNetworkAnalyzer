using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialNetworkAnalyzer.Core.DataService
{
    public interface IInputDataProcessor
    {
        Task Proccess(List<NewEntryLine> newEntryLines, int? datasetId = null);

        bool TryParseInputLine(string line, out int userId, out int user2Id);
    }
}