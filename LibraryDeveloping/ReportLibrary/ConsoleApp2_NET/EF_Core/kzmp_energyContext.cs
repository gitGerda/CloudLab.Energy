using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ConsoleApp2_NET
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

        public virtual DbSet<EnergyTable> EnergyTables { get; set; } = null!;
        public virtual DbSet<Meter> Meters { get; set; } = null!;
        public virtual DbSet<MsgNumber> MsgNumbers { get; set; } = null!;
        public virtual DbSet<PowerProfileM> PowerProfileMs { get; set; } = null!;
        public virtual DbSet<ReportsDetalization> ReportsDetalizations { get; set; } = null!;
        public virtual DbSet<ReportsHistory> ReportsHistories { get; set; } = null!;
        public virtual DbSet<UsersAuth> UsersAuths { get; set; } = null!;
        public virtual DbSet<UsersConfig> UsersConfigs { get; set; } = null!;

        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server = WS-OIT-007\\SQLEXPRESS;Database = kzmp_energy; User Id = TestUser; Password = qwerty852456;");
            }
        }*/
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server = MEMENTO_MORI\\SQLEXPRESS;Database = kzmp_energy; User Id = TestUser; Password = 852456;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EnergyTable>(entity =>
            {
                entity.HasKey(e => new { e.RowNumber, e.MeterId });

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

            modelBuilder.Entity<Meter>(entity =>
            {
                entity.HasKey(e => e.IdMeter);

                entity.ToTable("meter");

                entity.Property(e => e.IdMeter)
                    .ValueGeneratedNever()
                    .HasColumnName("id_meter");

                entity.Property(e => e.Address).HasColumnName("address");

                entity.Property(e => e.CenterName).IsUnicode(false);

                entity.Property(e => e.CompanyName).IsUnicode(false);

                entity.Property(e => e.Inn)
                    .IsUnicode(false)
                    .HasColumnName("INN");

                entity.Property(e => e.Inncenter)
                    .IsUnicode(false)
                    .HasColumnName("INNcenter");

                entity.Property(e => e.Interface).IsUnicode(false);

                entity.Property(e => e.MeasuringchannelA).IsUnicode(false);

                entity.Property(e => e.MeasuringchannelR).IsUnicode(false);

                entity.Property(e => e.MeasuringpointName).IsUnicode(false);

                entity.Property(e => e.Sim)
                    .IsUnicode(false)
                    .HasColumnName("SIM");

                entity.Property(e => e.TransformationRatio)
                    .IsUnicode(false)
                    .HasColumnName("Transformation_ratio");

                entity.Property(e => e.Type)
                    .IsUnicode(false)
                    .HasColumnName("type");

                entity.Property(e => e.Xml80020code)
                    .IsUnicode(false)
                    .HasColumnName("XML80020code");
            });

            modelBuilder.Entity<MsgNumber>(entity =>
            {
                entity.HasKey(e => e.CompanyInn);

                entity.ToTable("msg_number");

                entity.Property(e => e.CompanyInn)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CompanyINN");

                entity.Property(e => e.CompanyName).IsUnicode(false);

                entity.Property(e => e.Contract).IsUnicode(false);

                entity.Property(e => e.Date).IsUnicode(false);

                entity.Property(e => e.MsgNumber1)
                    .IsUnicode(false)
                    .HasColumnName("MsgNumber");
            });

            modelBuilder.Entity<PowerProfileM>(entity =>
            {
                entity.HasKey(e => new { e.RowNumber, e.Id });

                entity.ToTable("power_profile_m");

                entity.Property(e => e.RowNumber)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("rowNumber");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address).HasColumnName("address");

                entity.Property(e => e.Date)
                    .HasColumnType("date")
                    .HasColumnName("date");

                entity.Property(e => e.Time).HasColumnName("time");
            });

            modelBuilder.Entity<ReportsDetalization>(entity =>
            {
                entity.ToTable("reportsDetalization");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DetalizationType)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("detalizationType");

                entity.Property(e => e.ReportId).HasColumnName("reportId");

                entity.Property(e => e.TblColCheck)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("tblColCheck");

                entity.Property(e => e.TblColDateOfRead)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("tblColDateOfRead");

                entity.Property(e => e.TblColEnergySum)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("tblColEnergySum");

                entity.Property(e => e.TblColMeterAddress).HasColumnName("tblColMeterAddress");

                entity.Property(e => e.TblColMeterType)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("tblColMeterType");

                entity.Property(e => e.TblColPeriodEnd)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("tblColPeriodEnd");

                entity.Property(e => e.TblColPeriodStart)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("tblColPeriodStart");

                entity.Property(e => e.TblColPowerSum)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("tblColPowerSum");

                entity.Property(e => e.TblColTransformRatio)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("tblColTransformRatio");
            });

            modelBuilder.Entity<ReportsHistory>(entity =>
            {
                entity.ToTable("reportsHistory");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CompanyInn).HasDefaultValueSql("(N'')");

                entity.Property(e => e.CompanyName)
                    .HasMaxLength(1000)
                    .IsUnicode(false)
                    .HasColumnName("companyName");

                entity.Property(e => e.PeriodEndDate)
                    .HasColumnType("date")
                    .HasColumnName("periodEndDate");

                entity.Property(e => e.PeriodStartDate)
                    .HasColumnType("date")
                    .HasColumnName("periodStartDate");

                entity.Property(e => e.ReportsDate)
                    .HasColumnType("date")
                    .HasColumnName("reportsDate");
            });

            modelBuilder.Entity<UsersAuth>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.UserName });

                entity.ToTable("UsersAuth");

                entity.Property(e => e.UserId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("UserID");

                entity.Property(e => e.UserName)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Salt)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.UserPwd)
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UsersConfig>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.UserLogin, e.ConfigPropertyName });

                entity.ToTable("usersConfig");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.UserLogin)
                    .HasColumnName("userLogin")
                    .HasDefaultValueSql("(N'')");

                entity.Property(e => e.ConfigPropertyName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("configPropertyName")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.ConfigPropertyValue)
                    .HasMaxLength(1000)
                    .IsUnicode(false)
                    .HasColumnName("configPropertyValue");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
