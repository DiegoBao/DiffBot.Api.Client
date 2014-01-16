using Diffbot.Api.Client.Model;
using Newtonsoft.Json;
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
            try
            {
                var response = await diffbotCall.PostAsync("bulk", this.version, job.Settings);
                if (response["Error"] != null)
                {
                    job.Error = response["StatusCode"].ToString() + ": " + response["Error"].ToString();
                }
                else
                {
                    job.Error = null;
                    Jobs jobs = JsonConvert.DeserializeObject<Jobs>(response.ToString());
                    job.JobStatus = jobs.AllJobs.FirstOrDefault(j => j.Name == job.Name);
                }
            }
            catch (Exception ex)
            {
                job.Error = ex.InnerException != null ? " " + ex.InnerException.Message : ex.Message;
            }
        }

        public async Task Pause(BulkJob job)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("pause", "1");
            await BulkOperations(job, parameters);

        }

        public async Task Resume(BulkJob job)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("pause", "0");
            await BulkOperations(job, parameters);
        }

        public async Task Delete(BulkJob job)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("delete", "1");
            await BulkOperations(job, parameters);
        }

        public async Task UpdateStatus(BulkJob job)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            await BulkOperations(job, parameters);
        }

       private async Task BulkOperations(BulkJob job, Dictionary<string, string> parameters)
        {
            try
            {
                parameters.Add("token", job.Settings.Token);
                parameters.Add("name", job.Name);
                var response = await diffbotCall.GetAsync("bulk", this.version, parameters);
                if (response["Error"] != null)
                {
                    job.Error = response["StatusCode"].ToString() + ": " + response["Error"].ToString();
                }
                else
                {
                    job.Error = null;
                    Jobs jobs = JsonConvert.DeserializeObject<Jobs>(response.ToString());
                    if (jobs.AllJobs != null)
                    {
                        job.JobStatus = jobs.AllJobs.FirstOrDefault(j => j.Name == job.Name);
                    }
                    else
                    {
                        job.JobStatus = null;
                    }
                }
            }
            catch (Exception ex)
            {
                job.Error = ex.InnerException != null ? " " + ex.InnerException.Message : ex.Message;
            }
        }
    }
}
