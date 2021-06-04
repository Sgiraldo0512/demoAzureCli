using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebWriteCosmos.Entidades;

namespace WebWriteCosmos.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CosmosDbController : ControllerBase
    {

        protected IConfiguration Configuration { get; }
        private string EndpointUri = "https://db-modulo-cosmos.documents.azure.com:443/";

        // The primary key for the Azure Cosmos account.
        private string PrimaryKey = "caeJsUKE5w4PdmRTTmpKQGiieDLGM8J4hAN7so0lCZi3Bq5iPFrTv3YUS0wzwilKmWdo5qsSyV9idbenU33jFQ==";

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        private Database database;

        // The container we will create.
        private Container container;

        // The name of the database and container we will create
        private string databaseId = "curso1";
        private string containerId = "laboratorioviernes";

        private readonly ILogger<CosmosDbController> _logger;
        private static Random random = new Random();
        public CosmosDbController(ILogger<CosmosDbController> logger)
        {
            _logger = logger;

            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
        }

        [HttpGet("Get")]
        public async Task<List<Entidades.Pelicula>> Get()
        {
            return await Buscar();
        }

        protected async Task gestionarDbCn()
        {
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/genero", 400);
        }

        [HttpGet("Buscar")]
        private async Task<List<Entidades.Pelicula>> Buscar()
        {
            await gestionarDbCn();
            var sqlQueryText = "SELECT * FROM c";


            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Entidades.Pelicula> queryResultSetIterator = this.container.GetItemQueryIterator<Entidades.Pelicula>(queryDefinition);

            List<Entidades.Pelicula> peliculas = new List<Entidades.Pelicula>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Entidades.Pelicula> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (Entidades.Pelicula peli in currentResultSet)
                {
                    peliculas.Add(peli);
                }
            }

            return peliculas;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create()
        {
            await gestionarDbCn();
            for (int i = 0; i < 100; i++)
            {
                Constantes.TipoAlmacenamiento tipoAlmacenamiento = (Constantes.TipoAlmacenamiento)random.Next(1, 3);

                Entidades.Pelicula pelicula = new Entidades.Pelicula
                {
                    Nombre = RandomString(15),
                    Genero = "Accion",
                    Id = System.Guid.NewGuid().ToString(),
                    FechaLanzamiento = DateTime.Now,
                    LugarAlmacenamiento = tipoAlmacenamiento.ToString()
                };

                ItemResponse<Entidades.Pelicula> personaResponse = await this.container.CreateItemAsync<Entidades.Pelicula>(pelicula, new PartitionKey(pelicula.Genero));

            }
            return Content("Exito");
        }


        [HttpDelete("DeleteContainer")]
        public async Task<IActionResult> DeleteContainer()
        {
            await gestionarDbCn();
            var personResponse = await this.container.DeleteContainerAsync();
            await Get();
            return Content("¡EXITO!");
        }

        protected static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
