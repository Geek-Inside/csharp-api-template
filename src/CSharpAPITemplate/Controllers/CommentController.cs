using CSharpAPITemplate.BusinessLayer.Models;
using CSharpAPITemplate.BusinessLayer.Services.Comments;
using Microsoft.AspNetCore.Mvc;

namespace CSharpAPITemplate.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("comments")]
    public class CommentController : BaseController<CommentDto>
    {
        public CommentController(
            ICommentService commentService,
            ILogger<CommentController> logger) : base(commentService, logger)
        { }
    }
}
