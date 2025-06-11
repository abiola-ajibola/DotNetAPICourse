using System.Text.Json;

namespace DotnetAPI.Models
{
    public partial class UserJobInfo
    {
        public int UserId { get; set; }
        public string JobTitle { get; set; } = "";
        public string Department { get; set; } = "";

        override public string ToString()
        {
            return JsonSerializer.Serialize(new { UserId, JobTitle, Department });
        }

    }
}