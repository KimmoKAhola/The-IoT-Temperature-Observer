﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PlantAPI;

#nullable disable

namespace PlantAPI.Migrations
{
    [DbContext(typeof(PlantContext))]
    partial class PlantContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PlantAPI.Models.Message", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MessageId"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserChatId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("MessageId");

                    b.HasIndex("UserChatId");

                    b.ToTable("UserMessages");
                });

            modelBuilder.Entity("PlantAPI.Models.PlantData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DHT_Humidity")
                        .HasColumnType("int");

                    b.Property<int>("DHT_Temperature")
                        .HasColumnType("int");

                    b.Property<string>("SensorId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("Temperature")
                        .HasColumnType("float");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("SensorId");

                    b.ToTable("PlantData");
                });

            modelBuilder.Entity("PlantAPI.Models.Sensor", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Sensors");
                });

            modelBuilder.Entity("PlantAPI.Models.User", b =>
                {
                    b.Property<string>("UserChatId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsSubscriber")
                        .HasColumnType("bit");

                    b.HasKey("UserChatId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PlantAPI.Models.Message", b =>
                {
                    b.HasOne("PlantAPI.Models.User", "User")
                        .WithMany("Messages")
                        .HasForeignKey("UserChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PlantAPI.Models.PlantData", b =>
                {
                    b.HasOne("PlantAPI.Models.Sensor", "Sensor")
                        .WithMany()
                        .HasForeignKey("SensorId");

                    b.Navigation("Sensor");
                });

            modelBuilder.Entity("PlantAPI.Models.User", b =>
                {
                    b.Navigation("Messages");
                });
#pragma warning restore 612, 618
        }
    }
}
