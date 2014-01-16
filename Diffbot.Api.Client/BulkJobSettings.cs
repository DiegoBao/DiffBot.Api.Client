using Diffbot.Api.Client.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diffbot.Api.Client
{
    public class BulkJobSettings
    {
        public string Token { get; set; }
        public string Name { get; set; }

        [JsonConverter(typeof(UrlsSpaceSeparatedConverter))]
        public IEnumerable<string> Urls { get; set; }
        public string ApiUrl { get; set; }
        public string NotifyEmail { get; set; }
        public string NotifyWebHook { get; set; }
        public float? Repeat { get; set; }
        public int? MaxRounds { get; set; }
        public string PageProcessPattern { get; set; }

        public BulkJobSettings(string token, string name, IEnumerable<string> urls, string apiUrl)
        {
            Token = token;
            Name = name;
            Urls = urls;
            ApiUrl = apiUrl;
            NotifyEmail = null;
            NotifyWebHook = null;
            Repeat = null;
            MaxRounds = null;
            PageProcessPattern = null;
        }
    }
}
