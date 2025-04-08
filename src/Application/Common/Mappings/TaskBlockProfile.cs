using Itishnik.Application.Courses;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Common.Mappings;

public class TaskBlockProfile : Profile
{
    public TaskBlockProfile()
    {
        CreateMap<TaskBlock, TaskBlockResponse>()
            .ForMember(tbr => tbr.Tasks, options => options.MapFrom(tb => tb.Tasks))
            .ForMember(tbr => tbr.Weights, options => options.MapFrom(tb => tb.Weights));
    }
}
