using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackingService.Model.Objects;

namespace TrackingService.API.Database {
	public class TrackingDbContext : DbContext {
		public DbSet<Position> Positions { get; set; }
		public DbSet<Device> Devices { get; set; }
		public DbSet<RefreshToken> RefreshTokens { get; set; }

		public TrackingDbContext(DbContextOptions<TrackingDbContext> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder b) {
			base.OnModelCreating(b);
			b.HasDefaultSchema("tracking");

			// configure Positions so MiscInfo is stored as json string
			b.ApplyConfiguration(new PositionsConfiguration());
			b.Entity<Position>().HasIndex(x => x.Imei);
			b.Entity<Device>().HasIndex(x => x.Imei);
		}

		protected override void OnConfiguring(DbContextOptionsBuilder o) {
			base.OnConfiguring(o);
			o.UseSnakeCaseNamingConvention();
		}
	}
}
