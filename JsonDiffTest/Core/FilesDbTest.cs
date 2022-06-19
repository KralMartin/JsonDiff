using JsonDiff.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace JsonDiffTest.Core
{
    [TestClass]
    [TestCategory("Integration")]
    public class FilesDbTest
    {
        private const string ConStr = "Server=(LocalDb)\\MSSQLLocalDB;Database=JsonDiff;Trusted_Connection=True;";

        [ClassInitialize]
        public static void Init(TestContext tt)
            => Cleanup();

        [ClassCleanup]
        public static void Cleanup()
        {
            string command = "DELETE FROM [dbo].[Files] WHERE Id like 'test_%'";
            using (var con = new SqlConnection(ConStr))
            {
                con.Open();
                using (var com = new SqlCommand(command, con))
                {
                    com.ExecuteNonQuery();
                }
            }
        }

        [TestMethod]
        public void Get_WhenRecordExists_ShouldReturnTrue()
        {
            //Arange
            var db = new FilesDb(ConStr);
            var insertedBytes = new byte[] { 0x12, 0x34 };
            db.Insert("test_1", Side.Left, insertedBytes);

            //Act
            bool recordExists = db.TryGet("test_1", Side.Left, out byte[] returnedBytes);

            //Assert
            Assert.IsTrue(recordExists);
            Assert.IsTrue(insertedBytes.SequenceEqual(returnedBytes));
        }

        [TestMethod]
        public void Get_WhenRecordNotExists_ShouldReturnFalse()
        {
            //Arange
            var db = new FilesDb(ConStr);

            //Act
            bool recordExists = db.TryGet("test_6", Side.Left, out byte[] returnedBytes);

            //Assert
            Assert.IsFalse(recordExists);
        }

        [TestMethod]
        public void Insert_DuplicateRecords_ShouldThrow()
        {
            //Arange
            var db = new FilesDb(ConStr);
            var insertedBytes = new byte[] { 0x12, 0x34 };

            //Act / Assert
            Assert.IsTrue(db.Insert("test_2", Side.Left, insertedBytes));
            Assert.IsFalse(db.Insert("test_2", Side.Left, insertedBytes));
        }
    }
}
