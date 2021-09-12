using System;
using System.Text.Json;
using AutoMapper;
using CommandsService.Data.Repositories;
using CommandsService.DTOs;
using CommandsService.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch(eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine($"--> Determining Event Type");
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
            
            switch(eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("--> Platform Published Event detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("--> Could not detect the Event Type");
                    return EventType.Undetermined;
            }
        }

        private void AddPlatform(string platformPublishedMessage)
        {
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();

            var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);
            try
            {
                var platform = _mapper.Map<Platform>(platformPublishedDto);
                if (!repository.IsExternalPlatformExists(platform.ExternalId))
                {
                    repository.CreatePlatform(platform);
                    repository.SaveChanges();
                    Console.WriteLine("--> Platform created");
                }
                else
                {
                    Console.WriteLine("--> Platform already exists...");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"--> Could not add platform to Db: {ex.Message}");
            }
        }
    }

    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}