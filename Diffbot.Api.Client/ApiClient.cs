using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diffbot.Api.Client
{
    public class ApiClient
    {
        private DiffbotCall diffbotCall;
        private string token;

        public ApiClient(string token)
        {
            this.token = token;
            diffbotCall = new DiffbotCall(); // takes base api from config file
        }
        public ApiClient(string baseApiUrl, string token)
        {
            this.token = token;
            diffbotCall = new DiffbotCall(baseApiUrl);
        }

        public async Task<Article> GetArticleAsync(string url, string[] fields)
        {
            try
            {
                string result = await diffbotCall.ApiCallAsync(url, this.token, "article", fields, 2);

                return Newtonsoft.Json.JsonConvert.DeserializeObject<Article>(result);
            }
            catch (Exception ex)
            {
                throw ex; //TODO: better exception handler
            }
        }
    }
}
