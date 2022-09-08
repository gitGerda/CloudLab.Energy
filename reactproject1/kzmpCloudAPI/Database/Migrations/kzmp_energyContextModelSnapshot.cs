﻿// <auto-generated />
using System;
using kzmpCloudAPI.Database.EF_Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace WebApplication1.Migrations
{
    [DbContext(typeof(kzmp_energyContext))]
    partial class kzmp_energyContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("WebApplication1.EF_Core.Tables.RabbitMQLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime");

                    b.Property<string>("Message")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<string>("Status")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.HasKey("Id");

                    b.ToTable("RabbitMQLogs", (string)null);
                });

            modelBuilder.Entity("WebApplication1.EF_Core.Tables.SessionInfo", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("DeviceId")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("SessionToken")
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(500)");

                    b.Property<DateTime>("AuthTime")
                        .HasColumnType("datetime");

                    b.HasKey("UserId", "DeviceId", "SessionToken");

                    b.ToTable("SessionInfo", (string)null);
                });

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

                    b.Property<int?>("Address")
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
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("CompanyINN");

                    b.Property<string>("CompanyName")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<string>("Contract")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<string>("Date")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<string>("MsgNumber1")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)")
                        .HasColumnName("MsgNumber");

                    b.Property<string>("TestMigr")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CompanyInn");

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

                    b.Property<TimeSpan>("Time")
                        .HasColumnType("time")
                        .HasColumnName("time");

                    b.HasKey("RowNumber", "Id");

                    b.ToTable("power_profile_m", (string)null);
                });

            modelBuilder.Entity("WebApplication1.ReportsDetalization", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("DetalizationType")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("detalizationType");

                    b.Property<int>("ReportId")
                        .HasColumnType("int")
                        .HasColumnName("reportId");

                    b.Property<string>("TblColCheck")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("tblColCheck");

                    b.Property<string>("TblColDateOfRead")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("tblColDateOfRead");

                    b.Property<string>("TblColEnergySum")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("tblColEnergySum");

                    b.Property<int?>("TblColMeterAddress")
                        .HasColumnType("int")
                        .HasColumnName("tblColMeterAddress");

                    b.Property<string>("TblColMeterType")
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(500)")
                        .HasColumnName("tblColMeterType");

                    b.Property<string>("TblColPeriodEnd")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("tblColPeriodEnd");

                    b.Property<string>("TblColPeriodStart")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("tblColPeriodStart");

                    b.Property<string>("TblColPowerSum")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("tblColPowerSum");

                    b.Property<string>("TblColTransformRatio")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("tblColTransformRatio");

                    b.HasKey("Id");

                    b.ToTable("reportsDetalization", (string)null);
                });

            modelBuilder.Entity("WebApplication1.ReportsHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("CompanyInn")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .IsUnicode(false)
                        .HasColumnType("varchar(1000)")
                        .HasColumnName("companyName");

                    b.Property<DateTime>("PeriodEndDate")
                        .HasColumnType("date")
                        .HasColumnName("periodEndDate");

                    b.Property<DateTime>("PeriodStartDate")
                        .HasColumnType("date")
                        .HasColumnName("periodStartDate");

                    b.Property<DateTime>("RecordCreateDate")
                        .HasColumnType("datetime")
                        .HasColumnName("recordCreateDate");

                    b.Property<DateTime>("ReportsDate")
                        .HasColumnType("date")
                        .HasColumnName("reportsDate");

                    b.HasKey("Id");

                    b.ToTable("reportsHistory", (string)null);
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

            modelBuilder.Entity("WebApplication1.UsersConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("UserLogin")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("userLogin");

                    b.Property<string>("ConfigPropertyName")
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(500)")
                        .HasColumnName("configPropertyName");

                    b.Property<string>("ConfigPropertyValue")
                        .HasMaxLength(1000)
                        .IsUnicode(false)
                        .HasColumnType("varchar(1000)")
                        .HasColumnName("configPropertyValue");

                    b.HasKey("Id", "UserLogin", "ConfigPropertyName");

                    b.ToTable("usersConfig", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}