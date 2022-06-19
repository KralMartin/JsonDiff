using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace JsonDiff.Core
{
    /// <summary>
    /// Class for binarry compressed serialization and deserialization.
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// Serializes provided <paramref name="obj"/> into an array of <see cref="byte"/>s.
        /// </summary>
        /// <param name="obj">An object to be serialized.</param>
        /// <returns>
        /// Returns binary representation of serialized <paramref name="obj"/>.
        /// </returns>
        public static byte[] ToByteArray(object obj)
        {
            using var toReturn = new MemoryStream();
            using (DeflateStream compresStream = new DeflateStream(toReturn, CompressionLevel.Optimal))
                new BinaryFormatter().Serialize(compresStream, obj);
            return toReturn.ToArray();
        }

        /// <summary>
        /// Deserializes provided array of <paramref name="bytes"/> into specified type.
        /// </summary>
        /// <typeparam name="T">Type of result.</typeparam>
        /// <param name="bytes">Bytes from which result is created.</param>
        /// <returns>
        /// Returns deserialized object.
        /// </returns>
        public static T FromByteArray<T>(byte[] bytes)
        {
            using var memStreamInput = new MemoryStream(bytes);
            using var compresStream = new DeflateStream(memStreamInput, CompressionMode.Decompress);
            return (T)new BinaryFormatter().Deserialize(compresStream);
        }
    }
}
