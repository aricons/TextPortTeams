﻿using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

using TextPortCore.Models;

namespace TextPortCore.Data
{
    public partial class TextPortContext : DbContext
    {
        public TextPortContext(DbContextOptions<TextPortContext> options) : base(options)
        {
        }

        public TextPortContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TextPortContext>();
            optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["TextPortContext"].ConnectionString);
            //TextPortContext context = new TextPortContext(optionsBuilder.Options);
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<AreaCode> AreaCodes { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<DedicatedVirtualNumber> DedicatedVirtualNumbers { get; set; }
        public virtual DbSet<ErrorLogItem> ErrorLog { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<GroupMember> GroupMembers { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<MMSFile> MMSFiles { get; set; }
        public virtual DbSet<PurchaseTransaction> PurchaseTransactions { get; set; }
        public virtual DbSet<SupportRequest> SupportRequests { get; set; }
        public virtual DbSet<VirtualNumberCountry> VirtualNumberCountries { get; set; }
        public virtual DbSet<Models.TimeZone> TimeZones { get; set; }

        //public static readonly LoggerFactory _loggerFactory = new LoggerFactory(new[] {
        //    new DebugLoggerProvider((category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Debug)
        //});

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            //var loggerFactory = new LoggerFactory();
            //loggerFactory.AddProvider(new DebugLoggerProvider());
            //loggerFactory.AddDebug();

            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseLoggerFactory(loggerFactory);
                optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["TextPortContext"].ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Accounts");

                entity.HasKey(e => e.AccountId);

                entity.HasIndex(e => e.UserName);

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.AccountValidationKey)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PasswordResetToken)
                   .HasMaxLength(40)
                   .IsUnicode(false);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Credits).HasDefaultValueSql("((0))");

