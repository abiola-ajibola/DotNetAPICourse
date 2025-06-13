using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace DotnetAPI.Dtos
{
    public partial class UserForLoginDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }

        private readonly JsonSerializerOptions _options = new() { WriteIndented = true };
        public override string ToString()
        {
            return JsonSerializer.Serialize(new { Email, Password }, _options);
        }
    }
}