using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("username", "admin");
var password = builder.AddParameter("password", "admin");

var postgres = builder.AddPostgres("postgres", username, password)
    .WithDataBindMount(
        source: @"C:\PostgreSQL\EFCore101",
        isReadOnly: false);

var postgresdb = postgres.AddDatabase("postgresdb");

var api = builder.AddProject<EFCore101_API>("api")
    .WithReference(postgresdb)
    .WithExternalHttpEndpoints();

builder.Build().Run();
