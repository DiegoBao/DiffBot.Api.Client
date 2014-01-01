using Diffbot.Api.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diffbot.Api.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            ApiClient apiClient = new ApiClient("408da6bbb2f6f23fd44f73ee78de2fa8");

            apiClient.GetArticleAsync("http://blog.xamarin.com", null).ContinueWith(result =>
            {
                if (!result.IsCompleted)
                {
                    Console.WriteLine("Oops! something went wrong!");
                }
                else
                {
                    Article a = result.Result;
                    Console.WriteLine("Title: {0}", a.Title);
                    Console.WriteLine("Date: {0}", a.Date);
                    Console.WriteLine("Images: {0}", (a.Images != null) ? a.Images.Count() : 0);
                }
            });

            Console.WriteLine("Press any key to finish.");
            Console.ReadKey();
        }
    }
}
