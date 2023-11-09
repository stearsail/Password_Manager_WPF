namespace Password_manager.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public string Username { get; set; }
        public string EncryptedPassword { get; set; }
        public string Website { get; set; }
    }
}
