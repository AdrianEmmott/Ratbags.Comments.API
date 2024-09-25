using Comments.API.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Ratbags.Shared.DTOs.Events.DTOs.Articles.Comments;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Ratbags.Comments.API.Controllers
{
    [ApiController]
    [Route("api/comments")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentsService _service;
        private readonly ILogger<CommentsController> _logger;

        public CommentsController(ICommentsService service
            , ILogger<CommentsController> logger)
        {
            _service = service;
            _logger = logger;
        }


        [HttpPost("create")]
        [ProducesResponseType((int)HttpStatusCode.BadGateway)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [SwaggerOperation(Summary = "Adds a comment to an article", Description = "The article id must exist!")]
        public async Task<IActionResult> Create(CreateCommentDTO commentDTO)
        {
            try
            {
                var commentId = await _service.CreateCommentAsync(commentDTO);

                if (commentId != Guid.Empty)
                {
                    return Created(nameof(GetCommentById), commentId);
                }

                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogInformation($"error - add comments to article {commentDTO.ArticleId}: {e.Message}");
                return BadRequest(e.Message);
                throw;
                throw;
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]        
        [ProducesResponseType(typeof(CommentDTO), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Summary = "Gets a comment by id", Description = "Retrieves a specific comment by its unique id")]
        public async Task<IActionResult> GetCommentById(Guid id)
        {
            try
            {
                var comment = await _service.GetCommentByIdAsync(id);

                if (comment != null)
                {
                    return Ok(comment);
                }

                return NotFound();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
                throw;
            }  
        }


        [HttpGet("article/{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CommentDTO), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Summary = "Gets all comments by article id", Description = "Retrieves all comments by article id")]
        public async Task<IActionResult> GetCommentsByArticleId(Guid id)
        {
            try
            {
                _logger.LogInformation($"get comments for article {id}");
                var comments = await _service.GetCommentsByArticleIdAsync(id);

                if (comments != null)
                {
                    return Ok(comments);
                }

                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogInformation($"error - get comments for article {id}: {e.Message}");
                return BadRequest(e.Message);
                throw;
            }
        }
    }
}
