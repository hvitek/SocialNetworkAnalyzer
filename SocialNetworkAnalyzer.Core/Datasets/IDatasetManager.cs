using System.Collections.Generic;
using System.Threading.Tasks;
using SocialNetworkAnalyzer.Core.Json;
using SocialNetworkAnalyzer.Entities;

namespace SocialNetworkAnalyzer.Core.Datasets
{
    public interface IDatasetManager
    {
        Task<int> CreateDatasetAndGetId(Dataset dataset);

        Task<List<Dataset>> GetAll();

        Task<List<NodeData>> GetEdges(int id);

        Task<List<NodeData>> GetNodes(int id);

        Task<DatasetInfo> GetWithInfo(int id);
    }
}