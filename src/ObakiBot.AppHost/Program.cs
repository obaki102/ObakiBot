var builder = DistributedApplication.CreateBuilder(args);
var ollama = builder.AddOllama("ollama", null)
    .WithDataVolume()
    .WithContainerRuntimeArgs("--gpus=all")
    .PublishAsContainer()
    .AddModel("llama3.2");

var ui = builder.AddProject<Projects.ObakiBot_Ui>("ui")
    .WithExternalHttpEndpoints()
    .WithReference(ollama);;
builder.Build().Run();
