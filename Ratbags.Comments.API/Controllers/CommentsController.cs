using Comments.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ratbags.Comments.API.Models.API;
using Ratbags.Core.DTOs.Articles;
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

        public CommentsController(
            ICommentsService service,
            ILogger<CommentsController> logger)
        {
            _service = service;
            _logger = logger;
        }


        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [SwaggerOperation(Summary = "Deletes a comment by id", Description = "Deletes a comment by id")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);

                return result ? NoContent() : NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error deleting comment {id}: {e.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while deleting the comment");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CommentDTO), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Summary = "Gets a comment by id", Description = "Returns a specific comment by id")]
        public async Task<IActionResult> Get(Guid id)
        {
            var comment = await _service.GetByIdAsync(id);

            return comment == null ? NotFound() : Ok(comment);
        }

        [HttpGet("article/{id}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CommentDTO), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Summary = "Gets all comments by article id", Description = "Returns all comments for an article or an empty list if no data")]
        public async Task<IActionResult> GetByArticleId(Guid id)
        {
            var comments = await _service.GetByArticleIdAsync(id);

            return comments == null ? NotFound() : Ok(comments);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [SwaggerOperation(Summary = "Adds a comment to an article", Description = "The article id must exist")]
        public async Task<IActionResult> Post(CommentCreate model)
        {
            try
            {
                var commentId = await _service.CreateAsync(model);

                return commentId != Guid.Empty ? BadRequest("Failed to create comment") : CreatedAtAction(nameof(GetByArticleId), new { id = commentId }, commentId);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error creating comment for article {model.ArticleId}: {e.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"An error occurred while creating the comment");
            }
        }

        [HttpPut]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [SwaggerOperation(Summary = "Updates a comment ", Description = "Updates a comment")]
        public async Task<IActionResult> Put([FromBody] CommentUpdate model)
        {
            try
            {
                var result = await _service.UpdateAsync(model);

                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error updating comment {model.Id} for article {model.ArticleId}: {e.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while updating the comment");
            }
        }
    }
}
