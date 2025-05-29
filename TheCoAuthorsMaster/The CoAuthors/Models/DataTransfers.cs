namespace The_CoAuthors.Models
{
    public class DataTransfers
    {
        public class Register
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class Login
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
