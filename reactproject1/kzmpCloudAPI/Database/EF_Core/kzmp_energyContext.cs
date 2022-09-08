using System;
using System.Collections.Generic;
using kzmpCloudAPI.Database.EF_Core.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace kzmpCloudAPI.Database.EF_Core
{
    public partial class kzmp_energyContext : DbContext
    {
        public kzmp_energyContext()
        {
        }

        public kzmp_energyContext(DbContextOptions<kzmp_energyContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CommunicInterface> CommunicInterfaces { get; set; } = null!;
        public virtual DbSet<CommunicPoint> CommunicPoints { get; set; } = null!;
        public virtual DbSet<EnergyTable> EnergyTables { get; set; } = null!;
        public virtual DbSet<FailedIndicationsTask> FailedIndicationsTasks { get; set; } = null!;
        public virtual DbSet<Meter> Meters { get; set; } = null!;
        public virtual DbSet<MetersType> MetersTypes { get; set; } = null!;
        public virtual DbSet<MsgNumber> MsgNumbers { get; set; } = null!;
        public virtual DbSet<PowerProfileM> PowerProfileMs { get; set; } = null!;
        public virtual DbSet<RabbitMQLog> RabbitMQLogs { get; set; } = null!;
        public virtual DbSet<ReportsDetalization> ReportsDetalizations { get; set; } = null!;
        public virtual DbSet<ReportsHistory> ReportsHistories { get; set; } = null!;
        public virtual DbSet<SessionInfo> SessionInfos { get; set; } = null!;
        public virtual DbSet<Shedule> Shedules { get; set; } = null!;
        public virtual DbSet<ShedulesLog> ShedulesLogs { get; set; } = null!;
        public virtual DbSet<ShedulesMeter> ShedulesMeters { get; set; } = null!;
        public virtual DbSet<UsersAuth> UsersAuths { get; set; } = null!;
        public virtual DbSet<UsersConfig> UsersConfigs { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<CommunicInterface>(entity =>
            {
                entity.ToTable("communic_interfaces");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(200)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<CommunicPoint>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Name })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("communic_points");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .HasColumnName("description");

                entity.Property(e => e.Port)
                    .HasMaxLength(10)
                    .HasColumnName("port");
            });

            modelBuilder.Entity<EnergyTable>(entity =>
            {
                entity.HasKey(e => new { e.RowNumber, e.MeterId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("EnergyTable");

                entity.Property(e => e.RowNumber)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("rowNumber");

                entity.Property(e => e.MeterId).HasColumnName("MeterID");

                entity.Property(e => e.Address).HasColumnName("address");

                entity.Property(e => e.Date).HasMaxLength(50);

                entity.Property(e => e.EndValue).HasMaxLength(50);

                entity.Property(e => e.Month).HasMaxLength(100);

                entity.Property(e => e.StartValue).HasMaxLength(50);

                entity.Property(e => e.Total).HasMaxLength(50);
            });

            modelBuilder.Entity<FailedIndicationsTask>(entity =>
            {
                entity.ToTable("failed_indications_tasks");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.LastDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("last_datetime");

                entity.Property(e => e.MeterId).HasColumnName("meter_id");

                entity.Property(e => e.SheduleId).HasColumnName("shedule_id");
            });

            modelBuilder.Entity<Meter>(entity =>
            {
                entity.HasKey(e => e.IdMeter)
                    .HasName("PRIMARY");

                entity.ToTable("meter");

                entity.Property(e => e.IdMeter)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id_meter");

                entity.Property(e => e.Address).HasColumnName("address");

                entity.Property(e => e.CenterName).HasMaxLength(500);

                entity.Property(e => e.CompanyName).HasMaxLength(500);

                entity.Property(e => e.Inn)
                    .HasMaxLength(500)
                    .HasColumnName("INN");

                entity.Property(e => e.Inncenter)
                    .HasMaxLength(500)
                    .HasColumnName("INNcenter");

                entity.Property(e => e.Interface).HasMaxLength(500);

                entity.Property(e => e.MeasuringchannelA).HasMaxLength(500);

                entity.Property(e => e.MeasuringchannelR).HasMaxLength(500);

                entity.Property(e => e.MeasuringpointName).HasMaxLength(500);

                entity.Property(e => e.Sim)
                    .HasMaxLength(500)
                    .HasColumnName("SIM");

                entity.Property(e => e.TransformationRatio)
                    .HasMaxLength(500)
                    .HasColumnName("Transformation_ratio");

                entity.Property(e => e.Type)
                    .HasMaxLength(500)
                    .HasColumnName("type");

                entity.Property(e => e.Xml80020code)
                    .HasMaxLength(500)
                    .HasColumnName("XML80020code");
            });

            modelBuilder.Entity<MetersType>(entity =>
            {
                entity.ToTable("meters_types");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Type)
                    .HasMaxLength(100)
                    .HasColumnName("type");
            });

            modelBuilder.Entity<MsgNumber>(entity =>
            {
                entity.HasKey(e => e.CompanyInn)
                    .HasName("PRIMARY");

                entity.ToTable("msg_number");

                entity.Property(e => e.CompanyInn)
                    .HasMaxLength(50)
                    .HasColumnName("CompanyINN");

                entity.Property(e => e.CompanyName).HasMaxLength(500);

                entity.Property(e => e.Contract).HasMaxLength(500);

                entity.Property(e => e.Date).HasMaxLength(500);

                entity.Property(e => e.MsgNumber1)
                    .HasMaxLength(500)
                    .HasColumnName("MsgNumber");
            });

            modelBuilder.Entity<PowerProfileM>(entity =>
            {
                entity.HasKey(e => new { e.RowNumber, e.Id })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("power_profile_m");

                entity.Property(e => e.RowNumber)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("rowNumber");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address).HasColumnName("address");

                entity.Property(e => e.Date).HasColumnName("date");

                entity.Property(e => e.Time)
                    .HasMaxLength(6)
                    .HasColumnName("time");
            });

            modelBuilder.Entity<RabbitMQLog>(entity =>
            {
                entity.ToTable("RabbitMQLogs");

                entity.Property(e => e.Date).HasMaxLength(6);

                entity.Property(e => e.Status).HasMaxLength(100);
            });

            modelBuilder.Entity<ReportsDetalization>(entity =>
            {
                entity.ToTable("reportsDetalization");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DetalizationType)
                    .HasMaxLength(50)
                    .HasColumnName("detalizationType");

                entity.Property(e => e.ReportId).HasColumnName("reportId");

                entity.Property(e => e.TblColCheck)
                    .HasMaxLength(50)
                    .HasColumnName("tblColCheck");

                entity.Property(e => e.TblColDateOfRead)
                    .HasMaxLength(50)
                    .HasColumnName("tblColDateOfRead");

                entity.Property(e => e.TblColEnergySum)
                    .HasMaxLength(50)
                    .HasColumnName("tblColEnergySum");

                entity.Property(e => e.TblColMeterAddress).HasColumnName("tblColMeterAddress");

                entity.Property(e => e.TblColMeterType)
                    .HasMaxLength(500)
                    .HasColumnName("tblColMeterType");

                entity.Property(e => e.TblColPeriodEnd)
                    .HasMaxLength(50)
                    .HasColumnName("tblColPeriodEnd");

                entity.Property(e => e.TblColPeriodStart)
                    .HasMaxLength(50)
                    .HasColumnName("tblColPeriodStart");

                entity.Property(e => e.TblColPowerSum)
                    .HasMaxLength(50)
                    .HasColumnName("tblColPowerSum");

                entity.Property(e => e.TblColTransformRatio)
                    .HasMaxLength(50)
                    .HasColumnName("tblColTransformRatio");
            });

            modelBuilder.Entity<ReportsHistory>(entity =>
            {
                entity.ToTable("reportsHistory");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CompanyInn).HasMaxLength(500);

                entity.Property(e => e.CompanyName)
                    .HasMaxLength(1000)
                    .HasColumnName("companyName");

                entity.Property(e => e.PeriodEndDate).HasColumnName("periodEndDate");

                entity.Property(e => e.PeriodStartDate).HasColumnName("periodStartDate");

                entity.Property(e => e.RecordCreateDate)
                    .HasMaxLength(6)
                    .HasColumnName("recordCreateDate");

                entity.Property(e => e.ReportsDate).HasColumnName("reportsDate");

                entity.Property(e => e.ReportPath).HasMaxLength(600).HasColumnName("reportPath");
            });

            modelBuilder.Entity<SessionInfo>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.SessionToken, e.DeviceId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 255, 0 });

                entity.ToTable("SessionInfo");

                entity.Property(e => e.SessionToken).HasMaxLength(500);

                entity.Property(e => e.DeviceId).HasMaxLength(50);

                entity.Property(e => e.AuthTime).HasMaxLength(6);
            });

            modelBuilder.Entity<Shedule>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Name })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("shedules");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.CommunicPointId).HasColumnName("communic_point_id");

                entity.Property(e => e.CreatingDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creating_date");

                entity.Property(e => e.Shedule1)
                    .HasMaxLength(100)
                    .HasColumnName("shedule");

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<ShedulesLog>(entity =>
            {
                entity.ToTable("shedules_logs");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("date_time");

                entity.Property(e => e.Description)
                    .HasMaxLength(200)
                    .HasColumnName("description");

                entity.Property(e => e.SheduleId).HasColumnName("shedule_id");

                entity.Property(e => e.Status)
                    .HasMaxLength(10)
                    .HasColumnName("status");
            });

            modelBuilder.Entity<ShedulesMeter>(entity =>
            {
                entity.ToTable("shedules_meters");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.MeterId).HasColumnName("meter_id");

                entity.Property(e => e.SheduleId).HasColumnName("shedule_id");
            });

            modelBuilder.Entity<UsersAuth>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.UserName })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 255 });

                entity.ToTable("UsersAuth");

                entity.Property(e => e.UserId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("UserID");

                entity.Property(e => e.UserName).HasMaxLength(500);

                entity.Property(e => e.Salt)
                    .HasMaxLength(500)
                    .HasDefaultValueSql("''");

                entity.Property(e => e.UserPwd).HasMaxLength(200);
            });

            modelBuilder.Entity<UsersConfig>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.UserLogin, e.ConfigPropertyName })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 255, 255 });

                entity.ToTable("usersConfig");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.UserLogin)
                    .HasMaxLength(450)
                    .HasColumnName("userLogin");

                entity.Property(e => e.ConfigPropertyName)
                    .HasMaxLength(500)
                    .HasColumnName("configPropertyName")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.ConfigPropertyValue)
                    .HasMaxLength(1000)
                    .HasColumnName("configPropertyValue");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
