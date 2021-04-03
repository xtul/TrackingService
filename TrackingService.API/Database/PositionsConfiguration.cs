using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackingService.Model.Objects;

namespace TrackingService.API.Database {
    public class PositionsConfiguration : IEntityTypeConfiguration<Position> {
        public void Configure(EntityTypeBuilder<Position> builder) {
            // This Converter will perform the conversion to and from Json to the desired type
            builder.Property(e => e.MiscInfo).HasConversion(
                v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                v => JsonConvert.DeserializeObject<MiscInfo>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }
    }
}
