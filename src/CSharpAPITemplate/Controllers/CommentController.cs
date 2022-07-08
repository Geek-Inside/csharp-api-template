using CSharpAPITemplate.BusinessLayer.Models;
using CSharpAPITemplate.BusinessLayer.Services.Posts;
using Microsoft.AspNetCore.Mvc;

namespace CSharpAPITemplate.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("posts")]
    public class PostController : BaseController<PostDto>
    {
        public PostController(
            IPostService postService,
            ILogger<PostController> logger) : base(postService, logger)
        { }
    }
}
