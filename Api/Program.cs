namespace Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddGrpc();
        var services = builder.Services;

        var app = builder.Build();

        // app.MapGrpcService<GreeterService>();

        app.Run();
    }
}