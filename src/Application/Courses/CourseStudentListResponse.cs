using Itishnik.Domain.Entities;

namespace Itishnik.Application.Courses;

public class StudentDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;

    public string FullName { get; init; } = null!;
    public string Group { get; init; } = null!;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Student, StudentDto>()
                .ForMember(dto => dto.Group, options =>
                {
                    options.PreCondition(s => s.GroupNumber != 100);
                    options.MapFrom(s => $"{s.EducationStartYear} {s.EducationalProgram} {s.GroupNumber}");
                });
        }
    }
}

public class CourseStudentListResponse
{
    public List<StudentDto> Students { get; init; } = null!;
}
