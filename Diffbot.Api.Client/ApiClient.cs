using Diffbot.Api.Client.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diffbot.Api.Client
{
    public class ApiClient
    {
        private DiffbotCall diffbotCall;
        private string token;
        private int version;

        /// <summary>
        /// Creates an instance of the ApiClient taken the needed parameters from the configuration file.
        /// </summary>
        public ApiClient()
            : this(
                ConfigurationManager.AppSettings["DiffbotUrl"],
                ConfigurationManager.AppSettings["DiffbotToken"],
                ConfigurationManager.AppSettings["DiffbotVersion"])
        {
        }

        /// <summary>
        /// Create an instance of the ApiClient with an specific token. Url and version are taken from the configuration file.
        /// </summary>
        /// <param name="token">Diffbot API Token required for using the API.</param>
        public ApiClient(string token)
            : this(ConfigurationManager.AppSettings["DiffbotUrl"], token, ConfigurationManager.AppSettings["DiffbotVersion"])
        {
        }

        /// <summary>
        /// Create an instance of the ApiClient.
        /// </summary>
        /// <param name="baseApiUrl">URL of the Diffbot Api. (ex. http://api.diffbot.com )</param>
        /// <param name="token">Diffbot API Token required for using the API.</param>
        /// <param name="version">Vesion of the API</param>
        public ApiClient(string baseApiUrl, string token, string version)
        {
            if (string.IsNullOrWhiteSpace(baseApiUrl))
            {
                throw new ArgumentNullException("baseApiUrl");
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException("token");
            }

            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentException("Incorrect API version", "version");
            }

            this.token = token;
            this.version = int.Parse(version);
            diffbotCall = new DiffbotCall(baseApiUrl);
        }

        /// <summary>
        /// Calls the Article API. 
        /// </summary>
        /// <param name="url">Article URL to process.</param>
        /// <param name="fields">Array with the fields that will be returned by the API.</param>
        /// <returns>Returns information about the primary article content on the submitted page.</returns>
        public async Task<Article> GetArticleAsync(string url, string[] fields, Dictionary<string, string> optionalParameters)
        {
            try
            {
                JObject result = await diffbotCall.ApiGetAsync(url, this.token, "article", fields, this.version, optionalParameters);

                Article article = Newtonsoft.Json.JsonConvert.DeserializeObject<Article>(result.ToString());
                article.Properties = new Dictionary<string, JToken>();
                foreach (var property in ((dynamic)result))
                {
                    if (!Article.PropertyNames.Any(p => p == property.Name.ToString()))
                    {
                        article.Properties.Add(property.Name, property.Value);
                    }
                }

                return article;
            }
            catch (Exception ex)
            {
                throw ex; //TODO: better exception handler
            }
        }

        /// <summary>
        /// Calls the Article API submitting the html to analyse directly.
        /// </summary>
        /// <param name="url">Article URL to process.</param>
        /// <param name="fields">Array with the fields that will be returned by the API.</param>
        /// <param name="html">Html to be analysed instead of calling the URL. The url parameter is still required.</param>
        /// <returns>Returns information about the primary article content on the submitted page.</returns>
        public async Task<Article> GetArticleAsync(string url, string[] fields, Dictionary<string, string> optionalParameters, string html)
        {
            try
            {
                JObject result = await diffbotCall.ApiPostAsync(url, this.token, "article", fields, this.version, optionalParameters, html, "text/html");

                Article article = Newtonsoft.Json.JsonConvert.DeserializeObject<Article>(result.ToString());
                article.Properties = new Dictionary<string, JToken>();
                foreach (var property in ((dynamic)result))
                {
                    if (!Article.PropertyNames.Any(p => p == property.Name.ToString()))
                    {
                        article.Properties.Add(property.Name, property.Value);
                    }
                }

                return article;
            }
            catch (Exception ex)
            {
                throw ex; //TODO: better exception handler
            }
        }

        /// <summary>
        /// Calls the Article API  submitting the html to analyse directly. 
        /// </summary>
        /// <param name="url">Article URL to process.</param>
        /// <param name="fields">Array with the fields that will be returned by the API.</param>
        /// <param name="htmlStream">Stream with the html to be analysed instead of calling the URL. The url parameter is still required.</param>
        /// <returns>Returns information about the primary article content on the submitted page.</returns>
        public async Task<Article> GetArticleAsync(string url, string[] fields, Dictionary<string, string> optionalParameters, Stream htmlStream)
        {
            try
            {
                byte[] buffer = new byte[htmlStream.Length];
                await htmlStream.ReadAsync(buffer, 0, buffer.Length);

                string html = System.Text.UTF8Encoding.UTF8.GetString(buffer);

                return await GetArticleAsync(url, fields, optionalParameters, html);
            }
            catch (Exception ex)
            {
                throw ex; //TODO: better exception handler
            }
        }
    }
}
