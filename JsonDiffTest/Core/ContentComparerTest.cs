using JsonDiff.Core;
using JsonDiff.DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JsonDiffTest.Core
{
    [TestClass]
    public class ContentComparerTest
    {
        [TestMethod]
        [DataRow(null, null, true)]
        [DataRow(null, "", true)]
        [DataRow("", "", true)]
        [DataRow("qwe", "qwe", true)]
        public void Compare_AreEqualInLen(string left, string right, bool expectedAreEqual)
        {
            Compare_AreEqualInLenImpl(left, right, expectedAreEqual);
            Compare_AreEqualInLenImpl(right, left, expectedAreEqual);
        }

        private static void Compare_AreEqualInLenImpl(string left, string right, bool expectedAreEqual)
        {
            //Arrange
            var comparer = new ContentComparer();

            //Act
            ComparisonResult result = comparer.Compare(left, right);

            //Assert
            Assert.IsTrue(result.AreEqual);
            Assert.IsTrue(result.AreSameLength);
            Assert.IsNull(result.DifferenceOffsets);
        }

        [TestMethod]
        [DataRow(null, "a", true)]
        [DataRow("qwe", "zqwe", true)]
        public void Compare_AreNotEqualInLen(string left, string right, bool expectedAreEqual)
        {
            Compare_AreNotEqualInLenImp(left, right, expectedAreEqual);
            Compare_AreNotEqualInLenImp(right, left, expectedAreEqual);
        }

        public void Compare_AreNotEqualInLenImp(string left, string right, bool expectedAreEqual)
        {
            //Arrange
            var comparer = new ContentComparer();

            //Act
            ComparisonResult result = comparer.Compare(left, right);

            //Assert
            Assert.IsFalse(result.AreEqual);
            Assert.IsFalse(result.AreSameLength);
            Assert.IsNull(result.DifferenceOffsets);
        }

        [TestMethod]
        [DataRow("qwe", "asd", "0-3")]
        [DataRow("qwe", "qwX", "2-1")]
        [DataRow("qwe", "Xwe", "0-1")]
        [DataRow("qweAsdqwe", "qweXXXqwe", "3-3")]
        [DataRow("qweAsdqwe", "xxxAsdxxx", "0-3,6-3")]
        [DataRow("aXbbXcX", "qXwwXeX", "0-1,2-2,5-1")]
        [DataRow("QWE", "qwe", "0-3")]
        public void Compare_TestDifferenceOffsets(string left, string right, string expectedResult)
        {
            Compare_TestDifferenceOffsetsImp(left, right, expectedResult);
            Compare_TestDifferenceOffsetsImp(right, left, expectedResult);
        }

        public void Compare_TestDifferenceOffsetsImp(string left, string right, string expectedResult)
        {
            //Arrange
            var comparer = new ContentComparer();

            //Act
            ComparisonResult result = comparer.Compare(left, right);
            
            //Assert
            Assert.IsFalse(result.AreEqual);
            Assert.IsTrue(result.AreSameLength);

            string resultOffsets = string.Join(",",
                result.DifferenceOffsets
                .Select(x => $"{x.Offset}-{x.Length}"));
            Assert.AreEqual(expectedResult, resultOffsets);
        }
    }
}
