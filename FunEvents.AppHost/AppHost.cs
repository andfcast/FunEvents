var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                      .WithPgAdmin() // OPCIONAL: Levanta PgAdmin en un contenedor para inspeccionar tablas
                      .WithDataVolume(); // Mantiene la persistencia de datos local

var postgresDb = postgres.AddDatabase("funeventsdb");

// 2. Registrar la Web API
var apiService = builder.AddProject<Projects.FunEvents_API>("apiservice")
                        .WithReference(postgresDb)
                        .WaitFor(postgresDb); // Espera a que PostgreSQL esté listo antes de iniciar la API

// 3. Registrar el Cliente de Consola
builder.AddProject<Projects.FunEvents_ConsoleApp>("consoleclient")
       .WithReference(apiService)
       .WaitFor(apiService);

builder.Build().Run();
