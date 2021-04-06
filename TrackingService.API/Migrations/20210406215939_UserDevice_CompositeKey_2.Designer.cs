﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TrackingService.API.Database;

namespace TrackingService.API.Migrations
{
    [DbContext(typeof(TrackingDbContext))]
    [Migration("20210406215939_UserDevice_CompositeKey_2")]
    partial class UserDevice_CompositeKey_2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("tracking")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("TrackingService.Model.Objects.DbSet.UserDevice", b =>
                {
                    b.Property<int>("DeviceId")
                        .HasColumnType("integer")
                        .HasColumnName("device_id");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_user_device_user_id");

                    b.HasIndex("DeviceId", "UserId")
                        .IsUnique()
                        .HasDatabaseName("ix_user_device_device_id_user_id");

                    b.ToTable("user_device");
                });

            modelBuilder.Entity("TrackingService.Model.Objects.Device", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<bool>("Enabled")
                        .HasColumnType("boolean")
                        .HasColumnName("enabled");

                    b.Property<string>("Imei")
                        .HasColumnType("text")
                        .HasColumnName("imei");

                    b.Property<int>("LastPositionId")
                        .HasColumnType("integer")
                        .HasColumnName("last_position_id");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_devices");

                    b.HasIndex("Imei")
                        .HasDatabaseName("ix_devices_imei");

                    b.ToTable("devices");
                });

            modelBuilder.Entity("TrackingService.Model.Objects.Position", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date");

                    b.Property<decimal>("Direction")
                        .HasColumnType("numeric")
                        .HasColumnName("direction");

                    b.Property<string>("Imei")
                        .HasColumnType("text")
                        .HasColumnName("imei");

                    b.Property<decimal>("Lat")
                        .HasColumnType("numeric")
                        .HasColumnName("lat");

                    b.Property<decimal>("Lon")
                        .HasColumnType("numeric")
                        .HasColumnName("lon");

                    b.Property<string>("MiscInfo")
                        .HasColumnType("text")
                        .HasColumnName("misc_info");

                    b.Property<string>("Protocol")
                        .HasColumnType("text")
                        .HasColumnName("protocol");

                    b.Property<decimal>("Speed")
                        .HasColumnType("numeric")
                        .HasColumnName("speed");

                    b.HasKey("Id")
                        .HasName("pk_positions");

                    b.HasIndex("Imei")
                        .HasDatabaseName("ix_positions_imei");

                    b.ToTable("positions");
                });

            modelBuilder.Entity("TrackingService.Model.Objects.RefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("AddedDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("added_date");

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("expiry_date");

                    b.Property<bool>("IsRevoked")
                        .HasColumnType("boolean")
                        .HasColumnName("is_revoked");

                    b.Property<bool>("IsUsed")
                        .HasColumnType("boolean")
                        .HasColumnName("is_used");

                    b.Property<string>("JwtId")
                        .HasColumnType("text")
                        .HasColumnName("jwt_id");

                    b.Property<string>("Token")
                        .HasColumnType("text")
                        .HasColumnName("token");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_refresh_tokens");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_refresh_tokens_user_id");

                    b.ToTable("refresh_tokens");
                });

            modelBuilder.Entity("TrackingService.Model.Objects.TrackingUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer")
                        .HasColumnName("access_failed_count");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("text")
                        .HasColumnName("concurrency_stamp");

                    b.Property<string>("Email")
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean")
                        .HasColumnName("email_confirmed");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean")
                        .HasColumnName("lockout_enabled");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("lockout_end");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("text")
                        .HasColumnName("normalized_email");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("text")
                        .HasColumnName("normalized_user_name");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text")
                        .HasColumnName("password_hash");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text")
                        .HasColumnName("phone_number");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean")
                        .HasColumnName("phone_number_confirmed");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text")
                        .HasColumnName("security_stamp");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean")
                        .HasColumnName("two_factor_enabled");

                    b.Property<string>("UserName")
                        .HasColumnType("text")
                        .HasColumnName("user_name");

                    b.HasKey("Id")
                        .HasName("pk_tracking_user");

                    b.ToTable("tracking_user");
                });

            modelBuilder.Entity("TrackingService.Model.Objects.DbSet.UserDevice", b =>
                {
                    b.HasOne("TrackingService.Model.Objects.Device", "Device")
                        .WithMany()
                        .HasForeignKey("DeviceId")
                        .HasConstraintName("fk_user_device_devices_device_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TrackingService.Model.Objects.TrackingUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_device_tracking_user_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Device");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TrackingService.Model.Objects.RefreshToken", b =>
                {
                    b.HasOne("TrackingService.Model.Objects.TrackingUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_refresh_tokens_tracking_user_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
