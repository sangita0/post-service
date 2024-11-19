using Microsoft.AspNetCore.Mvc;
using post_microservice.Model;
using post_microservice.Services;

namespace post_microservice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;

        public PostController(PostService postService)
        {
            _postService = postService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromForm] string userId, [FromForm] string content, [FromForm] List<IFormFile> mediaFiles)
        {
            var post = new Post
            {
                UserId = userId,
                Content = content
            };

            // Pass both `post` and `mediaFiles` to `CreatePostAsync` as required
            var createdPost = await _postService.CreatePostAsync(post, mediaFiles);
            return CreatedAtAction(nameof(GetPostById), new { postId = createdPost.PostId }, createdPost);
        }

        [HttpGet("{postId}")]
        public async Task<IActionResult> GetPostById(string postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }

        [HttpPut("{postId}")]
        public async Task<IActionResult> UpdatePost(string postId, [FromBody] string content)
        {
            var updatedPost = await _postService.UpdatePostAsync(postId, content);
            return Ok(updatedPost);
        }

        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost(string postId)
        {
            await _postService.DeletePostAsync(postId);
            return NoContent();
        }
    }
}
