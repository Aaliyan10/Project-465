var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Movies_API>("movies-api");

builder.Build().Run();
