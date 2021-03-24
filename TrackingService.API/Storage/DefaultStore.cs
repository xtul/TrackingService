using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingService.Model.Objects;

namespace TrackingService.API.Storage {
	public abstract class DefaultStore<T> where T : TrackingCollection {
        private readonly IMongoCollection<T> _mongoCollection;

        public DefaultStore(IDatabaseSettings settings) {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _mongoCollection = database.GetCollection<T>(settings.PositionsName);
        }

        public List<T> Get() =>
            _mongoCollection.Find(x => true).ToList();

        public T Get(string id) =>
            _mongoCollection.Find(x => x.Id == id).FirstOrDefault();

        public T Create(T collectionItem) {
            _mongoCollection.InsertOne(collectionItem);
            return collectionItem;
        }

        public async Task<IEnumerable<T>> Create(IEnumerable<T> collectionItems) {
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
