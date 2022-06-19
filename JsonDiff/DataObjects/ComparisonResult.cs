using System.Collections.Generic;

namespace JsonDiff.DataObjects
{
    /// <summary>
    /// Result of comparison of two objects.
    /// </summary>
    public class ComparisonResult
    {
        /// <summary>
        /// Says whether compared objects were equal.
        /// </summary>
        public bool AreEqual { get; set; }

        /// <summary>
        /// Says whether compared objects were of same length.
        /// </summary>
        public bool AreSameLength { get; set; }

        /// <summary>
        /// A <see cref="List{T}"/> that describes where the differences were found.
        /// </summary>
        public List<SubArrayIdentifier> DifferenceOffsets { get; set; }
    }
}
