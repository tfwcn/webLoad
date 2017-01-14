using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebLoad
{
    public static class MyAsync
    {
        public static async Task<string> GetResponseReturn(this IFrame frame, string script)
        {
            var response = await frame.EvaluateScriptAsync(script);
            if (!response.Success)
            {
                throw new Exception(response.Message);
            }
            Console.WriteLine(response.Result);
            return (string)response.Result;
        }
    }
}