                entity.Property(e => e.Balance).HasDefaultValueSql("((0))");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Enabled).HasDefaultValueSql("((0))");

                entity.Property(e => e.EnableEmailNotifications).HasDefaultValueSql("((0))");

                entity.Property(e => e.EnableMobileForwarding).HasDefaultValueSql("((0))");

                entity.Property(e => e.ForwardVnmessagesTo)
                    .HasColumnName("ForwardVNMessagesTo")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.LastLogin).HasColumnType("datetime");

                entity.Property(e => e.LoginCount).HasDefaultValueSql("((0))");

                entity.Property(e => e.MessageInCount).HasDefaultValueSql("((0))");

                entity.Property(e => e.MessageOutCount).HasDefaultValueSql("((0))");

                entity.Property(e => e.NotificationsEmailAddress)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.TimeZoneId)
                    .HasColumnName("TimeZoneID")
                    .HasDefaultValueSql("((5))");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                //entity.Property(e => e.ComplimentaryNumber).HasColumnType("bit");
            });

            modelBuilder.Entity<AreaCode>(entity =>
            {
                entity.ToTable("AreaCodes");

                entity.HasKey(e => e.AreaCodeId);

                entity.HasIndex(e => e.AreaCodeNum);

                entity.Property(e => e.AreaCodeNum)
                    .IsRequired()
                    .HasMaxLength(6)
                    .HasColumnName("AreaCode")
                    .IsUnicode(false);

                entity.Property(e => e.GeographicArea)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("Contacts");

                entity.HasKey(e => e.ContactId);

                entity.HasIndex(e => e.AccountId);

                entity.HasIndex(e => new { e.AccountId, e.Name });

                entity.Property(e => e.ContactId).HasColumnName("ContactID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.CarrierId).HasColumnName("CarrierID");

                entity.Property(e => e.DateAdded).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.MobileNumber)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DedicatedVirtualNumber>(entity =>
            {
                entity.ToTable("DedicatedVirtualNumbers");

                entity.HasKey(e => e.VirtualNumberId);

                entity.HasIndex(e => e.AccountId);

                entity.Property(e => e.VirtualNumberId).HasColumnName("VirtualNumberID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.CountryCode)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.ExpirationDate).HasColumnType("datetime");

                entity.Property(e => e.Fee).HasColumnType("money");

                entity.Property(e => e.Provider)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.SevenDayReminderSent).HasColumnType("datetime");

                entity.Property(e => e.TwoDayReminderSent).HasColumnType("datetime");

                entity.Property(e => e.VirtualNumber)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ErrorLogItem>(entity =>
            {
                entity.ToTable("ErrorLog");

                entity.HasKey(e => e.ErrorLogId);

                entity.Property(e => e.Details)
                    .HasMaxLength(1024)
                    .IsUnicode(false);

                entity.Property(e => e.ErrorDateTime).HasColumnType("datetime");

                entity.Property(e => e.ErrorMessage)
                    .IsRequired()
                    .HasMaxLength(1024)
                    .IsUnicode(false);

                entity.Property(e => e.ProgramName)
                    .IsRequired()
                    .HasMaxLength(80)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.ToTable("Groups");

                entity.HasKey(e => e.GroupId);

                entity.HasIndex(e => e.AccountId);

                entity.Property(e => e.GroupId).HasColumnName("GroupID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.GroupName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<GroupMember>(entity =>
            {
                entity.ToTable("GroupMembers");

                entity.HasKey(e => e.GroupMemberId);

                entity.HasIndex(e => e.GroupId);

                entity.Property(e => e.GroupMemberId).HasColumnName("GroupMemberID");

                entity.Property(e => e.CarrierId).HasColumnName("CarrierID");

                entity.Property(e => e.GroupId).HasColumnName("GroupID");

                entity.Property(e => e.MemberName)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.MobileNumber)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("MessagesOut");

                entity.HasKey(e => e.MessageId);

                entity.HasIndex(e => e.AccountId);

                entity.HasIndex(e => e.VirtualNumberId);

                entity.HasIndex(e => e.GatewayMessageId);

                entity.HasIndex(e => e.TimeStamp);

                entity.HasIndex(e => e.UniqueMessageId);

                entity.HasIndex(e => new { e.AccountId, e.MobileNumber });

                entity.Property(e => e.MessageId).HasColumnName("MessageID");

                entity.HasMany(e => e.MMSFiles).WithOne().HasForeignKey(e => e.MessageId);

                entity.HasMany(m => m.MMSFiles).WithOne().IsRequired(false);

                //entity.HasOne(u => u.Role).WithMany().HasForeignKey(u => u.RoleId).IsRequired(true);
                //.WithRequired()
                //.HasForeignKey(e => e.Mess);


                entity.Property(e => e.AccountId)
                    .HasColumnName("AccountID")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.VirtualNumberId)
                    .HasColumnName("VirtualNumberID")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.CarrierId).HasColumnName("CarrierID");

                entity.Property(e => e.DeleteFlag).HasColumnType("datetime");

                entity.Property(e => e.Delivered).HasColumnType("datetime");

                entity.Property(e => e.Direction).HasDefaultValueSql("((0))");

                entity.Property(e => e.MessageType).HasDefaultValueSql("((0))");

                entity.Property(e => e.FromEmail)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.FromNumber)
                   .HasMaxLength(30)
                   .IsUnicode(false);

                entity.Property(e => e.GatewayEmail)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.GatewayMessageId)
                    .HasColumnName("GatewayMessageID")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Ipaddress)
                    .IsRequired()
                    .HasColumnName("IPAddress")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.IsMMS)
                    .IsUnicode(false);

                entity.Property(e => e.MessageText)
                    .HasColumnName("Message")
                    .IsUnicode(true);

                entity.Property(e => e.MmsfileNames)
                    .HasColumnName("MMSFileName")
                    .HasMaxLength(120)
                    .IsUnicode(false);

                entity.Property(e => e.MobileNumber)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCost)
                    .HasColumnType("money")
                    .HasColumnName("CreditCost");

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.ProcessingMessage)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                //entity.Property(e => e.RoutingType)
                //    .HasMaxLength(10)
                //    .IsUnicode(false);

                entity.Property(e => e.SmtphostName)
                    .HasColumnName("SMTPHostName")
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Subject)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

                entity.Property(e => e.UniqueMessageId)
                    .HasColumnName("UniqueMessageID")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.VirtualNumber)
                    .HasMaxLength(18)
                    .IsUnicode(false);

                //entity.Ignore(e => e.MMSFiles);

                //this.HasRequired(t => t.V).WithMany(t => t.CustomerIntegrationTransactionReferences).HasForeignKey(d => d.CustomerIntegrationDetailID);
            });

            modelBuilder.Entity<MMSFile>(entity =>
            {
                entity.ToTable("MMSFiles");

                entity.HasKey(e => e.FileId);

                entity.Property(e => e.MessageId)
                   .HasDefaultValueSql("((0))");

                entity.Property(e => e.StorageId)
                  .HasDefaultValueSql("((1))");

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PurchaseTransaction>(entity =>
            {
                entity.ToTable("PurchaseTransactions");

                entity.HasKey(e => e.PurchaseId);

                entity.Property(e => e.PurchaseId).HasColumnName("PurchaseID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.Fee)
                    .IsRequired()
                    .HasColumnType("nchar(10)");

                entity.Property(e => e.GrossAmount).HasColumnType("money");

                entity.Property(e => e.ItemPurchased)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.PaymentService)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.ReceiverId)
                    .IsRequired()
                    .HasColumnName("ReceiverID")
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.TransactionDate).HasColumnType("datetime");

                entity.Property(e => e.TransactionId)
                    .IsRequired()
                    .HasColumnName("TransactionID")
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.TransactionType)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SupportRequest>(entity =>
            {
                entity.ToTable("SupportRequests");

                entity.HasKey(e => e.SupportId);

                entity.Property(e => e.SupportId).HasColumnName("SupportID");

                entity.Property(e => e.Category)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Ipaddress)
                    .HasColumnName("IPAddress")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.Message)
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.RequestorEmail)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            });

            modelBuilder.Entity<Models.TimeZone>(entity =>
            {
                entity.ToTable("TimeZones");

                entity.HasKey(e => e.TimeZoneId);

                entity.Property(e => e.TimeZoneId).HasColumnName("TimeZoneID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Utcoffset)
                    .HasColumnName("UTCOffset")
                    .HasColumnType("decimal(5, 2)");
            });

            modelBuilder.Entity<VirtualNumberCountry>(entity =>
            {
                entity.ToTable("VirtualNumberCountries");

                entity.HasKey(e => e.VirtualNumberCountryId);

                entity.HasIndex(e => e.CountryName);

                entity.Property(e => e.BaseCost).HasColumnType("money");

                entity.Property(e => e.CountryAlphaCode)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.CountryName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.CountryPhoneCode)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.MonthlyRate).HasColumnType("money");

                entity.Property(e => e.Provider)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });
        }
    }
}