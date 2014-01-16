using Diffbot.Api.Client;
using Diffbot.Api.Client.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diffbot.Api.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            //ArticleUrl();
            //ArticleHtmlString();
            //ArticleHtmlStream();
            //FrontPageUrl();
            //ImageUrl();

            //ProductUrl();

            //ClassifierResultUrl();
            //TestUrl();

            //BulkTest();
            BulkGetDataTest();

            Console.WriteLine("Press any key to finish.");
            Console.ReadKey();
        }

        private async static void BulkGetDataTest()
        {
            BulkApiClient apiClient = new BulkApiClient("http://api.diffbot.com", "2");

            var job = apiClient.CreateJob(
                "408da6bbb2f6f23fd44f73ee78de2fa8",
                "testJob",
                new string[] { "http://blog.diffbot.com", "http://www.nytimes.com/2012/05/22/us/george-lucas-retreats-from-battle-with-neighbors.html", "http://techcrunch.com/2012/05/16/google-just-got-a-whole-lot-smarter-launches-its-knowledge-graph" },
                "http://api.diffbot.com/v2/article");

            await apiClient.UpdateStatus(job);

            var result = await apiClient.GetData(job);

        }

        private async static void BulkTest()
        {
            BulkApiClient apiClient = new BulkApiClient("http://api.diffbot.com", "2");
            var job = apiClient.CreateJob(
                "408da6bbb2f6f23fd44f73ee78de2fa8", 
                "testJob",
                new string[] { "http://blog.diffbot.com", "http://www.nytimes.com/2012/05/22/us/george-lucas-retreats-from-battle-with-neighbors.html", "http://techcrunch.com/2012/05/16/google-just-got-a-whole-lot-smarter-launches-its-knowledge-graph" }, 
                "http://api.diffbot.com/v2/article");
            await apiClient.Start(job);

            if (job.HasError)
            {
                Console.WriteLine("Error: {0}", job.Error);
            }
            else
            {
                Console.WriteLine("Name: {0}", job.Name);
                await apiClient.Pause(job);
                await apiClient.UpdateStatus(job);
                await apiClient.Resume(job);
                await apiClient.Delete(job);
            }
        }

        private async static void TestUrl()
        {
            ApiClient apiClient = new ApiClient("http://www.diffbot.com", "408da6bbb2f6f23fd44f73ee78de2fa8", "0");

            try
            {
                // http://goo.gl/JjqwN
                // http://www.diffbot.com/dev/docs/article/
                // http://www.diffbot.com/products/automatic/
                var result = await apiClient.CallCustomAPIAsync("http://www.xconomy.com/san-francisco/2012/07/25/diffbot-is-using-computer-vision-to-reinvent-the-semantic-web/", "test", null);

                if (result != null)
                {
                    Console.WriteLine("Title: {0}", result["Title"]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async static void ClassifierResultUrl()
        {
            ApiClient apiClient = new ApiClient();

            try
            {
                // http://goo.gl/JjqwN
                // http://www.diffbot.com/dev/docs/article/
                // http://www.diffbot.com/products/automatic/
                var result = await apiClient.GetPageClassification("http://www.pixmania.com/es/es/9044923/art/canon/eos-1100d-ef-s-18-55mm-dc.html", new string[] { "*" }, null, true);

                if (result != null)
                {
                    Console.WriteLine("Types: {0}", result.Stats.Types.Count());
                    if (result.Type == PageType.Product)
                    {
                        Products a = result.GetPageData<Products>();
                        Console.WriteLine("Title: {0}", a.Type);
                        Console.WriteLine("Date: {0}", a.DateCreated);
                        Console.WriteLine("Products: {0}", (a.Items != null) ? a.Items.Count() : 0);
                        if (a.Items.Count > 0)
                        {
                            Product p = a.Items.First();
                            Console.WriteLine(p.ProductId);
                            Console.WriteLine(p.Title);
                            Console.WriteLine(p.Description);
                            Console.WriteLine(p.RegularPriceDetails.Amount.ToString() + p.RegularPriceDetails.Symbol);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Error!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async static void ImageUrl()
        {
            ApiClient apiClient = new ApiClient();

            try
            {
                // http://goo.gl/JjqwN
                // http://www.diffbot.com/dev/docs/article/
                // http://www.diffbot.com/products/automatic/
                var result = await apiClient.GetImagesAsync("http://blog.xamarin.com", new string[] { "*" }, null);

                if (result != null)
                {
                    Images a = result;
                    Console.WriteLine("Title: {0}", a.Title);
                    Console.WriteLine("Date: {0}", a.Date_Created);
                    Console.WriteLine("Images: {0}", (a.Items != null) ? a.Items.Count() : 0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async static void ProductUrl()
        {
            ApiClient apiClient = new ApiClient();

            try
            {
                // http://goo.gl/JjqwN
                // http://www.diffbot.com/dev/docs/article/
                // http://www.diffbot.com/products/automatic/
                var result = await apiClient.GetProductsAsync("http://www.amazon.es/gp/product/0007532326/ref=s9_hps_bw_g14_i1?pf_rd_m=A1AT7YVPFBWXBL&pf_rd_s=merchandised-search-4&pf_rd_r=0BGE9195WKGR7AJ6T19Y&pf_rd_t=101&pf_rd_p=454426767&pf_rd_i=665418031", new string[] { "*" }, null);

                if (result != null)
                {
                    Products a = result;
                    Console.WriteLine("Title: {0}", a.Type);
                    Console.WriteLine("Date: {0}", a.DateCreated);
                    Console.WriteLine("Products: {0}", (a.Items != null) ? a.Items.Count() : 0);
                    if (a.Items.Count > 0)
                    {
                        Product p = a.Items.First();
                        Console.WriteLine(p.ProductId);
                        Console.WriteLine(p.Title);
                        Console.WriteLine(p.Description);
                        Console.WriteLine(p.RegularPriceDetails.Amount.ToString() + p.RegularPriceDetails.Symbol);
                    }
                }
                else
                {
                    Console.WriteLine("Error!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async static void FrontPageUrl()
        {
            ApiClient apiClient = new ApiClient("http://www.diffbot.com", "408da6bbb2f6f23fd44f73ee78de2fa8", "0");

            try
            {
                // http://goo.gl/JjqwN
                // http://www.diffbot.com/dev/docs/article/
                // http://www.diffbot.com/products/automatic/
                Dictionary<string, string> optionalParameters = new Dictionary<string, string>();
                optionalParameters.Add("all", "true");
                var result = await apiClient.GetFrontPageAsync("http://www.diffbot.com/products/automatic/", optionalParameters);

                if (result != null)
                {
                    FrontPage fp = result;
                    //Console.WriteLine("Title: {0}", fp.Title);
                    //Console.WriteLine("Date: {0}", fp.Date);
                    //Console.WriteLine("Images: {0}", (fp.Images != null) ? fp.Images.Count() : 0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async static void ArticleHtmlStream()
        {
            ApiClient apiClient = new ApiClient();

            try
            {

                using (var file = System.IO.File.OpenRead("demo.html"))
                {
                    var result = await apiClient.GetArticleAsync("http://www.diffbot.com/products/automatic/", new string[] { "*" }, null, file);

                    if (result != null)
                    {
                        Article a = result;
                        Console.WriteLine("Title: {0}", a.Title);
                        Console.WriteLine("Date: {0}", a.Date);
                        Console.WriteLine("Images: {0}", (a.Images != null) ? a.Images.Count() : 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async static void ArticleHtmlString()
        {
            ApiClient apiClient = new ApiClient();

            try
            {
                using (var file = System.IO.File.OpenText("demo.html"))
                {
                    var html = await file.ReadToEndAsync();
                    var result = await apiClient.GetArticleAsync("http://www.diffbot.com/products/automatic/", new string[] { "*" }, null, html);

                    if (result != null)
                    {
                        Article a = result;
                        Console.WriteLine("Title: {0}", a.Title);
                        Console.WriteLine("Date: {0}", a.Date);
                        Console.WriteLine("Images: {0}", (a.Images != null) ? a.Images.Count() : 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        static async void ArticleUrl()
        {
            ApiClient apiClient = new ApiClient();

            try
            {
                // http://goo.gl/JjqwN
                // http://www.diffbot.com/dev/docs/article/
                // http://www.diffbot.com/products/automatic/
                var result = await apiClient.GetArticleAsync("http://www.diffbot.com/products/automatic/", new string[] { "*" }, null);

                if (result != null)
                {
                    Article a = result;
                    Console.WriteLine("Title: {0}", a.Title);
                    Console.WriteLine("Date: {0}", a.Date);
                    Console.WriteLine("Images: {0}", (a.Images != null) ? a.Images.Count() : 0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
