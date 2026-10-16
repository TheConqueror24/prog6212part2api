using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using prog_part_2;
using prog_part_2.Models;

namespace prog_part_2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RaceDayController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public RaceDayController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (_context.Users.Any(u => u.Email == dto.Email))
                return BadRequest("Email is already registered.");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                RoleId = dto.RoleId
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully!" });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);
            if (user == null)
                return Unauthorized("Invalid email or password.");

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (verificationResult == PasswordVerificationResult.Failed)
                return Unauthorized("Invalid email or password.");

            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetInt32("RoleId", user.RoleId);

            return Ok(new { message = "Login successful!", userId = user.UserId, email = user.Email, roleId = user.RoleId });
        }

        [HttpGet("profile/{id}")]
        public IActionResult GetProfile(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound("User not found.");
            return Ok(new { user.UserId, user.Username, user.Email, user.RoleId });
        }

        [HttpPut("profile/{id}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateProfileDto dto)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound("User not found.");

            user.Username = dto.Username;
            user.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.NewPassword))
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Profile updated successfully!" });
        }

        [HttpGet("events")]
        public IActionResult GetEvents()
        {
            var events = _context.Events.ToList();
            return Ok(events);
        }

        [HttpPost("events")]
        public async Task<IActionResult> CreateEvent([FromBody] EventDto dto)
        {
            var newEvent = new Event
            {
                EventName = dto.EventName,
                EventDate = dto.EventDate,
                Location = dto.Location
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return Ok(newEvent);
        }

        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            var categories = _context.Categories.ToList();
            return Ok(categories);
        }

        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return Ok(category);
        }

        [HttpPost("enrol")]
        public async Task<IActionResult> Enrol([FromBody] EnrolmentDto dto)
        {
            var enrolment = new Enrolment
            {
                UserId = dto.UserId,
                EventId = dto.EventId,
                EnrolmentDate = DateTime.Now
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Enrolment successful!" });
        }

        [HttpGet("results")]
        public IActionResult GetResults()
        {
            var results = _context.Results.ToList();
            return Ok(results);
        }

        [HttpPost("results")]
        public async Task<IActionResult> CaptureResult([FromBody] ResultDto dto)
        {
            var result = new Result
            {
                EnrolmentId = dto.EnrolmentId,
                Position = int.TryParse(dto.Position, out var pos) ? pos : 0,
                TimeRecord = dto.TimeRecord
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Result captured successfully!" });
        }
    }
}