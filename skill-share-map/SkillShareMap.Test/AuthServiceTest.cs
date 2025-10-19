using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SkillShareMap.Data;
using SkillShareMap.Models;
using SkillShareMap.Services;

namespace SkillShareMap.Tests;

public class AuthServiceTest
{
    private ApplicationDbContext _db = null!;
    private AuthService _service = null!;

    [SetUp]
    public void Init()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"AuthTest_{Guid.NewGuid()}")
            .Options;

        _db = new ApplicationDbContext(options);
        _service = new AuthService(_db);
    }

    [TearDown]
    public void Cleanup()
    {
        _db.Dispose();
    }

    [Test]
    public async Task Register_AddsUserAndWallet()
    {
        var student = new User
        {
            Username = "student1",
            Email = "student1@test.com",
            Role = UserRole.Student
        };

        var created = await _service.RegisterAsync(student, "pass123");

        Assert.That(created, Is.Not.Null);
        Assert.That(await _db.Users.CountAsync(), Is.EqualTo(1));
        Assert.That(await _db.Wallets.CountAsync(), Is.EqualTo(1));
    }
}
