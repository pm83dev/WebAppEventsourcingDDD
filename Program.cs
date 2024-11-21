namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Aggiungi i servizi al contenitore di DI
            builder.Services.AddSingleton<IEventStore, InMemoryEventStore>();
            builder.Services.AddSingleton<IEventPublisher, EventPublisher>();
            builder.Services.AddSingleton<IMessageBroker, InMemoryMessageBroker>();
            builder.Services.AddTransient<CommandHandler>();

            // Aggiungi il logging
            builder.Services.AddLogging();

            builder.Services.AddControllers();

            // Configura Swagger per la documentazione API
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configura la pipeline di richiesta HTTP
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Configura la gestione delle richieste HTTPS
            app.UseHttpsRedirection();

            // Configura la gestione delle autorizzazioni
            app.UseAuthorization();

            // Aggiungi la gestione degli errori globali (opzionale)
            app.UseExceptionHandler("/Home/Error"); // Puoi anche creare una pagina di errore personalizzata

            // Mappa i controller
            app.MapControllers();

            app.Run();
        }
    }
}