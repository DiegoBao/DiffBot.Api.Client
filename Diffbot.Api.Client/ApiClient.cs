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

        #region Article
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

                return  CreateArticle(result);
            }
            catch (Exception ex)
            {
                throw ex; //TODO: better exception handler
            }
        }

        private static Article CreateArticle(JObject result)
        {
            Article article = Newtonsoft.Json.JsonConvert.DeserializeObject<Article>(result.ToString());

            IDictionary<string, JToken> properties = result;
            // Set all properties in the additional field
            // Then remove those properties that have coded properties in the class Article
            // leaving the rest as additional properties.
            article.Properties = properties.ToDictionary(k => k.Key, k => (object)k.Value);
            foreach (var propName in article.Properties.Where(k => Article.PropertyNames.Contains(k.Key)).ToArray())
            {
                article.Properties.Remove(propName.Key);
            }
            return article;
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

                return CreateArticle(result);
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
            byte[] buffer = new byte[htmlStream.Length];
            await htmlStream.ReadAsync(buffer, 0, buffer.Length);

            string html = System.Text.UTF8Encoding.UTF8.GetString(buffer);

            return await GetArticleAsync(url, fields, optionalParameters, html);
        }
        #endregion

        #region FrontPage

        /// <summary>
        /// Calls the Article API. 
        /// </summary>
        /// <param name="url">Article URL to process.</param>
        /// <param name="fields">Array with the fields that will be returned by the API.</param>
        /// <returns>Returns information about the primary article content on the submitted page.</returns>
        public async Task<FrontPage> GetFrontPageAsync(string url, Dictionary<string, string> optionalParameters)
        {
            try
            {
                JObject result = await diffbotCall.ApiGetAsync(url, this.token, "frontpage", null, this.version, optionalParameters);

                return CreateFrontPage(result);
            }
            catch (Exception ex)
            {
                throw ex; //TODO: better exception handler
            }
        }

        public async Task<FrontPage> GetFrontPageAsync(string url, Dictionary<string, string> optionalParameters, string html)
        {
            try
            {
                JObject result = await diffbotCall.ApiPostAsync(url, this.token, "frontpage", null, this.version, optionalParameters, html, "text/html");

                return CreateFrontPage(result);
            }
            catch (Exception ex)
            {
                throw ex; //TODO: better exception handler
            }
        }

        public async Task<FrontPage> GetFrontPageAsync(string url, Dictionary<string, string> optionalParameters, Stream htmlStream)
        {
            try
            {
                byte[] buffer = new byte[htmlStream.Length];
                await htmlStream.ReadAsync(buffer, 0, buffer.Length);

                string html = System.Text.UTF8Encoding.UTF8.GetString(buffer);

                return await GetFrontPageAsync(url, optionalParameters, html);
            }
            catch (Exception ex)
            {
                throw ex; //TODO: better exception handler
            }
        }

        private static FrontPage CreateFrontPage(JObject result)
        {
            FrontPage frontPage = new FrontPage();

            foreach (var item in ((JArray)result["childNodes"]))
            {
                dynamic x = item;
                if (x.tagName.Value == "info")
                {
                    foreach (var cn in x["childNodes"])
                    {
                        var value = cn["childNodes"][0];
                        string tagName = cn["tagName"];
                        switch (tagName)
                        {
                            case "title":
                                frontPage.Title = value;
                                break;
                            case "sourceType":
                                frontPage.SourceType = value;
                                break;
                            case "sourceURL":
                                frontPage.SourceURL = value;
                                break;
                            case "icon":
                                frontPage.Icon = value;
                                break;
                            case "numItems":
                                frontPage.NumItems = (int)value;
                                break;
                            case "numSpamItems":
                                frontPage.NumSpamItems = (int)value;
                                break;
                            default:
                                frontPage.Properties.Add(tagName, (string)value);
                                break;
                        }
                    }
                }
                else
                {
                    FrontPageItem fpItem = Newtonsoft.Json.JsonConvert.DeserializeObject<FrontPageItem>(x.ToString());
                    foreach (var cn in x["childNodes"])
                    {
                        var value = cn["childNodes"][0];
                        string tagName = cn["tagName"];
                        switch (tagName)
                        {
                            case "pubDate":
                                fpItem.PubDate = (DateTime)value;
                                break;
                            case "description":
                                fpItem.Description = value;
                                break;
                            default:
                                fpItem.Properties.Add(tagName, (string)value);
                                break;
                        }
                    }
                    frontPage.Items.Add(fpItem);
                }
            }
            return frontPage;
        }


        #endregion

        #region Image
        /// <summary>
        /// Calls the Article API. 
        /// </summary>
        /// <param name="url">Article URL to process.</param>
        /// <param name="fields">Array with the fields that will be returned by the API.</param>
        /// <returns>Returns information about the primary article content on the submitted page.</returns>
        public async Task<Images> GetImagesAsync(string url, string[] fields, Dictionary<string, string> optionalParameters)
        {
            try
            {
                JObject result = await diffbotCall.ApiGetAsync(url, this.token, "image", fields, this.version, optionalParameters);

                return CreateImages(result);
            }
            catch (Exception ex)
            {
                throw ex; //TODO: better exception handler
            }
        }

        private static Images CreateImages(JObject result)
        {
            Images images = Newtonsoft.Json.JsonConvert.DeserializeObject<Images>(result.ToString());

            IDictionary<string, JToken> properties = result;
            // Set all properties in the additional field
            // Then remove those properties that have coded properties in the class Article
            // leaving the rest as additional properties.
            images.Properties = properties.ToDictionary(k => k.Key, k => (object)k.Value);
            foreach (var propName in images.Properties.Where(k => Images.PropertyNames.Contains(k.Key)).ToArray())
            {
                images.Properties.Remove(propName.Key);
            }
            return images;
        }
        #endregion

        #region Product
        /// <summary>
        /// Calls the Article API. 
        /// </summary>
        /// <param name="url">Article URL to process.</param>
        /// <param name="fields">Array with the fields that will be returned by the API.</param>
        /// <returns>Returns information about the primary article content on the submitted page.</returns>
        public async Task<Products> GetProductsAsync(string url, string[] fields, Dictionary<string, string> optionalParameters)
        {
            try
            {
                JObject result = await diffbotCall.ApiGetAsync(url, this.token, "product", fields, this.version, optionalParameters);

                return CreateProduct(result);
            }
            catch (Exception ex)
            {
                throw ex; //TODO: better exception handler
            }
        }

        private static Products CreateProduct(JObject result)
        {
            Products products = Newtonsoft.Json.JsonConvert.DeserializeObject<Products>(result.ToString());

            IDictionary<string, JToken> properties = result;
            // Set all properties in the additional field
            // Then remove those properties that have coded properties in the class Article
            // leaving the rest as additional properties.
            products.Properties = properties.ToDictionary(k => k.Key, k => (object)k.Value);
            foreach (var propName in products.Properties.Where(k => Products.PropertyNames.Contains(k.Key)).ToArray())
            {
                products.Properties.Remove(propName.Key);
            }
            return products;
        }
        #endregion

        #region Page Classifier

        /// <summary>
        /// Calls the Article API. 
        /// </summary>
        /// <param name="url">Article URL to process.</param>
        /// <param name="fields">Array with the fields that will be returned by the API.</param>
        /// <returns>Returns information about the primary article content on the submitted page.</returns>
        public async Task<ClassifierResult> GetPageClassification(string url, string[] fields, PageType? mode = null, bool stats = false)
        {
            try
            {
                Dictionary<string, string> optionalParameters = new Dictionary<string, string>();
                if (mode != null)
                {
                    optionalParameters.Add("mode", mode.Value.ToString().ToLowerInvariant());
                }

                if (stats)
                {
                    optionalParameters.Add("mode", "true");
                }

                JObject result = await diffbotCall.ApiGetAsync(url, this.token, "analyze", fields, this.version, optionalParameters);

                return CreateClassifierResult(result);
            }
            catch (Exception ex)
            {
                throw ex; //TODO: better exception handler
            }
        }

        private static ClassifierResult CreateClassifierResult(JObject result)
        {
            ClassifierResult classifierResult = Newtonsoft.Json.JsonConvert.DeserializeObject<ClassifierResult>(result.ToString());
            switch (classifierResult.Type)
            {
                case PageType.Article:
                    classifierResult.PageResult = CreateArticle(result);
                    break;
                case PageType.FrontPage:
                    classifierResult.PageResult = CreateFrontPage(result);
                    break;
                case PageType.Image:
                    classifierResult.PageResult = CreateImages(result);
                    break;
                case PageType.Product:
                    classifierResult.PageResult = CreateProduct(result);
                    break;
            }
            
            return classifierResult;
        }
        #endregion

        #region Custom API
        /// <summary>
        /// Calls the Article API. 
        /// </summary>
        /// <param name="url">Article URL to process.</param>
        /// <param name="fields">Array with the fields that will be returned by the API.</param>
        /// <returns>Returns information about the primary article content on the submitted page.</returns>
        public async Task<JObject> CallCustomAPIAsync(string url, string customApiName, Dictionary<string, string> optionalParameters)
        {
            try
            {
                return await diffbotCall.ApiGetAsync(url, this.token, customApiName, null, this.version, optionalParameters);
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
        public async Task<JObject> CallCustomAPIAsync(string url, string customApiName, Dictionary<string, string> optionalParameters, string html)
        {
            try
            {
                return await diffbotCall.ApiPostAsync(url, this.token, customApiName, null, this.version, optionalParameters, html, "text/html");
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
        public async Task<JObject> CallCustomAPIAsync(string url, string customApiName, Dictionary<string, string> optionalParameters, Stream htmlStream)
        {
            byte[] buffer = new byte[htmlStream.Length];
            await htmlStream.ReadAsync(buffer, 0, buffer.Length);

            string html = System.Text.UTF8Encoding.UTF8.GetString(buffer);

            return await CallCustomAPIAsync(url, customApiName, optionalParameters, html);
        }
        #endregion
    }
}
