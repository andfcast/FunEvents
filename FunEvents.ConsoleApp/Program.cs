using FunEvents.Application.DTO;
using System.Net.Http.Json;

namespace FunEvents.ConsoleApp
{
    internal class Program
    {
        private static readonly HttpClient httpClient = new()
        {
            BaseAddress = new Uri("https://localhost:44367") // Ajusta el puerto de tu API
        };

        static async Task Main(string[] args)
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("    FunEvents - Cliente de Pruebas de Concurrencia");
            Console.WriteLine("==================================================\n");

            // 1. Ejecutar el Seed para asegurar que el evento exista
            await EjecutarSeedAsync();

            // 2. Ejecutar la prueba de reservas concurrentes
            await SimularReservasConcurrentesAsync(totalPeticiones: 10);

            Console.WriteLine("\nPresiona cualquier tecla para salir...");
            Console.ReadKey();
        }

        private static async Task EjecutarSeedAsync()
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

        private static async Task SimularReservasConcurrentesAsync(int totalPeticiones)
        {
            var eventoId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            Console.WriteLine($"--> Lanzando {totalPeticiones} solicitudes de reserva concurrentes al evento {eventoId}...\n");

            var tareas = new List<Task>();

            for (int i = 1; i <= totalPeticiones; i++)
            {
                int clienteNumero = i;
                tareas.Add(Task.Run(() => RealizarReservaAsync(eventoId, clienteNumero)));
            }

            // Esperar a que todas las peticiones terminen simultáneamente
            await Task.WhenAll(tareas);
        }

        private static async Task RealizarReservaAsync(Guid eventoId, int clienteNumero)
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
