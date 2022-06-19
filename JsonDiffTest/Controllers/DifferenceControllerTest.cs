using JsonDiff.Controllers;
using JsonDiff.Core;
using JsonDiff.DataObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonDiffTest.Controllers
{
    [TestClass]
    public class DifferenceControllerTest
    {
        [TestMethod]
        [DataRow(true, StatusCodes.Status201Created)]
        [DataRow(false, StatusCodes.Status409Conflict)]
        public void PostRight(bool isInserted, int expectedStatusCode)
        {
            //Arrange
            var comparer = new Mock<IContentComparer>();
            var jsonFile = new JsonFile();
            byte[] expectedBytes = Serializer.ToByteArray(jsonFile);

            var fileDb = new Mock<IFilesDb>();
            fileDb.Setup(x => x.Insert("test", Side.Right, expectedBytes))
                .Returns(isInserted);
            var controller = new DifferenceController(fileDb.Object, comparer.Object);

            //Act
            ObjectResult res = (ObjectResult)controller.PostRight("test", jsonFile);

            //Assert
            Assert.AreEqual(expectedStatusCode, res.StatusCode);
            fileDb.Verify(x => x.Insert("test", Side.Right, expectedBytes), Times.Once());
        }

        [TestMethod]
        [DataRow(true, StatusCodes.Status201Created)]
        [DataRow(false, StatusCodes.Status409Conflict)]
        public void PostLeft(bool isInserted, int expectedStatusCode)
        {
            //Arrange
            var comparer = new Mock<IContentComparer>();
            var jsonFile = new JsonFile();
            byte[] expectedBytes = Serializer.ToByteArray(jsonFile);

            var fileDb = new Mock<IFilesDb>();
            fileDb.Setup(x => x.Insert("test", Side.Left, expectedBytes))
                .Returns(isInserted);
            var controller = new DifferenceController(fileDb.Object, comparer.Object);

            //Act
            ObjectResult res = (ObjectResult)controller.PostLeft("test", jsonFile);

            //Assert
            Assert.AreEqual(expectedStatusCode, res.StatusCode);
            fileDb.Verify(x => x.Insert("test", Side.Left, expectedBytes), Times.Once());
        }

        [TestMethod]
        [DataRow(false, false, "No files to be compared for id 'testid'.")]
        [DataRow(false, true, "No 'Left' file to be compared for id 'testid'.")]
        [DataRow(true, false, "No 'Right' file to be compared for id 'testid'.")]
        public void Diff_MissingFilesToCompare_ShouldReturn404(bool foundLeft, bool foundRight, string expectedMessage)
        {
            //Arrange
            var comparer = new Mock<IContentComparer>();
            var jsonFile = new JsonFile();
            byte[] expectedBytes = Serializer.ToByteArray(jsonFile);

            byte[] bytes = Serializer.ToByteArray(new JsonFile());
            var fileDb = new Mock<IFilesDb>();
            fileDb.Setup(x => x.TryGet("testid", Side.Left, out bytes))
                .Returns(foundLeft);
            fileDb.Setup(x => x.TryGet("testid", Side.Right, out bytes))
                .Returns(foundRight);

            var controller = new DifferenceController(fileDb.Object, comparer.Object);

            //Act
            ObjectResult res = (ObjectResult)controller.Diff("testid");

            //Assert
            Assert.AreEqual(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.AreEqual(expectedMessage, res.Value);
            comparer.Verify(x => x.Compare(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [TestMethod]
        [DataRow(false, false, "Inputs are of different size.")]
        [DataRow(false, true, "Inputs are of same size.")]
        [DataRow(true, true, "Inputs were equal.")]
        public void Diff_FilesPresent_ShouldReturn200(bool areEqual, bool areSameLength, string expectedMessage)
        {
            //Arrange
            string jsonText = "json1";
            var comparer = new Mock<IContentComparer>();
            comparer.Setup(x => x.Compare(jsonText, jsonText))
                .Returns(new ComparisonResult
                {
                    AreEqual = areEqual,
                    AreSameLength = areSameLength
                });

            var jsonFile = new JsonFile();
            byte[] expectedBytes = Serializer.ToByteArray(jsonFile);

            byte[] bytes = Serializer.ToByteArray(new JsonFile() { Input = jsonText });
            var fileDb = new Mock<IFilesDb>();
            fileDb.Setup(x => x.TryGet("testid", Side.Left, out bytes))
                .Returns(true);
            fileDb.Setup(x => x.TryGet("testid", Side.Right, out bytes))
                .Returns(true);

            var controller = new DifferenceController(fileDb.Object, comparer.Object);

            //Act
            ObjectResult res = (ObjectResult)controller.Diff("testid");
            DiffResponse response = (DiffResponse)res.Value;

            //Assert
            Assert.AreEqual(StatusCodes.Status200OK, res.StatusCode);
            Assert.AreEqual(expectedMessage, response.Message);
            comparer.Verify(x => x.Compare(jsonText, jsonText), Times.Once());
        }
    }
}
