namespace NotesApplication.API.Controllers
{
    public class UserWithRolesModel
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}