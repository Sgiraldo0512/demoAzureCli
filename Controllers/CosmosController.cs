//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Azure.Cosmos;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Hosting;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Threading.Tasks;

//namespace WebWriteCosmos.Controllers
//{

//    [ApiController]
//    [Route("[controller]")]
//    public class CosmosController : Controller
//    {

//        protected IConfiguration Configuration { get; }


//        private string EndpointUri = String.Empty;

//        // The primary key for the Azure Cosmos account.
//        private string PrimaryKey = String.Empty;

//        // The Cosmos client instance
//        private CosmosClient cosmosClient;

//        // The database we will create
//        private Database database;

//        // The container we will create.
//        private Container container;

//        // The name of the database and container we will create
//        private string databaseId = "curso1";
//        private string containerId = "laboratorioviernes";

//        public CosmosController()
//        {
            

//            this.EndpointUri = Configuration.GetSection("CosmosDb:EndpointUri").Value;
//            this.PrimaryKey = Configuration.GetSection("CosmosDb:PrimaryKey").Value;

//            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);

//        }

//        [Route("gestionarDbCn")]

//        private async Task gestionarDbCn()
//        {
//            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
//            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/genero", 400);
//        }

//        [Route("Create")]
//        public async Task<IActionResult> Create()
//        {

//           await gestionarDbCn();
//            Entidades.Pelicula pelicula = new Entidades.Pelicula
//            {
//                Nombre = "Sample",
//                Genero = "Sample",
//                Id = "1",
//                LugarAlmacenamiento = DateTime.Now
//            };

//            try
//            {
//                ItemResponse<Entidades.Pelicula> personResponse = await this.container.ReadItemAsync<Entidades.Pelicula>(pelicula.Id.ToString(), new PartitionKey(pelicula.Genero));
//                Console.WriteLine("Item in database with id: {0} already exists\n", personResponse.Resource.Id);
//            }
//            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
//            {
//                ItemResponse<Entidades.Pelicula> personaResponse = await this.container.CreateItemAsync<Entidades.Pelicula>(pelicula, new PartitionKey(pelicula.Genero));
//            }
//            return Content("¡EXITO!");
//        }


//        [Route("DeleteContainer")]
//        public async Task<IActionResult> DeleteContainer()
//        {
//            try
//            {
//                var personResponse = await this.container.DeleteContainerAsync();
//            }
//            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
//            {
//            }
//            return Content("¡EXITO!");
//        }




//    }
//}
