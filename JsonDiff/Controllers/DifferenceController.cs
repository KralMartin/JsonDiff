using JsonDiff.Core;
using JsonDiff.DataObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JsonDiff.Controllers
{
    /// <summary>
    /// Controller covering 'diff' feature of <see cref="JsonFile"/>s.
    /// </summary>
    [ApiController]
    [Route("v{version:apiVersion}/diff/{id}/[action]")]
    [ApiVersion("1")]
    [ApiVersion("1")]    
    public class DifferenceController : ControllerBase
    {
        private readonly IContentComparer _comparer;
        private readonly IFilesDb _filesDb;

        public DifferenceController(IFilesDb filesDb, IContentComparer comparer)
        {
            _comparer = comparer;
            _filesDb = filesDb;
        }

        /// <summary>
        /// Persists provided <paramref name="file"/> on <paramref name="id"/>
        /// as <see cref="Side.Right"/> side for comparison.
        /// </summary>
        /// <param name="id">Identifier of inserted object.</param>
        /// <param name="file">Content to be inserted.</param>
        /// <returns>
        /// Returns an object, that describes result of insertion.
        /// </returns>
        [HttpPost]
        [ActionName("right")]
        public IActionResult PostRight(string id, JsonFile file)
        {
            return Insert(id, Side.Right, file);
        }

        /// <summary>
        /// Persists provided <paramref name="file"/> on <paramref name="id"/>
        /// as <see cref="Side.Left"/> side for comparison.
        /// </summary>
        /// <param name="id">Identifier of inserted object.</param>
        /// <param name="file">Content to be inserted.</param>
        /// <returns>
        /// Returns an object, that describes result of insertion.
        /// </returns>
        [HttpPost]
        [ActionName("left")]
        public IActionResult PostLeft(string id, JsonFile file)
        {
            return Insert(id, Side.Left, file);
        }

        /// <summary>
        /// Returns description of difference of <see cref="Side.Left"/> and <see cref="Side.Right"/> files on provided <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Identifier of files to be compared.</param>
        /// <returns>
        /// Returns and object, that describes result of difference.
        /// </returns>
        [HttpGet]
        [ActionName("")]
        public IActionResult Diff(string id)
        {
            //Response.Headers.Add("Content-Type", "application/custom");
            bool foundLeft = _filesDb.TryGet(id, Side.Left, out byte[] contentLeft);
            bool foundRight = _filesDb.TryGet(id, Side.Right, out byte[] contentRight);
            if (!foundLeft && !foundRight)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    $"No files to be compared for id '{id}'.");
            }
            if (!foundRight)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    $"No '{Side.Right}' file to be compared for id '{id}'.");
            }
            if (!foundLeft)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    $"No '{Side.Left}' file to be compared for id '{id}'.");
            }

            ComparisonResult result = _comparer.Compare(
                Serializer.FromByteArray<JsonFile>(contentRight).Input,
                Serializer.FromByteArray<JsonFile>(contentLeft).Input);
            return Ok(new DiffResponse
            {
                AreEqual = result.AreEqual,
                Message = GetMessage(result),
                DifferenceOffsets = result.DifferenceOffsets
            });        
        }

        private static string GetMessage(ComparisonResult value)
        {
            if (value.AreEqual)
                return "Inputs were equal.";
            if (value.AreSameLength)
                return "Inputs are of same size.";
            return "Inputs are of different size.";
        }

        private IActionResult Insert(string id, Side side, JsonFile file)
        {
            if (_filesDb.Insert(id, side, Serializer.ToByteArray(file)))
                return StatusCode(StatusCodes.Status201Created, "Object was successfully inserted.");
            else
                return StatusCode(StatusCodes.Status409Conflict, $"There is already an object '{id}'.");
        }
    }
}
