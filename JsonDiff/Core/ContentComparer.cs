using JsonDiff.DataObjects;
using System.Collections.Generic;
using System.Linq;

namespace JsonDiff.Core
{
    /// <summary>
    /// Compares strings.
    /// </summary>
    public interface IContentComparer
    {
        /// <summary>
        /// Compares <paramref name="str1"/> to <paramref name="str2"/>.
        /// </summary>
        /// <param name="str1">A string to be compared.</param>
        /// <param name="str2">A string to be compared.</param>
        /// <returns>
        /// Returns new instance of<see cref="ComparisonResult"/>.
        /// </returns>
        ComparisonResult Compare(string str1, string str2);
    }

    /// <inheritdoc />
    public class ContentComparer : IContentComparer
    {
        /// <inheritdoc />
        public ComparisonResult Compare(string str1, string str2)
        {
            str1 ??= string.Empty;
            str2 ??= string.Empty;
            if (str1.Length != str2.Length)
            {
                return new ComparisonResult
                {
                    AreEqual = false,
                    AreSameLength = false
                };
            }

            var offsets = new List<SubArrayIdentifier>();
            int charsSinceLastMatch = 0;
            for (int i = 0; i < str1.Length; i++)
            {
                if (str1[i] == str2[i])
                {
                    if (charsSinceLastMatch > 0)
                    {
                        offsets.Add(
                            new SubArrayIdentifier(i - charsSinceLastMatch, charsSinceLastMatch));
                        charsSinceLastMatch = 0;
                    }
                }
                else
                {
                    charsSinceLastMatch++;
                }
            }
            if (charsSinceLastMatch > 0)
            {
                offsets.Add(
                    new SubArrayIdentifier(str1.Length - charsSinceLastMatch, charsSinceLastMatch));
            }
            if (offsets.Any())
            {
                return new ComparisonResult
                {
                    AreEqual = false,
                    AreSameLength = true,
                    DifferenceOffsets = offsets
                };
            }
            return new ComparisonResult
            {
                AreEqual = true,
                AreSameLength = true
            };
        }
    }
}
