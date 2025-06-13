namespace DotnetAPI.Dtos
{
    public partial class UserForLoginConfirmationDto
    {
        public byte[] PasswordHash { get; set; } = [];
        public byte[] PasswordSalt { get; set; } = [];
        public string PasswordHash2 { get; set; } = ""; // This is used for string based password hash, also does not need to store the salt
    }
}