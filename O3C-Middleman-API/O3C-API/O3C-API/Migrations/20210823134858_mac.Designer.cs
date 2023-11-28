﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using O3C_API.Models;

namespace O3C_API.Migrations
{
    [DbContext(typeof(DeviceContext))]
    [Migration("20210823134858_mac")]
    partial class mac
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.9")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("O3C_API.Models.AxisDevice", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Customer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Mac")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OAK")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AxisDevices");
                });

            modelBuilder.Entity("O3C_API.Models.O3CDevice", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("client_id")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("client_srcaddr")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("mac")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("o3c_server")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("O3CDevices");
                });
#pragma warning restore 612, 618
        }
    }
}
