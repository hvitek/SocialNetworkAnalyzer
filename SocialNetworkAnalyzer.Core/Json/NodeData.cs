using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetworkAnalyzer.Core.Json
{
    public class NodeData
    {
        public string Id { get; set; }

        public string Source { get; set; }

        public string Target { get; set; }
    }
}