using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<EFCore101_API>("api")
    .WithExternalHttpEndpoints();

builder.Build().Run();
