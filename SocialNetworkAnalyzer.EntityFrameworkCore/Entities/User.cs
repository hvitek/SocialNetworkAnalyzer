using SocialNetworkAnalyzer.Entities;
using SocialNetworkAnalyzer.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetworkAnalyzer.EntityFrameworkCore.Entities
{
    public class User : DbEntity
    {
        public virtual Dataset Dataset { get; set; }

        public int DatasetId { get; set; }

        public int RemoteId { get; set; }
    }
}