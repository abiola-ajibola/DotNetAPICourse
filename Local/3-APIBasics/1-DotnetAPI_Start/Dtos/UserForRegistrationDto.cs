using System.Text.Json;

namespace DotnetAPI.Dtos
{
    public partial class UserForRegistrationDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string PasswordConfirm { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Gender { get; set; }

        private readonly JsonSerializerOptions _options = new() { WriteIndented = true };
        public override string ToString()
        {
            return JsonSerializer.Serialize(new
            {
                Email,
                Password,
                PasswordConfirm,
                FirstName,
                LastName,
                Gender
            }, _options);
        }
    }
}