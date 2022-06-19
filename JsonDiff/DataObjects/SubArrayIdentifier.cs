namespace JsonDiff.DataObjects
{
    /// <summary>
    /// Addresses an subarray within an array.
    /// </summary>
    public struct SubArrayIdentifier
    {
        /// <summary>
        /// Starting point of a subarray.
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// Length of a subarray.
        /// </summary>
        public int Length { get; }

        public SubArrayIdentifier(int offset, int length)
        {
            Offset = offset;
            Length = length;
        }
    }
}
