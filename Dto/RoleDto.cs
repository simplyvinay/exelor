namespace Exelor.Dto
{
    public class RoleDto
    {
        protected RoleDto() { }

        public RoleDto(
            int role,
            string name)
        {
            Role = role;
            Name = name;
        }

        public int Id { get; set; }
        public int Role { get; }
        public string Name { get; set; }
    }
}