using AutoMapper;
using CommandsService.DTOs;
using CommandsService.Models;
using PlatformService;

namespace CommandsService.Profiles
{
    public class CommandsProfile : Profile
    {
        public CommandsProfile()
        {
            // Source -> Target
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<Command, CommandReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<PlatformPublishedDto, Platform>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.ExternalId, opt => opt.MapFrom(s => s.Id));
            CreateMap<GrpcPlatformModel, Platform>()
                .ForMember(d => d.ExternalId, opt => opt.MapFrom(s => s.PlatformId))
                .ForMember(d => d.Commands, opt => opt.Ignore());                
        }   
    }
}