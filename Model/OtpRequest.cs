namespace hrms_api.Model
{
    public class OtpRequest
    {
        public int id { get; set; }
        public string email { get; set; }

        public string otp { get; set; }

        public DateTime ExpirationTime { get; set; }
        public bool IsUsed { get; set; } = false;
    }
}
