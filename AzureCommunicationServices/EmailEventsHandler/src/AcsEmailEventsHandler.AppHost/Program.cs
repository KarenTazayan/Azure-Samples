var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.AcsEmailEventsHandler_Engine>("acsemaileventshandler-engine");

builder.AddProject<Projects.AcsEmailEventsHandler_WebApp>("acsemaileventshandler-webapp");

builder.Build().Run();
