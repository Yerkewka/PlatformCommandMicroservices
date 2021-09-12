using System;
using System.Collections.Generic;
using CommandsService.Data.Repositories;
using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder applicationBuilder)
        {
            using var scope = applicationBuilder.ApplicationServices.CreateScope();

            var grpcClient = scope.ServiceProvider.GetRequiredService<IPlatformDataClient>();
            var platforms = grpcClient.ReturnAllPlatforms();

            var repository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
            SeedData(repository, platforms);
        }

        private static void SeedData(ICommandRepository repository, IEnumerable<Platform> platforms)
        {
            Console.WriteLine($"--> Seeding new platforms...");

            foreach (var plarform in platforms)
            {
                if (!repository.IsExternalPlatformExists(plarform.ExternalId))
                {
                    repository.CreatePlatform(plarform);
                }
            }
            repository.SaveChanges();
        }
    }
}