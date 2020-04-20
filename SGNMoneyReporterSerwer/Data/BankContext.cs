using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SGNMoneyReporterSerwer.Data.Entities
{
    public partial class BankContext : DbContext
    {
        public BankContext()
        {
        }

        public BankContext(DbContextOptions<BankContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cashier> Cashier { get; set; }
        public virtual DbSet<CountDetail> CountDetail { get; set; }
        public virtual DbSet<CountResult> CountResult { get; set; }
        public virtual DbSet<CountSummary> CountSummary { get; set; }
        public virtual DbSet<Currency> Currency { get; set; }
        public virtual DbSet<CurrencyFaceValue> CurrencyFaceValue { get; set; }
        public virtual DbSet<FileHistory> FileHistory { get; set; }
        public virtual DbSet<Machine> Machine { get; set; }
        public virtual DbSet<Mode> Mode { get; set; }
        public virtual DbSet<Parameter> Parameter { get; set; }
        public virtual DbSet<Quality> Quality { get; set; }
        public virtual DbSet<RefreshToken> RefreshToken { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=ConnectionDB");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cashier>(entity =>
            {
                entity.HasKey(e => e.IdCashier);

                entity.ToTable("Cashier", "Configuration");

                entity.Property(e => e.CashierName)
                    .IsRequired()
                    .HasMaxLength(128);
            });

            modelBuilder.Entity<CountDetail>(entity =>
            {
                entity.HasKey(e => e.IdCountDetail);

                entity.ToTable("CountDetail", "Report");

                entity.Property(e => e.BanknoteSn)
                    .HasColumnName("BanknoteSN")
                    .HasMaxLength(16);

                entity.HasOne(d => d.IdCountResultNavigation)
                    .WithMany(p => p.CountDetail)
                    .HasForeignKey(d => d.IdCountResult)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CountDetail_CountResult");

                entity.HasOne(d => d.IdCurrencyNavigation)
                    .WithMany(p => p.CountDetail)
                    .HasForeignKey(d => d.IdCurrency)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CountDetail_Currency");

                entity.HasOne(d => d.IdCurrencyFaceValueNavigation)
                    .WithMany(p => p.CountDetail)
                    .HasForeignKey(d => d.IdCurrencyFaceValue)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CountDetail_CurrencyFaceValue");
            });

            modelBuilder.Entity<CountResult>(entity =>
            {
                entity.HasKey(e => e.IdCountResult);

                entity.ToTable("CountResult", "Report");

                entity.Property(e => e.BagNo).HasMaxLength(32);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.MachineSoftVer).HasMaxLength(16);

                entity.Property(e => e.SavedDate).HasColumnType("datetime");

                entity.HasOne(d => d.IdCashierNavigation)
                    .WithMany(p => p.CountResult)
                    .HasForeignKey(d => d.IdCashier)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CountResult_Cashier");

                entity.HasOne(d => d.IdMachineNavigation)
                    .WithMany(p => p.CountResult)
                    .HasForeignKey(d => d.IdMachine)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CountResult_Machine");

                entity.HasOne(d => d.IdModeNavigation)
                    .WithMany(p => p.CountResult)
                    .HasForeignKey(d => d.IdMode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CountResult_Mode");
            });

            modelBuilder.Entity<CountSummary>(entity =>
            {
                entity.HasKey(e => e.IdCountSummary);

                entity.ToTable("CountSummary", "Report");

                entity.HasOne(d => d.IdCountResultNavigation)
                    .WithMany(p => p.CountSummary)
                    .HasForeignKey(d => d.IdCountResult)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CountSummary_CountResult");

                entity.HasOne(d => d.IdCurrencyFaceValueNavigation)
                    .WithMany(p => p.CountSummary)
                    .HasForeignKey(d => d.IdCurrencyFaceValue)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CountSummary_CurrencyFaceValue");

                entity.HasOne(d => d.IdQualityNavigation)
                    .WithMany(p => p.CountSummary)
                    .HasForeignKey(d => d.IdQuality)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CountSummary_Quality");
            });

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.HasKey(e => e.IdCurrency)
                    .HasName("PK_Dictionary.Currency");

                entity.ToTable("Currency", "Dictionary");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Symbol)
                    .IsRequired()
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<CurrencyFaceValue>(entity =>
            {
                entity.HasKey(e => e.IdCurrencyFaceValue);

                entity.ToTable("CurrencyFaceValue", "Dictionary");

                entity.Property(e => e.FaceValue).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<FileHistory>(entity =>
            {
                entity.HasKey(e => e.IdFileHistory);

                entity.ToTable("FileHistory", "Report");

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.ProcessDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.IdCountResultNavigation)
                    .WithMany(p => p.FileHistory)
                    .HasForeignKey(d => d.IdCountResult)
                    .HasConstraintName("FK_FileHistory_CountResult");
            });

            modelBuilder.Entity<Machine>(entity =>
            {
                entity.HasKey(e => e.IdMachine);

                entity.ToTable("Machine", "Configuration");

                entity.Property(e => e.Model)
                    .IsRequired()
                    .HasMaxLength(16);

                entity.Property(e => e.Sn)
                    .IsRequired()
                    .HasColumnName("SN")
                    .HasMaxLength(16);

                entity.Property(e => e.SoftwareVersion).HasMaxLength(16);
            });

            modelBuilder.Entity<Mode>(entity =>
            {
                entity.HasKey(e => e.IdMode);

                entity.ToTable("Mode", "Dictionary");

                entity.Property(e => e.ModeValue)
                    .IsRequired()
                    .HasMaxLength(32);
            });

            modelBuilder.Entity<Parameter>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Parameter", "Configuration");

                entity.Property(e => e.IdParameter).ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Quality>(entity =>
            {
                entity.HasKey(e => e.IdQuality);

                entity.ToTable("Quality", "Dictionary");

                entity.Property(e => e.QualityValue)
                    .IsRequired()
                    .HasMaxLength(16);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.TokenId);

                entity.ToTable("RefreshToken", "Configuration");

                entity.Property(e => e.TokenId).HasColumnName("token_id");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnName("expiry_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasColumnName("token")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.RefreshTokens)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__RefreshTo__user___03F0984C");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.IdUser);

                entity.ToTable("User", "Configuration");

                entity.Property(e => e.LastEditDate).HasColumnType("datetime");

                entity.Property(e => e.UserEmailAddress).HasMaxLength(255);

                entity.Property(e => e.UserLastName)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.UserPassword).HasMaxLength(255);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
