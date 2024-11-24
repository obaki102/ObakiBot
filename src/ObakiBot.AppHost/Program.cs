var builder = DistributedApplication.CreateBuilder(args);

var ui = builder.AddProject<Projects.ObakiBot_Ui>("ui")
    .WithExternalHttpEndpoints();
builder.Build().Run();
