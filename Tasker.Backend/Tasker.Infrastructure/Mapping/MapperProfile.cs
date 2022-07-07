using AutoMapper;
using Tasker.Core.Entities;
using Tasker.Core.Logic.Objective.Responses;

namespace Tasker.Infrastructure.Mapping;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Objective, GetObjectiveResponse>();
    }
}
