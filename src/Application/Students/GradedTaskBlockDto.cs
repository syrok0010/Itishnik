using Itishnik.Domain.Entities;

namespace Itishnik.Application.Students;

public class GradedTaskBlockDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }

    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public TimeSpan? TimeAllowed { get; init; }
    public DateTime? StudentStartTime { get; init; }

    public int? Grade { get; init; }
    public int TaskCount { get; init; }

    public ICollection<SolutionDto>? Solutions { get; init; }

    protected class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<GradedTaskBlock, GradedTaskBlockDto>()
                .ForMember(w => w.Name, options => options.MapFrom(gtb => gtb.TaskBlock.Name))
                .ForMember(w => w.Description, options => options.MapFrom(gtb => gtb.TaskBlock.Description))
                .ForMember(w => w.StartTime, options => options.MapFrom(gtb => gtb.TaskBlock.StartTime))
                .ForMember(w => w.EndTime, options => options.MapFrom(gtb => gtb.TaskBlock.EndTime))
                .ForMember(w => w.TimeAllowed, options => options.MapFrom(gtb => gtb.TaskBlock.TimeAllowed))
                .ForMember(w => w.StudentStartTime, options => options.MapFrom(gtb => gtb.StartTime))
                .ForMember(w => w.Grade, options => options.MapFrom(gtb => gtb.Grade))
                .ForMember(w => w.TaskCount, options => options.MapFrom(gtb => gtb.TaskBlock.TasksEntries.Count()))
                .ForMember(w => w.Solutions, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    if (src.StartTime == null)
                        return null;

                    var entryData = src.TaskBlock.TasksEntries.ToDictionary(
                        entry => entry.TaskId,
                        entry => new { entry.Weight, entry.Position }
                    );

                    return src.Solutions
                        .Select(solution =>
                        {
                            entryData.TryGetValue(solution.TaskId, out var data);

                            return new SolutionDto
                            {
                                Id = solution.Id,
                                Text = solution.Text,
                                Grade = solution.Grade,
                                Task = context.Mapper.Map<TaskDto>(solution.Task),
                                Weight = data!.Weight,
                                Position = data.Position
                            };
                        })
                        .OrderBy(dto => dto.Position)
                        .ToList();
                }));
            ;
        }
    }
}
