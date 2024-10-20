// ReSharper disable SuggestVarOrType_Elsewhere

using Projects;

namespace Aspire.AppHost;

public static class Program
{
    public static void Main(string[] args)
    {
        IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

        var db = builder.AddPostgres("db-judgetry")
                        .WithPgAdmin(null, "panel-postgres")
                        .PublishAsContainer()
                        .AddDatabase("cs-judgetry", "judgetry");

        builder.AddProject<Judgetry_Web>("web-judgetry")
               .WithReference(db);

        builder.Build().Run();
    }
}
