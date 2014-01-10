using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diffbot.Api.Client.Model
{
    public class Images
    {
        public string Title { get; set; }
        public List<Image> AllImages { get; set; }
        public List<string> Links { get; set; }
        public string Type { get; set; }
        public string Resolved_Url { get; set; }
        public string Url { get; set; }
    }
}
