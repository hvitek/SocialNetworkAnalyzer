using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetworkAnalyzer.EntityFrameworkCore.Entities
{
    public class Relationship : Entities.DbEntity
    {
        public virtual User User1 { get; set; }

        public int User1Id { get; set; }

        public virtual User User2 { get; set; }

        public int User2Id { get; set; }
    }
}