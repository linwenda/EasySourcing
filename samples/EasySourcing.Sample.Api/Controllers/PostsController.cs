using EasySourcing.Sample.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EasySourcing.Sample.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IPostQueries _postQueries;

        public PostsController(IPostService postService, IPostQueries postQueries)
        {
            _postService = postService;
            _postQueries = postQueries;
        }

        [HttpGet]
        public async Task<IActionResult> GetPostsAsync()
        {
            return Ok(await _postQueries.GetPostsAsync());
        }

        [HttpGet("{postId:guid}/histories")]
        public async Task<IActionResult> GetPostHistoriesAsync(Guid postId)
        {
            return Ok(await _postQueries.GetPostHistoriesAsync(postId));
        }

        [HttpPost]
        public async Task<IActionResult> CreatePostAsync([FromBody] CreatePostModel model)
        {
            await _postService.CreatePostAsync(model.AuthorId, model.Title, model.Content);
            return Ok();
        }

        [HttpPut("{postId:guid}")]
        public async Task<IActionResult> EditPostAsync([FromRoute] Guid postId, [FromBody] EditPostModel model)
        {
            await _postService.EditPostAsync(postId, model.Title, model.Content);
            return Ok();
        }
    }
}