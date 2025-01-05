//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Moq;
//using Ratbags.Comments.API.Interfaces;
//using Ratbags.Comments.API.Models.API;
//using Ratbags.Comments.API.Models.DB;
//using Ratbags.Comments.API.Services;

//namespace Ratbags.Comments.API.Tests;

//public class ServiceTests
//{
//    private Mock<ICommentsRepository> _mockRepository;
//    private Mock<ILogger<CommentsService>> _mockLogger;
//    private Mock<IMassTransitService> _mockMassTransitService;

//    private CommentsService _service;

//    [SetUp]
//    public void SetUp()
//    {
//        _mockRepository = new Mock<ICommentsRepository>();
//        _mockLogger = new Mock<ILogger<CommentsService>>();
//        _mockMassTransitService = new Mock<IMassTransitService>();

//        _service = new CommentsService(
//                                _mockRepository.Object,
//                                _mockMassTransitService.Object,
//                                _mockLogger.Object);
//    }

//    // CREATE
//    [Test]
//    public async Task CreateAsync_Success()
//    {
//        // arrange
//        var articleId = Guid.NewGuid();
//        var model = new CommentCreate
//        {
//            ArticleId = articleId,
//            Content = "<p>lorem ipsum</p>",
//            Published = DateTime.Now,
//        };

//        var id = Guid.NewGuid();

//        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Comment>()))
//                       .ReturnsAsync(id);

//        // act
//        var result = await _service.CreateAsync(model);

//        // assert
//        Assert.That(result, Is.EqualTo(id));
//        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Comment>()), Times.Once);
//    }


//    // DELETE
//    [Test]
//    public async Task DeleteAsync_Success()
//    {
//        // arrange
//        var id = Guid.NewGuid();

//        var model = new Comment
//        {
//            Id = id,
//            CommentContent = "Test"
//        };

//        _mockRepository.Setup(r => r.GetByIdAsync(id))
//                       .ReturnsAsync(model);

//        // act
//        var result = await _service.DeleteAsync(id);

//        // assert
//        Assert.That(result, Is.True);
//        _mockRepository.Verify(r => r.GetByIdAsync(id), Times.Once);
//        _mockRepository.Verify(r => r.DeleteAsync(id), Times.Once);
//    }

//    [Test]
//    public async Task DeleteAsync_Fail()
//    {
//        // arrange
//        var id = Guid.NewGuid();

//        _mockRepository.Setup(r => r.GetByIdAsync(id))
//                       .ReturnsAsync(null as Comment);

//        // act
//        var result = await _service.DeleteAsync(id);

//        // assert
//        Assert.That(result, Is.False);
//        _mockRepository.Verify(r => r.GetByIdAsync(id), Times.Once);
//        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
//    }

//    [Test]
//    public void DeleteAsync_Exception()
//    {
//        // arrange
//        var id = Guid.NewGuid();

//        var model = new Comment
//        {
//            Id = id,
//            CommentContent = "Test comment"
//        };

//        _mockRepository.Setup(r => r.GetByIdAsync(id))
//                       .ReturnsAsync(model);

//        _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<Guid>()))
//                       .ThrowsAsync(new DbUpdateException("Error deleting comment"));

//        // play interrupted - act / assert
//        var ex = Assert.ThrowsAsync<DbUpdateException>(() => _service.DeleteAsync(id));
//        Assert.That(ex.Message, Is.EqualTo("Error deleting comment"));
//    }


//    // GET ONE
//    [Test]
//    public async Task GetByIdAsync_Success()
//    {
//        // arrange
//        var id = Guid.NewGuid();
//        var article = new Comment
//        {
//            Id = id,
//            CommentContent = "test"
//        };

//        _mockRepository.Setup(r => r.GetByIdAsync(id))
//                       .ReturnsAsync(article);

//        // act
//        var result = await _service.GetByIdAsync(id);

//        // assert
//        Assert.That(result, Is.Not.Null);
//        Assert.That(result.Id, Is.EqualTo(id));

//        _mockRepository.Verify(r => r.GetByIdAsync(id), Times.Once);
//    }

//    [Test]
//    public async Task GetByIdAsync_Fail_NotFound()
//    {
//        // arrange
//        var id = Guid.NewGuid();

//        _mockRepository.Setup(r => r.GetByIdAsync(id))
//                       .ReturnsAsync(null as Comment);

//        // act
//        var result = await _service.GetByIdAsync(id);

