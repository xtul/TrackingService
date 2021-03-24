using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingService.Model.Objects {

    public class DatabaseSettings : IDatabaseSettings {
        public string PositionsName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IDatabaseSettings {
        string PositionsName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
