using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Configura la rappresentazione globale di Guid
            BsonSerializer.RegisterSerializer(
                typeof(Guid), 
                new MongoDB.Bson.Serialization.Serializers.GuidSerializer(GuidRepresentation.Standard)
            );
            // Configura il client MongoDB
            builder.Services.AddSingleton<IMongoClient>(new MongoClient("mongodb://localhost:27017"));
            
            // Aggiungi i servizi al contenitore di DI
            builder.Services.AddSingleton<IEventStore, MongoDbEventStore>(); // MongoDB
            //builder.Services.AddSingleton<IEventStore,InMemoryEventStore>(); // InMemory
            builder.Services.AddSingleton<IMessageBroker, InMemoryMessageBroker>();
            
            //Servizi Rabbit
            builder.Services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
            builder.Services.AddSingleton<IEventListener,RabbitMqEventListener>();
            
            builder.Services.AddTransient<CommandHandler>();
            

            // Aggiungi il logging
            builder.Services.AddLogging();

            builder.Services.AddControllers();

            // Configura Swagger per la documentazione API
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            // Registra il servizio in background per l'ascolto
            builder.Services.AddHostedService<RabbitMqListenerService>();
            
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