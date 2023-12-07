var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.FaceBot_WebApi>("facebot.webapi");

builder.Build().Run();
