using System.Collections.Generic;

namespace JsonDiff.DataObjects
{
    public class DiffResponse
    {
        public bool AreEqual { get; set; }
        public string Message { get; set; }
        public List<SubArrayIdentifier> DifferenceOffsets { get; set; }
    }
}
