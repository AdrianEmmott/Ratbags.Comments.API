using Comments.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Ratbags.Comments.API.Controllers;
using Ratbags.Core.DTOs.Articles;
using Ratbags.Core.Models.Articles;
using System.Net;

namespace Ratbags.Comments.API.Tests;

[TestFixture]
public class CommentsControllerTests
{
    private Mock<IService> _mockService;
    private Mock<ILogger<Controllers.CommentsController>> _mockLogger;

    private Controllers.CommentsController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<IService>();
        _mockLogger = new Mock<ILogger<Controllers.CommentsController>>();
        _controller = new Controllers.CommentsController(_mockService.Object, _mockLogger.Object);
    }

    // DELETE
    [Test]
    public async Task Delete_NoContent()
    {
        // arrange
        var id = Guid.NewGuid();
        
        _mockService.Setup(s => s.DeleteAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        // act
        var result = await _controller.Delete(id);

        // assert
        Assert.That(result, Is.TypeOf<NoContentResult>());
    }

    [Test]
    public async Task Delete_NotFound()
    {
        // arrange
        var id = Guid.NewGuid();
        
        _mockService.Setup(s => s.DeleteAsync(It.IsAny<Guid>()))
            .ReturnsAsync(false);

        // act
        var result = await _controller.Delete(id);

        // assert
        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task Delete_Exception()
    {
        // arrange
        var id = Guid.Empty;
        
        _mockService.Setup(s => s.DeleteAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new Exception("test exception"));

        // act
        var result = await _controller.Delete(id);

        // assert
        var statusCodeResult = result as ObjectResult;
        Assert.That(statusCodeResult, Is.Not.Null);
        Assert.That(statusCodeResult.StatusCode, 
            Is.EqualTo((int)HttpStatusCode.InternalServerError));

        Assert.That(statusCodeResult.Value, 
            Is.EqualTo("An error occurred while deleting the comment"));
    }


    // GET/{ID}
    [Test]
    public async Task GetById_Ok()
    {
        // arrange
        var dto = new CommentDTO 
        { 
            Id = Guid.NewGuid()
        };

        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(dto);

        // act
        var result = await _controller.Get(dto.Id);

        // assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(dto));
    }

    [Test]
    public async Task GetById_NotFound()
    {
        // arrange
        var id = Guid.NewGuid();

        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(null as CommentDTO);

        // act
        var result = await _controller.Get(id);

        // assert
        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }


    // GET/ARTICLE/{ID}
    [Test]
    public async Task GetByArticleId_Ok()
    {
        // arrange
        var articleId = Guid.NewGuid();

        var dtoList = new List<CommentDTO>
        {
            new CommentDTO {
                Id = Guid.NewGuid(),
                ArticleId = articleId,
                Content = "Test Comment"
            },
            new CommentDTO {
                Id = Guid.NewGuid(),
                ArticleId = articleId,
                Content = "Test Comment 2"
            }
        };

        _mockService.Setup(s => s.GetByArticleIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(dtoList);

        // act
        var result = await _controller.GetByArticleId(articleId);

        // assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());

        // assert ok has correct comments
        var okResult = result as OkObjectResult;
        Assert.That(okResult.Value, Has.Count.EqualTo(2));
        Assert.That(okResult.Value, Is.EqualTo(dtoList));
    }

    [Test]
    public async Task GetByArticleId_NotFound()
    {
        // arrange
        var articleId = Guid.NewGuid();

        _mockService.Setup(s => s.GetByArticleIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((List<CommentDTO>)null);

        // act
        var result = await _controller.GetByArticleId(articleId);

        // assert
        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }


    // POST
    [Test]
    public async Task Post_Created()
    {
        // arrange
        var model = new CreateCommentModel 
        { 
            ArticleId = Guid.NewGuid(), 
            Content = "New Comment" 
        };

        var newId = Guid.NewGuid();

        _mockService.Setup(s => s.CreateAsync(It.IsAny<CreateCommentModel>()))
            .ReturnsAsync(newId);

        // act
        var result = await _controller.Post(model);

        // assert
        Assert.That(result, Is.TypeOf<CreatedAtActionResult>());
        var createdResult = result as CreatedAtActionResult;

        // assert action name correct
        Assert.That(createdResult.ActionName, 
            Is.EqualTo(nameof(CommentsController.GetByArticleId)));

        // assert route values correct id
        Assert.That(createdResult.RouteValues.ContainsKey("id"));
        Assert.That(createdResult.RouteValues["id"], Is.EqualTo(newId));

        // assert returned value is the new id
        Assert.That(createdResult.Value, Is.EqualTo(newId));
    }

    [Test]
    public async Task Post_BadRequest()
    {
        // arrange
        _mockService.Setup(s => s.CreateAsync(It.IsAny<CreateCommentModel>()))
                   .ReturnsAsync(Guid.Empty);

        var model = new CreateCommentModel
        {
            Content = "yak",
            Published = DateTime.Now,
        };

        // act 
        var result = await _controller.Post(model);

        // assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);

        // assert correct status
        Assert.That(badRequestResult.StatusCode, 
            Is.EqualTo((int)HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Post_Exception()
    {
        // arrange
        _mockService.Setup(s => s.CreateAsync(It.IsAny<CreateCommentModel>()))
            .ThrowsAsync(new Exception("test exception"));

        var model = new CreateCommentModel
        {
            Content = "<p>lorem ipsum</p>",
            Published = DateTime.Now,
        };

        // act
        var result = await _controller.Post(model);

        // assert
        var statusCodeResult = result as ObjectResult;
        Assert.That(statusCodeResult, Is.Not.Null);

        Assert.That(statusCodeResult.StatusCode, 
            Is.EqualTo((int)HttpStatusCode.InternalServerError));

        Assert.That(statusCodeResult.Value, 
            Is.EqualTo("An error occurred while creating the comment"));
    }


    // PUT
    [Test]
    public async Task Put_NoContent()
    {
        // arrange
        var model = new UpdateCommentModel
        { 
            Id = Guid.NewGuid(), 
            Content = "Updated Comment" 
        };

        _mockService.Setup(s => s.UpdateAsync(It.IsAny<UpdateCommentModel>()))
            .ReturnsAsync(true);

        // act
        var result = await _controller.Put(model);

        // assert
        Assert.That(result, Is.TypeOf<NoContentResult>());
    }

    [Test]
    public async Task Put_NotFound()
    {
        // arrange
        var model = new UpdateCommentModel 
        { 
            Id = Guid.NewGuid(), 
            Content = "Updated Comment" 
        };

        _mockService.Setup(s => s.UpdateAsync(It.IsAny<UpdateCommentModel>()))
            .ReturnsAsync(false);

        // act
        var result = await _controller.Put(model);

        // assert
        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task Put_Exception()
    {
        // arrange
        _mockService.Setup(s => s.UpdateAsync(It.IsAny<UpdateCommentModel>()))
            .ThrowsAsync(new Exception("test exception"));

        var model = new UpdateCommentModel
        {
            Content = "<p>lorem ipsum</p>",
            Published = DateTime.Now,
        };

        // act
        var result = await _controller.Put(model);

        // assert
        var statusCodeResult = result as ObjectResult;
        Assert.That(statusCodeResult, Is.Not.Null);

        Assert.That(statusCodeResult.StatusCode, 
            Is.EqualTo((int)HttpStatusCode.InternalServerError));

        Assert.That(statusCodeResult.Value, 
            Is.EqualTo("An error occurred while updating the comment"));
    }
}
