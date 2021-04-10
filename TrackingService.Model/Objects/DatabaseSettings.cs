using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingService.Model.Objects {

    public class DatabaseSettings : IDatabaseSettings {
        public string ConnectionString { get; set; }
        public string HangfireString { get; set; }
        public int PositionDaysToKeep { get; set; }
    }

    public interface IDatabaseSettings {
        string ConnectionString { get; set; }
        string HangfireString { get; set; }
        public int PositionDaysToKeep { get; set; }
    }
}
