namespace Exelor.Dto
{
    public class UserDetailsDto
    {
        protected UserDetailsDto()
        {
        }

        public UserDetailsDto(
            int id,
            string firstName,
            string lastName,
            string fullName,
            string email,
            string roles)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            FullName = fullName;
            Email = email;
            Roles = roles;
        }

        public int Id { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string FullName { get; }
        public string Email { get; }
        public string Roles { get; }
    }
}