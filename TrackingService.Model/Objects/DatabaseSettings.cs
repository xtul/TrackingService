using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingService.Model.Objects {

    public class DatabaseSettings : IDatabaseSettings {
        public string DevicesName { get; set; }
        public string PositionsName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IDatabaseSettings {
        string DevicesName { get; set; }
        string PositionsName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }

        /// <summary>
        /// Gets collection name by string.
        /// </summary>
        /// <param name="collection">Class name, like "Position".</param>
        /// <returns></returns>
        public string GetCollectionByString(string collection) {
			return collection switch {
				"Position" => PositionsName,
				"Device" => DevicesName,
				_ => throw new ArgumentException($"No collection could be found by string \"{collection}\"."),
			};
		}
    }
}
