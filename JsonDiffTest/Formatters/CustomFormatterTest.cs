using JsonDiff.Controllers;
using JsonDiff.DataObjects;
using JsonDiff.Formatters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JsonDiffTest.Formatters
{
    [TestClass]
    public class CustomFormatterTest
    {
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("testValue")]
        public void RoundTrip(string input)
        {
            var memStream = Serialize(new JsonFile { Input = input });
            memStream.Flush();
            memStream.Position = 0;
            JsonFile output = Deserialize(memStream);

            Assert.AreEqual(input, output.Input);
        }

        private static JsonFile Deserialize(Stream requestBody)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Body = requestBody;
            var context = new InputFormatterContext(
                httpContext,
                nameof(JsonFile),
                new ModelStateDictionary(),
                new EmptyModelMetadataProvider().GetMetadataForType(typeof(object)),
                (stream, encoding) => new StreamReader(stream, encoding));

            var inputFormatter = new CustomInputFormatter();
            InputFormatterResult result = inputFormatter
                .ReadRequestBodyAsync(context, Encoding.UTF8)
                .Result;
            return (JsonFile)result.Model;
        }

        private static Stream Serialize(JsonFile jsonFile)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();

            var inputContext = new OutputFormatterWriteContext(
                httpContext,
                (stream, encoding) => new HttpResponseStreamWriter(stream, encoding),
                typeof(string),
                jsonFile
                );

            var outputFormatter = new CustomOutputFormatter();
            outputFormatter.WriteResponseBodyAsync(inputContext, Encoding.UTF8).Wait();
            return inputContext.HttpContext.Response.Body;
        }
    }
}
