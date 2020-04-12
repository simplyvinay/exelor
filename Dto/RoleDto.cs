namespace Exelor.Dto
{
    public class RoleDto
    {
        protected RoleDto() { }

        public RoleDto(
            int id,
            string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; }
        public string Name { get; }
    }
}