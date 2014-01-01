using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Diffbot.Api.Client
{
    internal class DiffbotCall
    {
        HttpClient httpClient;
        public DiffbotCall()
            : this("http://api.diffbot.com") // Should be in configuration but for demo purposes it's hardcoded
        {
        }

        public DiffbotCall(string baseApiUrl)
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseApiUrl);
        }

        public async Task<string> ApiCallAsync(string url, string token, string api, string[] fields, int version)
        {
            #region Argument Validation
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException("url");
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException("token");
            }

            if (string.IsNullOrWhiteSpace(api))
            {
                throw new ArgumentNullException("api");
            }
            else if (!IsValidApi(api))
            {
                throw new ArgumentException("Invalid value for parameter 'api'.");
            }

            // TODO: Validate this is a valid check
            if (version < 2)
            {
                throw new ArgumentException("Invalid api version");
            }
            #endregion

            StringBuilder apiUrl = new StringBuilder();
            apiUrl.AppendFormat("v{0}/{1}?token={2}", version, api, token);
            if (fields != null && fields.Length > 0)
            {
                apiUrl.AppendFormat("&fields={0}", string.Join(",", fields));
            }
            apiUrl.AppendFormat("&url={0}", url);

            return await httpClient.GetStringAsync(apiUrl.ToString());            
        }

        private bool IsValidApi(string api)
        {
            // TODO: rest of the apis
            return api == "article";
        }
    }
}
