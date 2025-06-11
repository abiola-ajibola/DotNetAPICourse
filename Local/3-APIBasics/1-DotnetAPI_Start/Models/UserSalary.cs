using System.Text.Json;

namespace DotnetAPI.Models
{
    public partial class UserSalary
    {
        public int UserId { get; set; }
        public decimal Salary { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(new { UserId, Salary });
        }
    }
}