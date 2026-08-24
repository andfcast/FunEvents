using FunEvents.Application.DTO;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FunEvents.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // 1. Configurar Host con Service Discovery y Telemetría de Aspire
            var builder = Host.CreateApplicationBuilder(args);

            builder.AddServiceDefaults();

            // 2. Registrar HttpClient apuntando al nombre del servicio en AppHost ("apiservice")
            builder.Services.AddHttpClient<ReservaApiClient>(client =>
            {
                client.BaseAddress = new Uri("http://apiservice");
            });

            using var host = builder.Build();

            // 3. Obtener el servicio y ejecutar las pruebas
            var apiClient = host.Services.GetRequiredService<ReservaApiClient>();
            await apiClient.EjecutarPruebaCompletaAsync();

            Console.WriteLine("\nPresiona cualquier tecla para salir...");
            Console.ReadKey();
        }
    }

    public class ReservaApiClient(HttpClient httpClient)
    {
        public async Task EjecutarPruebaCompletaAsync()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("    FunEvents - Cliente de Pruebas (Aspire)");
            Console.WriteLine("==================================================\n");

            // 1. Ejecutar el Seed
            await EjecutarSeedAsync();

            // 2. Ejecutar la prueba de reservas concurrentes
            await SimularReservasConcurrentesAsync(totalPeticiones: 1);
        }

        private async Task EjecutarSeedAsync()
        {
            Console.WriteLine("--> Inicializando datos de prueba en la API (/api/eventos/seed)...");

            try
            {
                var response = await httpClient.PostAsync("/api/eventos/seed", null);

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[OK] Seed completado: {resultado}\n");
                }
                else
                {
                    Console.WriteLine($"[ERROR] No se pudo ejecutar el seed. Status Code: {response.StatusCode}\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPCIÓN] Error al conectar con la API: {ex.Message}\n");
            }
        }

        private async Task SimularReservasConcurrentesAsync(int totalPeticiones)
        {
            var eventoId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            Console.WriteLine($"--> Lanzando {totalPeticiones} solicitudes de reserva concurrentes al evento {eventoId}...\n");

            var tareas = new List<Task>();

            for (int i = 1; i <= totalPeticiones; i++)
            {
                int clienteNumero = i;
                tareas.Add(Task.Run(() => RealizarReservaAsync(eventoId, clienteNumero)));
            }

            await Task.WhenAll(tareas);
        }

        private async Task RealizarReservaAsync(Guid eventoId, int clienteNumero)
        {
            var request = new
            {
                EventoId = eventoId,
                UsuarioId = Guid.NewGuid(),
                Cantidad = 2
            };

            try
            {
                var response = await httpClient.PostAsJsonAsync("/api/reservas", request);
                var contenido = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[Cliente #{clienteNumero}] [201 Created] Reserva Exitosa -> {contenido}");
                }
                else
                {
                    Console.WriteLine($"[Cliente #{clienteNumero}] [{(int)response.StatusCode} {response.StatusCode}] Falló la reserva -> {contenido}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Cliente #{clienteNumero}] [EXCEPCIÓN] -> {ex.Message}");
            }
        }
    }
}
