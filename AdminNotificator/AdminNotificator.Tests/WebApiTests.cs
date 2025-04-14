using AdminNotificator.Core.Domain;
using AdminNotificator.Core.DTOs;
using AdminNotificator.Core.Repositories;
using AdminNotificator.WebApi;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using AdminNotificator.Core;

namespace AdminNotificator.Tests;

[TestFixture]
public class WebApiTests
{
    private Mock<IMapper> _mockMapper;
    private Mock<ILogger<EmailController>> _mockLogger;
    private DbContextOptions<AdminNotificatorDbContext> _dbContextOptions;

    private EmailType CreateValidEmailType(string id = "1")
    {
        return new EmailType
        {
            Id = id,
            EmailTitle = "Test Title",
            BodyName = "Test Body",
            SenderEmail = "test@example.com"
        };
    }

    private EmailTypeDTO CreateValidEmailTypeDto()
    {
        return new EmailTypeDTO(
            ExperianceDays: null,
            EmailTitle: "Test Title",
            IntersectDepartmentIds: null,
            ExceptDepartmentIds: null,
            IntersectOrganizationNames: null,
            BodyName: "Test Body",
            IntersectTowns: null,
            ExceptTowns: null,
            DontSendAfterDate: null,
            MaternityDays: null,
            ForGenders: null,
            DayCountsForResend: null,
            IntersectUserPosts: null,
            ExceptUserPosts: null,
            SenderEmail: "test@example.com",
            Bcc: null
        );
    }

    [SetUp]
    public void Setup()
    {
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<EmailController>>();
        _dbContextOptions = new DbContextOptionsBuilder<AdminNotificatorDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Test]
    public async Task GetAll_ReturnsOkResultWithAllEmailTypes()
    {
        var testData = new List<EmailType>
        {
            CreateValidEmailType("1"),
            CreateValidEmailType("2")
        };

        using (var context = new AdminNotificatorDbContext(_dbContextOptions))
        {
            context.EmailTypes.AddRange(testData);
            await context.SaveChangesAsync();
        }

        using (var context = new AdminNotificatorDbContext(_dbContextOptions))
        {
            var repository = new Repository<EmailType>(context);
            var controller = new EmailController(repository, _mockMapper.Object, _mockLogger.Object);

            var result = controller.GetAll();

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(testData);
        }
    }

    [Test]
    public async Task GetById_WithExistingId_ReturnsEmailType()
    {
        var testEmail = CreateValidEmailType(Guid.NewGuid().ToString());

        using (var context = new AdminNotificatorDbContext(_dbContextOptions))
        {
            await context.EmailTypes.AddAsync(testEmail);
            await context.SaveChangesAsync();
        }

        using (var context = new AdminNotificatorDbContext(_dbContextOptions))
        {
            var repository = new Repository<EmailType>(context);
            var controller = new EmailController(repository, _mockMapper.Object, _mockLogger.Object);

            var result = await controller.GetById(testEmail.Id);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(testEmail);
        }
    }

    [Test]
    public async Task GetById_WithNonExistingId_ReturnsNotFound()
    {
        using (var context = new AdminNotificatorDbContext(_dbContextOptions))
        {
            var repository = new Repository<EmailType>(context);
            var controller = new EmailController(repository, _mockMapper.Object, _mockLogger.Object);

            var nonExistingId = Guid.NewGuid().ToString();
            var result = await controller.GetById(nonExistingId);

            result.Should().BeOfType<NotFoundResult>();
        }
    }

    [Test]
    public async Task Post_WithValidDto_ReturnsCreatedAtRoute()
    {
        var dto = CreateValidEmailTypeDto();
        var generatedId = Guid.NewGuid().ToString();
        var emailType = CreateValidEmailType(generatedId);
        _mockMapper.Setup(m => m.Map<EmailType>(dto))
            .Returns(() => {
                var newEmail = CreateValidEmailType(generatedId);
                return newEmail;
            });

        using (var context = new AdminNotificatorDbContext(_dbContextOptions))
        {
            var repository = new Repository<EmailType>(context);
            var controller = new EmailController(repository, _mockMapper.Object, _mockLogger.Object);

            var result = await controller.Post(dto);

            result.Should().BeOfType<CreatedAtRouteResult>();
            var createdResult = result as CreatedAtRouteResult;
            createdResult!.RouteName.Should().Be(nameof(EmailController.GetById));
            createdResult.RouteValues!["id"].Should().Be(generatedId);
            createdResult.Value.Should().BeEquivalentTo(emailType);

            var dbItem = await context.EmailTypes.FindAsync(generatedId);
            dbItem.Should().NotBeNull();
            dbItem.Should().BeEquivalentTo(emailType);
        }
    }

    [Test]
    public async Task Post_WithDbUpdateException_ReturnsConflict()
    {
        var dto = CreateValidEmailTypeDto();
        _mockMapper.Setup(m => m.Map<EmailType>(dto)).Returns(CreateValidEmailType());

        var mockRepository = new Mock<IRepository<EmailType>>();
        mockRepository.Setup(r => r.AddAsync(It.IsAny<EmailType>(), It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new DbUpdateException());

        var controller = new EmailController(mockRepository.Object, _mockMapper.Object, _mockLogger.Object);

        var result = await controller.Post(dto);

        result.Should().BeOfType<ConflictResult>();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!),
            Times.Once);
    }

    [Test]
    public async Task Post_WithGenericException_ReturnsServerError()
    {
        var dto = CreateValidEmailTypeDto();
        _mockMapper.Setup(m => m.Map<EmailType>(dto)).Returns(CreateValidEmailType());

        var mockRepository = new Mock<IRepository<EmailType>>();
        mockRepository.Setup(r => r.AddAsync(It.IsAny<EmailType>(), It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new Exception("Something bad"));

        var controller = new EmailController(mockRepository.Object, _mockMapper.Object, _mockLogger.Object);

        var result = await controller.Post(dto);

        result.Should().BeOfType<ConflictResult>();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!),
            Times.Once);
    }

    [Test]
    public async Task DeleteAsync_WithExistingId_ReturnsNoContent()
    {
        var testEmail = CreateValidEmailType("1");

        using (var context = new AdminNotificatorDbContext(_dbContextOptions))
        {
            context.EmailTypes.Add(testEmail);
            await context.SaveChangesAsync();
        }

        using (var context = new AdminNotificatorDbContext(_dbContextOptions))
        {
            var repository = new Repository<EmailType>(context);
            var controller = new EmailController(repository, _mockMapper.Object, _mockLogger.Object);

            var result = await controller.DeleteAsync("1");

            result.Should().BeOfType<NoContentResult>();
            var email = await context.EmailTypes.FindAsync("1");
            email.Should().BeNull();
        }
    }

    [Test]
    public async Task GetById_WithInvalidGuid_ReturnsUnprocessableEntity()
    {
        using (var context = new AdminNotificatorDbContext(_dbContextOptions))
        {
            var repository = new Repository<EmailType>(context);
            var controller = new EmailController(repository, _mockMapper.Object, _mockLogger.Object);

            var result = await controller.GetById("invalid-guid");

            result.Should().BeOfType<UnprocessableEntityObjectResult>();
        }
    }
}