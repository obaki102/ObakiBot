var builder = DistributedApplication.CreateBuilder(args);

var ollama = builder.AddOllama("ollama", null)
    .WithDataVolume()
    .WithContainerRuntimeArgs("--gpus=all")
    .PublishAsContainer()
    .AddModel("llama3.2");

var api = builder.AddProject<Projects.ObakiBot_Api>("api")
    .WithExternalHttpEndpoints()
    .WithReference(ollama);;

var ui = builder.AddProject<Projects.ObakiBot_Ui>("ui")
    .WithExternalHttpEndpoints()
    .WithReference(ollama);;

// var discord =  builder.AddProject<Projects.ObakiBot_Discord>("discord")
//     .WithExternalHttpEndpoints()
//     .WithReference(ollama);;
builder.Build().Run();
