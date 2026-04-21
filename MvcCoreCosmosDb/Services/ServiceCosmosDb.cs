using Microsoft.Azure.Cosmos;
using MvcCoreCosmosDb.Models;

namespace MvcCoreCosmosDb.Services
{
    public class ServiceCosmosDb
    {
        //DENTRO DE COSMOS SE TRABAJA CON LOS CONTAINERS
        //QUE ES DONDE ESTAN LOS ITEMS.
        //EN NUESTRO EJEMPLO TAMBIEN VAMOS A CREAR EL CONTAINER
        //POR LO QUE RECIBIREMOS EL COSMOS CLIENT

        private CosmosClient cosmosClient;
        private Container containerCosmos;

        public ServiceCosmosDb(CosmosClient cosmosClient, Container containerCosmos)
        {
            this.cosmosClient = cosmosClient;
            this.containerCosmos = containerCosmos;
        }

        //VAMOS A CREAR UN METODO PARA LA BASE DE DATOS
        //Y EL CONTENEDOR DE COSMOS DB EN CELESTE (AZURE)
        public async Task CreateDatabaseAsync()
        {
            await this.cosmosClient.CreateDatabaseIfNotExistsAsync("vehiculoscosmos");
            //CREAMOS UN CONTENEDOR PARA LA BBDD
            ContainerProperties properties = new ContainerProperties("containercoches", "/id");// el /id es la key que hemos indicado en el model
            await this.cosmosClient.GetDatabase("vehiculoscosmos").CreateContainerIfNotExistsAsync(properties);
        }

        public async Task CreateCocheAsync(Coche car)
        {
            //CUANDO INSERTAMOS, DEBEMOS INDICAR DE FORMA 
            //EXPLICITA EL DATO PARA EL PARTITION KEY
            await this.containerCosmos.CreateItemAsync<Coche>(car, new PartitionKey(car.Id));
        }

        //RECUPERAR TODOS LOS DATOS
        public async Task<List<Coche>> GetCochesAsync()
        {
            //NOSQL NUNCA SABE LOS REGISTROS TOTALES, NO FUNCIONA ASI
            //DEBEMOS RECORRER CON UN WHILE PARA EXTRAER LOS REGISTROS
            var query = this.containerCosmos.GetItemQueryIterator<Coche>();
            List<Coche> coches = new List<Coche>();
            while (query.HasMoreResults)
            {
                var result = await query.ReadNextAsync();
                //POR CADA RESULTADO, AGREGAMOS LA COLECCION
                //AUNQUE SOLO SEA COCHE A COCHE, DEVUELVE COMO SI
                //FUERAN VARIOS COCHES POR CADA COCHE, POR ESO USAMOS EL AddRange
                coches.AddRange(result);
            }
            return coches;
        }

        //BUSCAR COCHES. PARA BUSCAR SE HACE POR SU ID Y POR
        //SU PARTITION KEY, QUE EN NUESTRO CASO ES EL MISMO ID
        public async Task<Coche> FindCocheAsync(string id)
        {
            ItemResponse<Coche> response = await this.containerCosmos.ReadItemAsync<Coche>(id, new PartitionKey(id));
            return response.Resource;
        }


        public async Task DeleteCocheAsync(string id)
        {
            await this.containerCosmos.DeleteItemAsync<Coche>(id, new PartitionKey(id));
        }

        public async Task UpdateCocheAsync(Coche car)
        {
            //EXISTE UN METODO LLAMADO UPSERT Y QUE SI LO ENCUENTRA
            //LO MODIFICA Y SINO, LO CREA
            await this.containerCosmos.UpsertItemAsync<Coche>(car, new PartitionKey(car.Id));
        }

        public async Task<List<Coche>> GetCochesMarcaAsync(string marca)
        {
            string sql = $"SELECT * FROM c WHERE c.Marca = '{marca}'";
            QueryDefinition definition = new QueryDefinition(sql);
            var query = this.containerCosmos.GetItemQueryIterator<Coche>(definition);
            List<Coche> coches = new List<Coche>();
            while (query.HasMoreResults)
            {
                var result = await query.ReadNextAsync();
                coches.AddRange(result);
            }
            return coches;
        }
    }
}
