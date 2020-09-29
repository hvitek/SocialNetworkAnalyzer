using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetworkAnalyzer.EntityFrameworkCore.Entities
{
    public class DbInformationEntity : DbEntity
    {
        public string Description { get; set; }

        public string Name { get; set; }
    }
}