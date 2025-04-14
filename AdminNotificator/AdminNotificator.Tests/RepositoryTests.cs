using AdminNotificator.Core;
using AdminNotificator.Core.Domain;
using AdminNotificator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AdminNotificator.Tests;

[TestFixture]
public class RepositoryTests
{
    private AdminNotificatorDbContext _context;
    private Repository<EmailType> _repository;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AdminNotificatorDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AdminNotificatorDbContext(options);
        _repository = new Repository<EmailType>(_context);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }
        
    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public void GetAll_ShouldReturnQueryable()
    {
        // Arrange
        _context.EmailTypes.AddRange(
            new EmailType { Id = "1", EmailTitle = "Test 1", BodyName = "Body1", SenderEmail = "test1@example.com" },
            new EmailType { Id = "2", EmailTitle = "Test 2", BodyName = "Body2", SenderEmail = "test2@example.com" }
        );
        _context.SaveChanges();

        // Act
        var result = _repository.GetAll().ToList();

        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.First().Id, Is.EqualTo("1"));
    }

    [Test]
    public async Task AddAsync_ShouldAddItem()
    {
        // Arrange
        var emailType = new EmailType
        {
            Id = "1",
            EmailTitle = "Test",
            BodyName = "Body",
            SenderEmail = "test@example.com"
        };

        // Act
        await _repository.AddAsync(emailType);
        await _context.SaveChangesAsync();

        // Assert
        var added = _context.EmailTypes.FirstOrDefault(x => x.Id == "1");
        Assert.That(added, Is.Not.Null);
        Assert.That(added.EmailTitle, Is.EqualTo("Test"));
    }

    [Test]
    public async Task AddAllAsync_ShouldAddMultipleItems()
    {
        // Arrange
        var items = new List<EmailType>
        {
            new() { Id = "1", EmailTitle = "One", BodyName = "Body1", SenderEmail = "1@example.com" },
            new() { Id = "2", EmailTitle = "Two", BodyName = "Body2", SenderEmail = "2@example.com" }
        };

        // Act
        await _repository.AddAllAsync(items);
        await _context.SaveChangesAsync();

        // Assert
        Assert.That(_context.EmailTypes.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task UpdateAsync_ShouldUpdateEntity()
    {
        // Arrange
        var emailType = new EmailType { Id = "1", EmailTitle = "Old", BodyName = "Body", SenderEmail = "email@example.com" };
        _context.EmailTypes.Add(emailType);
        await _context.SaveChangesAsync();

        // Act
        emailType.EmailTitle = "Updated";
        await _repository.UpdateAsync(emailType);
        await _context.SaveChangesAsync();

        // Assert
        var updated = _context.EmailTypes.First(x => x.Id == "1");
        Assert.That(updated.EmailTitle, Is.EqualTo("Updated"));
    }

    [Test]
    public async Task DeleteAsync_ShouldRemoveItem()
    {
        // Arrange
        var emailType = new EmailType { Id = "1", EmailTitle = "Delete Me", BodyName = "Body", SenderEmail = "email@example.com" };
        _context.EmailTypes.Add(emailType);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(emailType);
        await _context.SaveChangesAsync();

        // Assert
        Assert.That(_context.EmailTypes.Any(x => x.Id == "1"), Is.False);
    }

    [Test]
    public async Task DeleteAllAsync_WithItems_ShouldRemoveRange()
    {
        // Arrange
        var items = new List<EmailType>
        {
            new() { Id = "1", EmailTitle = "One", BodyName = "Body1", SenderEmail = "1@example.com" },
            new() { Id = "2", EmailTitle = "Two", BodyName = "Body2", SenderEmail = "2@example.com" }
        };
        _context.EmailTypes.AddRange(items);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAllAsync(items);
        await _context.SaveChangesAsync();

        // Assert
        Assert.That(_context.EmailTypes.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task DeleteAllAsync_WithPredicate_ShouldRemoveMatching()
    {
        // Arrange
        _context.EmailTypes.AddRange(
            new EmailType { Id = "1", EmailTitle = "Delete", BodyName = "Body1", SenderEmail = "a@example.com" },
            new EmailType { Id = "2", EmailTitle = "Keep", BodyName = "Body2", SenderEmail = "b@example.com" }
        );
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAllAsync(x => x.EmailTitle == "Delete");
        await _context.SaveChangesAsync();

        // Assert
        Assert.That(_context.EmailTypes.Count(), Is.EqualTo(1));
        Assert.That(_context.EmailTypes.First().EmailTitle, Is.EqualTo("Keep"));
    }
}