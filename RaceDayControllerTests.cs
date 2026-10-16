using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using prog_part_2.Controllers;
using prog_part_2;
using prog_part_2.Models;
using Xunit;

namespace prog_part_2.Tests
{
    public class RaceDayControllerTests
    {
        private RaceDayDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new RaceDayDbContext(options);
            databaseContext.Database.EnsureCreated();
            return databaseContext;
        }

        [Fact]
        public async Task Register_Success_ReturnsOk()
        {
            // Arrange
            var context = GetDatabaseContext();
            var controller = new RaceDayController(context);
            var dto = new RegisterDto { Username = "TestUser", Email = "test@example.com", Password = "Password123" };

            // Act
            var result = await controller.Register(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
        }

        [Fact]
        public async Task Register_DuplicateEmail_ReturnsBadRequest()
        {
            // Arrange
            var context = GetDatabaseContext();
            context.Users.Add(new User { Username = "Existing", Email = "duplicate@example.com", PasswordHash = "123", RoleId = 2 });
            await context.SaveChangesAsync();

            var controller = new RaceDayController(context);
            var dto = new RegisterDto { Username = "NewUser", Email = "duplicate@example.com", Password = "Password123" };

            // Act
            var result = await controller.Register(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Login_Success_ReturnsOkAndSetsSession()
        {
            // Arrange
            var context = GetDatabaseContext();
            context.Users.Add(new User { UserId = 1, Username = "LoginUser", Email = "login@example.com", PasswordHash = "Password123", RoleId = 2 });
            context.SaveChanges();

            var controller = new RaceDayController(context);
            // Mocking HTTP Context for Session testing
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            httpContext.Session = new FakeSession();
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            var dto = new LoginDto { Email = "login@example.com", Password = "Password123" };

            // Act
            var result = controller.Login(dto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var context = GetDatabaseContext();
            var controller = new RaceDayController(context);
            var dto = new LoginDto { Email = "wrong@example.com", Password = "WrongPassword" };

            // Act
            var result = controller.Login(dto);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task CreateEvent_Success_ReturnsCreatedEvent()
        {
            // Arrange
            var context = GetDatabaseContext();
            var controller = new RaceDayController(context);
            var dto = new EventDto { EventName = "Marathon 2026", EventDate = DateTime.Now.AddDays(10), Location = "Stadium" };

            // Act
            var result = await controller.CreateEvent(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Enrol_Success_ReturnsOk()
        {
            // Arrange
            var context = GetDatabaseContext();
            var controller = new RaceDayController(context);
            var dto = new EnrolmentDto { UserId = 1, EventId = 1 };

            // Act
            var result = await controller.Enrol(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
        }
    }

    // Helper class to mock session storage in unit tests
    public class FakeSession : Microsoft.AspNetCore.Http.ISession
    {
        private readonly Dictionary<string, byte[]> _sessionStorage = new Dictionary<string, byte[]>();
        public bool IsAvailable => true;
        public string Id => Guid.NewGuid().ToString();
        public IEnumerable<string> Keys => _sessionStorage.Keys;
        public void Clear() => _sessionStorage.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _sessionStorage.Remove(key);
        public void Set(string key, byte[] value) => _sessionStorage[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _sessionStorage.TryGetValue(key, out value);
    }
}