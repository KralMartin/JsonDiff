using System.Data;
using System.Data.SqlClient;

namespace JsonDiff.Core
{
    public enum Side { Left = 0, Right = 1 }

    /// <summary>
    /// Contains logic to work with SQL queries on [Files] table. 
    /// </summary>
    public interface IFilesDb
    {
        /// <summary>
        /// Attempts to get an object on <paramref name="id"/> and <paramref name="side"/>.
        /// </summary>
        /// <param name="id">Identifier of an object.</param>
        /// <param name="side">Identifier of an object.</param>
        /// <param name="content">Content of retrieved object.</param>
        /// <returns>
        /// Returns value, that describes whether an object was successfully retieved or not.
        /// </returns>
        bool TryGet(string id, Side side, out byte[] content);

        /// <summary>
        /// Attempts to insert provided <paramref name="content"/> on <paramref name="id"/> and <paramref name="side"/>.
        /// </summary>
        /// <param name="id">Identifier of an object.</param>
        /// <param name="side">Identifier of an object.</param>
        /// <param name="content">Content of an object.</param>
        /// <returns>
        /// Returns value, that describes whether an object was successfully inserted or not.
        /// </returns>
        bool Insert(string id, Side side, byte[] content);
    }

    /// <inheritdoc />
    public class FilesDb : IFilesDb
    {
        private readonly string _conStr;

        public FilesDb(string conStr)
        {
            _conStr = conStr;
        }

        /// <inheritdoc />
        public bool TryGet(string id, Side side, out byte[] content)
        {
            using var con = new SqlConnection(_conStr);
            con.Open();
            using var com = new SqlCommand("[dbo].[GetFile]", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.Add("id", SqlDbType.Char).Value = id;
            com.Parameters.Add("side", SqlDbType.Bit).Value = side;

            SqlDataReader reader = com.ExecuteReader();
            if (reader.Read())
            {
                content = (byte[])reader[0];
                return true;
            }
            content = null;
            return false;
        }

        /// <inheritdoc />
        public bool Insert(string id, Side side, byte[] content)
        {
            using var con = new SqlConnection(_conStr);
            con.Open();
            using var com = new SqlCommand("[dbo].[InsertFile]", con);
            try
            {
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("id", SqlDbType.Char).Value = id;
                com.Parameters.Add("side", SqlDbType.Bit).Value = side;
                com.Parameters.Add("@content", SqlDbType.VarBinary).Value = content;

                com.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex) when (ex.Number == 2627)
            {
                return false;
            }
        }
    }
}
