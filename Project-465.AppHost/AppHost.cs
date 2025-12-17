var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Movies_API>("movies-api");
builder.AddProject<Projects.Users_API>("users-api");

builder.Build().Run();
