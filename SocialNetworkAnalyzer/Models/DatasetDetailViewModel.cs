using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetworkAnalyzer.Models
{
    public class DatasetDetailViewModel
    {
        public string Description { get; set; }

        public int Edges { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public double NumOfFriendsAvg { get; set; }

        public int Users { get; set; }
    }
}