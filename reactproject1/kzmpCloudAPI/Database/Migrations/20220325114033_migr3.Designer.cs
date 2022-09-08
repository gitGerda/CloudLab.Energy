﻿// <auto-generated />
using System;
using kzmpCloudAPI.Database.EF_Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace WebApplication1.Migrations
{
    [DbContext(typeof(kzmp_energyContext))]
    [Migration("20220325114033_migr3")]
    partial class migr3
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("WebApplication1.EnergyTable", b =>
                {
                    b.Property<int>("RowNumber")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("rowNumber");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RowNumber"), 1L, 1);

                    b.Property<int>("MeterId")
                        .HasColumnType("int")
                        .HasColumnName("MeterID");

                    b.Property<int>("Address")
                        .HasColumnType("int")
                        .HasColumnName("address");

                    b.Property<string>("Date")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("EndValue")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Month")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("StartValue")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Total")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("RowNumber", "MeterId");

                    b.ToTable("EnergyTable", (string)null);
                });

            modelBuilder.Entity("WebApplication1.Meter", b =>
                {
                    b.Property<int>("IdMeter")
                        .HasColumnType("int")
                        .HasColumnName("id_meter");

                    b.Property<int>("Address")
                        .HasColumnType("int")
                        .HasColumnName("address");

                    b.Property<string>("CenterName")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<string>("CompanyName")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<string>("Inn")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)")
                        .HasColumnName("INN");

                    b.Property<string>("Inncenter")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)")
                        .HasColumnName("INNcenter");

                    b.Property<string>("Interface")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<string>("MeasuringchannelA")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<string>("MeasuringchannelR")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<string>("MeasuringpointName")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<string>("Sim")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)")
                        .HasColumnName("SIM");

                    b.Property<string>("TransformationRatio")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)")
                        .HasColumnName("Transformation_ratio");

                    b.Property<string>("Type")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)")
                        .HasColumnName("type");

                    b.Property<string>("Xml80020code")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)")
                        .HasColumnName("XML80020code");

                    b.HasKey("IdMeter");

                    b.ToTable("meter", (string)null);
                });

            modelBuilder.Entity("WebApplication1.MsgNumber", b =>
                {
                    b.Property<string>("CompanyInn")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("CompanyINN");

                    b.Property<string>("CompanyName")
                        .HasColumnType("text");

                    b.Property<string>("Contract")
                        .HasColumnType("text");

                    b.Property<string>("Date")
                        .HasColumnType("text");

                    b.Property<string>("MsgNumber1")
                        .HasColumnType("text")
                        .HasColumnName("MsgNumber");

                    b.Property<string>("TestMigr")
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("msg_number", (string)null);
                });

            modelBuilder.Entity("WebApplication1.PowerProfileM", b =>
                {
                    b.Property<int>("RowNumber")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("rowNumber");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RowNumber"), 1L, 1);

                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<int>("Address")
                        .HasColumnType("int")
                        .HasColumnName("address");

                    b.Property<DateTime>("Date")
                        .HasColumnType("date")
                        .HasColumnName("date");

                    b.Property<double?>("Pminus")
                        .HasColumnType("float");

                    b.Property<double?>("Pplus")
                        .HasColumnType("float");

                    b.Property<double?>("Qminus")
                        .HasColumnType("float");

                    b.Property<double?>("Qplus")
                        .HasColumnType("float");

                    b.Property<object>("Time")
                        .IsRequired()
                        .HasColumnType("sql_variant")
                        .HasColumnName("time");

                    b.HasKey("RowNumber", "Id");

                    b.ToTable("power_profile_m", (string)null);
                });

            modelBuilder.Entity("WebApplication1.UsersAuth", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"), 1L, 1);

                    b.Property<string>("UserName")
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(500)");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(500)")
                        .HasDefaultValueSql("('')");

                    b.Property<string>("UserPwd")
                        .IsRequired()
                        .HasMaxLength(200)
                        .IsUnicode(false)
                        .HasColumnType("varchar(200)");

                    b.HasKey("UserId", "UserName");

                    b.ToTable("UsersAuth", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
