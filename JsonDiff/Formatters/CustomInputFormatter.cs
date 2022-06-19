using JsonDiff.DataObjects;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace JsonDiff.Formatters
{
    public class CustomInputFormatter : TextInputFormatter
    {
        public CustomInputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/custom"));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            try
            {
                //Read base64 content.
                string text;
                using (StreamReader reader = new StreamReader(context.HttpContext.Request.Body))
                    text = await reader.ReadToEndAsync();
                text = text.Trim('\"');

                //Convert to UTF8 string.
                byte[] bytes = Convert.FromBase64String(text);
                text = Encoding.UTF8.GetString(bytes);

                //Convert to Json.
                JsonFile model = JsonConvert.DeserializeObject<JsonFile>(text);
                return InputFormatterResult.Success(model);
            }
            catch
            {
                return InputFormatterResult.Failure();
            }
        }
    }
}
