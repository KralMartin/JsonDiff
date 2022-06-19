using JsonDiff.Core;
using JsonDiff.DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonDiffTest.Core
{
    [TestClass]
    public class SerializerTest
    {
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("qwe")]
        public void RoundTrip(string input)
        {
            byte[] bytes = Serializer.ToByteArray(new JsonFile { Input = input });
            JsonFile output = Serializer.FromByteArray<JsonFile>(bytes);
            Assert.AreEqual(input, output.Input);
        }
    }
}
