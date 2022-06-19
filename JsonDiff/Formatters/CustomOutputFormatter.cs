using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace JsonDiff.Formatters
{
    public class CustomOutputFormatter : TextOutputFormatter
    {
        public CustomOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/custom"));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            string text = JsonConvert.SerializeObject(context.Object);
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            text = Convert.ToBase64String(bytes);
            await context.HttpContext.Response.WriteAsync(text);
        }
    }
}