//        // assert
//        Assert.That(result, Is.Null);
//        _mockRepository.Verify(r => r.GetByIdAsync(id), Times.Once);
//    }


//    // GET MANY BY ARTICLE
//    [Test]
//    public async Task GetByArticleIdAsync_Success()
//    {
//        // arrange
//        var articleId = Guid.NewGuid();

//        var modelList = new List<Comment>
//        {
//            new Comment {
//                Id = Guid.NewGuid(),
//                ArticleId = articleId,
//                CommentContent = "test 1",
//                PublishDate = DateTime.Now.AddDays(-1)
//            },
//            new Comment {
//                Id = Guid.NewGuid(),
//                ArticleId = articleId,
//                CommentContent = "test 2",
//                PublishDate = DateTime.Now
//            }
//        };

//        _mockRepository.Setup(r => r.GetQueryable())
//                       .Returns(modelList.AsQueryable());

//        // act
//        var result = await _service.GetByArticleIdAsync(articleId);

//        // assert
//        Assert.That(result, Is.Not.Null);
//        Assert.That(result, Has.Exactly(2).Items);

//        // order by ascending (created date)
//        Assert.That(result.First().Content, Is.EqualTo("test 1"));
//        Assert.That(result.Last().Content, Is.EqualTo("test 2"));
//    }

//    [Test]
//    public async Task GetByArticleIdAsync_Success_NoData()
//    {
//        // arrange
//        var articleId = Guid.NewGuid();

//        var modelList = new List<Comment>();

//        _mockRepository.Setup(r => r.GetQueryable())
//                       .Returns(modelList.AsQueryable());

//        // act
//        var result = await _service.GetByArticleIdAsync(articleId);

//        // assert
//        Assert.That(result, Is.Not.Null);
//        Assert.That(result, Has.Exactly(0).Items);
//    }


//    // UPDATE
//    [Test]
//    public async Task UpdateAsync_Success()
//    {
//        // arrange
//        var id = Guid.NewGuid();
//        var articleId = Guid.NewGuid();

//        // in db
//        var existingModel = new Comment
//        {
//            Id = id,
//            ArticleId = articleId,
//            CommentContent = "Old Content",
//            PublishDate = DateTime.Now.AddDays(-2),
//        };

//        var model = new CommentUpdate
//        {
//            Id = id,
//            ArticleId = articleId,
//            Content = "<p>lorem ipsum</p>",
//            Published = DateTime.Now,
//        };

//        _mockRepository.Setup(r => r.GetByIdAsync(id))
//                       .ReturnsAsync(existingModel);

//        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Comment>()))
//               .Returns(Task.CompletedTask);

//        // act
//        var result = await _service.UpdateAsync(model);

//        // assert
//        Assert.That(result, Is.True);
//    }

//    [Test]
//    public async Task UpdateAsync_Fail_NotFound()
//    {
//        // arrange
//        var id = Guid.NewGuid();
//        var articleId = Guid.NewGuid();

//        var model = new CommentUpdate
//        {
//            Id = id,
//            ArticleId = articleId,
//            Content = "<p>lorem ipsum</p>",
//            Published = DateTime.Now,
//        };

//        _mockRepository.Setup(r => r.GetByIdAsync(id))
//                       .ReturnsAsync(null as Comment);

//        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Comment>()))
//               .Returns(Task.CompletedTask);

//        // act
//        var result = await _service.UpdateAsync(model);

//        // assert
//        Assert.That(result, Is.False);
//    }

//    [Test]
//    public void UpdateAsync_Exception()
//    {
//        // arrange
//        var id = Guid.NewGuid();
//        var articleId = Guid.NewGuid();

//        // in db
//        var existingModel = new Comment
//        {
//            Id = id,
//            ArticleId = articleId,
//            CommentContent = "Old Content",
//            PublishDate = DateTime.Now.AddDays(-2),
//        };

//        var model = new CommentUpdate
//        {
//            Id = id,
//            Content = "<p>lorem ipsum</p>",
//            Published = DateTime.Now
//        };

//        _mockRepository.Setup(r => r.GetByIdAsync(id))
//                       .ReturnsAsync(existingModel);

//        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Comment>()))
//                       .ThrowsAsync(new DbUpdateException("Error updating comment"));

//        // play interrupted - act / assert
//        var ex = Assert.ThrowsAsync<DbUpdateException>(() => _service.UpdateAsync(model));
//        Assert.That(ex.Message, Is.EqualTo("Error updating comment"));
//    }
//}
