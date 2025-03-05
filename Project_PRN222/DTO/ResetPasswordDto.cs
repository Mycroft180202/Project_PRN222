namespace Project_PRN222.DTO
{
    public class ResetPasswordDto
    {
        public string Email { get; set; }
        public string ResetCode { get; set; }
        public string NewPassword { get; set; }
    }
}
