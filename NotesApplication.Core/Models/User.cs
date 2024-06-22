namespace NotesApplication.Core.Models
{
    public class User
    {
        private string id;

        public User()
        {
        }

        private User(Guid id, string password, string email)
        {
            Id = id;
            // UserName = userName;
            Password = password;
            Email = email;
        }

        public Guid Id { get; }

        public string UserName { get; }

        public string Password { get; }

        public string Email { get; }

        public static User Create(Guid id, string password, string email)
        {
            return new User(id, password, email);
        }
    }
}