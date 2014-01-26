using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Diffbot.Api.Client.Batch
{
    public class BatchApi
    {
        private HttpClient httpClient;
        private DiffbotCall diffbotCall;
        private string baseApiUrl;
        private int? version;
        public BatchApi(string baseApiUrl, string version)
        {
            this.baseApiUrl = baseApiUrl;
            if (!string.IsNullOrWhiteSpace(version))
            {
                this.version = int.Parse(version);
            }
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseApiUrl);
            this.diffbotCall = new DiffbotCall(baseApiUrl);
        }

        public async Task<BatchResult> CreateBatch(string token, List<BatchUrlRequest> urls, int? timeout = null)
        {
            StringBuilder parameters = new StringBuilder();
            parameters.AppendFormat("token={0}", token);
            if (timeout.HasValue)
            {
                parameters.AppendFormat("&ttimeout={0}", timeout.Value);
            }
            parameters.AppendFormat("batch={0}", JsonConvert.SerializeObject(urls));

            var result = await this.httpClient.PostAsync("api/batch", new StringContent(parameters.ToString()));
            if (result.IsSuccessStatusCode)
            {
                string json = await result.Content.ReadAsStringAsync();
                BatchResult batchResult = JsonConvert.DeserializeObject<BatchResult>(json);
                return batchResult;
            }
            else
            {
                BatchResult batchResult = new BatchResult();
                batchResult.Code = (int)result.StatusCode;
                return batchResult;
            }
        }
    }
}
