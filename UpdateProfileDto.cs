namespace prog_part_2.Models
{
    public class UpdateProfileDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}