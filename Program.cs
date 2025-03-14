namespace WebApplication10
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSingleton<Room>();

            // Add services to the container.
            builder.Services.AddSignalR().
                AddJsonProtocol(s =>
                {
                    s.PayloadSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                });

            var app = builder.Build();

            app.MapHub<MyHub>("/game");

     

            app.Run();
        }
    }
}
