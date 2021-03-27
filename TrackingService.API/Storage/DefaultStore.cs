using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingService.Model.Objects;

namespace TrackingService.API.Storage {
    /// <summary>
    /// Default MongoDB collection CRUD operations.
    /// </summary>
	public abstract class DefaultStore<T> where T : TrackingServiceCollection {
        protected readonly IMongoCollection<T> _mongoCollection;

        public DefaultStore(IDatabaseSettings settings) {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            var collectionName = typeof(T).Name;

            _mongoCollection = database.GetCollection<T>(settings.GetCollectionByString(collectionName));
        }

        public List<T> GetAll() =>
            _mongoCollection.Find(x => true).ToList();

        public T GetByImei(string imei) =>
            _mongoCollection.Find(x => x.Imei == imei).FirstOrDefault();

        public T GetById(string id) =>
            _mongoCollection.Find(x => x.Id == id).FirstOrDefault();

        public T Get(Position position) => GetById(position.Id);

        public T Add(T collectionItem) {
            _mongoCollection.InsertOne(collectionItem);
            return collectionItem;
        }

        public async Task<IEnumerable<T>> AddManyAsync(IEnumerable<T> collectionItems) {
            await _mongoCollection.InsertManyAsync(collectionItems);
            return collectionItems;
		}

        public void Update(string id, T collectionItem) =>
            _mongoCollection.ReplaceOne(x => x.Id == id, collectionItem);

        public void Remove(T collectionItem) =>
            _mongoCollection.DeleteOne(x => x.Id == collectionItem.Id);

        public void Remove(string id) =>
            _mongoCollection.DeleteOne(x => x.Id == id);
    }
}
