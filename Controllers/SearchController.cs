using DoAn4.Services.SearchService;
using Microsoft.AspNetCore.Mvc;

namespace DoAn4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService) 
        {
            _searchService = searchService;
        }

        [HttpPost]
        public async Task<IActionResult> Search([FromForm] string keyword)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _searchService.Search(token, keyword);
            if (result == null) 
            {
                return NotFound();
            }
            return Ok(result);

        }
    }
}
