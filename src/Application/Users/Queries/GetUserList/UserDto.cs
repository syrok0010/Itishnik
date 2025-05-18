using Itishnik.Domain.Entities;

namespace Itishnik.Application.Users.Queries.GetUserList;

public class UserDto
{
    public Guid Id { get; set; }
    public string Surname { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Patronymic { get; set; } = null!;
    public string Email { get; set; } = null!;
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Student, UserDto>();
            CreateMap<Teacher, UserDto>();
        }
    }
}
