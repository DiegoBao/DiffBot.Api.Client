using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diffbot.Api.Client
{
    public class BulkApiClient
    {
        private DiffbotCall diffbotCall;
        private int version;

        #region Constructors

        /// <summary>
        /// Creates an instance of the ApiClient taken the needed parameters from the configuration file.
        /// </summary>
        public BulkApiClient()
            : this(
                ConfigurationManager.AppSettings["DiffbotUrl"],
                ConfigurationManager.AppSettings["DiffbotVersion"])
        {
        }

        /// <summary>
        /// Create an instance of the ApiClient.
        /// </summary>
        /// <param name="baseApiUrl">URL of the Diffbot Api. (ex. http://api.diffbot.com )</param>
        /// <param name="token">Diffbot API Token required for using the API.</param>
        /// <param name="version">Vesion of the API</param>
        public BulkApiClient(string baseApiUrl, string version)
        {
            if (string.IsNullOrWhiteSpace(baseApiUrl))
            {
                throw new ArgumentNullException("baseApiUrl");
            }

            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentException("Incorrect API version", "version");
            }

            this.version = int.Parse(version);
            diffbotCall = new DiffbotCall(baseApiUrl);
        }

        #endregion

        public BulkJob CreateJob(BulkJobSettings settings)
        {
            return new BulkJob(settings);
        }

        public BulkJob CreateJob(string token, string name, IEnumerable<string> urls, string apiUrl)
        {
            return new BulkJob(new BulkJobSettings(token, name, urls, apiUrl));
        }

        public async Task Start(BulkJob job)
        {
            
        }

        public async Task Pause(BulkJob job)
        {

        }

        public async Task Delete(BulkJob job)
        {

        }


    }
}
