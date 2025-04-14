using AdminNotificator.Core;
using AdminNotificator.Core.Domain;
using AdminNotificator.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

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
        _context.EmailTypes.AddRange(
            new EmailType { Id = "1", EmailTitle = "Test 1", BodyName = "Body1", SenderEmail = "test1@example.com" },
            new EmailType { Id = "2", EmailTitle = "Test 2", BodyName = "Body2", SenderEmail = "test2@example.com" }
        );
        _context.SaveChanges();

        var result = _repository.GetAll().ToList();

        result.Count.Should().Be(2);
        result.First().Id.Should().Be("1");
    }

    [Test]
    public async Task AddAsync_ShouldAddItem()
    {
        await _repository.AddAsync("1");
        await _context.SaveChangesAsync();

        var added = _context.EmailTypes.FirstOrDefault(x => x.Id == "1");
        added.Should().NotBeNull();
    }

    [Test]
    public async Task AddAllAsync_ShouldAddMultipleItems()
    {
        await _repository.AddAllAsync(new[] { "1", "2" });
        await _context.SaveChangesAsync();

        _context.EmailTypes.Count().Should().Be(2);
    }

    [Test]
    public async Task UpdateAsync_ShouldUpdateEntity()
    {
        var emailType = new EmailType { Id = "1", EmailTitle = "Old", BodyName = "Body", SenderEmail = "email@example.com" };
        _context.EmailTypes.Add(emailType);
        await _context.SaveChangesAsync();

        emailType.EmailTitle = "Updated";
        await _repository.UpdateAsync("1");
        await _context.SaveChangesAsync();

        var updated = _context.EmailTypes.First(x => x.Id == "1");
        updated.EmailTitle.Should().Be("Updated");
    }

    [Test]
    public async Task DeleteAsync_ShouldRemoveItem()
    {
        var emailType = new EmailType { Id = "1", EmailTitle = "Delete Me", BodyName = "Body", SenderEmail = "email@example.com" };
        _context.EmailTypes.Add(emailType);
        await _context.SaveChangesAsync();

        await _repository.DeleteAsync("1");
        await _context.SaveChangesAsync();

        _context.EmailTypes.Any(x => x.Id == "1").Should().BeFalse();
    }

    [Test]
    public async Task DeleteAllAsync_WithItems_ShouldRemoveRange()
    {
        var items = new List<EmailType>
        {
            new() { Id = "1", EmailTitle = "One", BodyName = "Body1", SenderEmail = "1@example.com" },
            new() { Id = "2", EmailTitle = "Two", BodyName = "Body2", SenderEmail = "2@example.com" }
        };
        _context.EmailTypes.AddRange(items);
        await _context.SaveChangesAsync();

        await _repository.DeleteAllAsync(new[] { "1", "2" });
        await _context.SaveChangesAsync();

        _context.EmailTypes.Count().Should().Be(0);
    }

    [Test]
    public async Task DeleteAllAsync_WithPredicate_ShouldRemoveMatching()
    {
        _context.EmailTypes.AddRange(
            new EmailType { Id = "1", EmailTitle = "Delete", BodyName = "Body1", SenderEmail = "a@example.com" },
            new EmailType { Id = "2", EmailTitle = "Keep", BodyName = "Body2", SenderEmail = "b@example.com" }
        );
        await _context.SaveChangesAsync();

        await _repository.DeleteAllAsync(x => x.EmailTitle == "Delete");
        await _context.SaveChangesAsync();

        _context.EmailTypes.Count().Should().Be(1);
        _context.EmailTypes.First().EmailTitle.Should().Be("Keep");
    }
}