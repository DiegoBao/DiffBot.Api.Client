using Diffbot.Api.Client;
using Diffbot.Api.Client.Model;
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
            FrontPageUrl();

            Console.WriteLine("Press any key to finish.");
            Console.ReadKey();
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
