using Microsoft.EntityFrameworkCore;
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

            // Disable lazy loading.
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<APIApplication> APIApplications { get; set; }
        public virtual DbSet<AreaCode> AreaCodes { get; set; }
        public virtual DbSet<BadEmailDomain> BadEmailDomains { get; set; }
        public virtual DbSet<BlockedNumber> BlockedNumbers { get; set; }
        public virtual DbSet<BlogPost> BlogPosts { get; set; }
        public virtual DbSet<Carrier> Carriers { get; set; }
        public virtual DbSet<CarrierResponseCode> CarrierResponseCodes { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<DedicatedVirtualNumber> DedicatedVirtualNumbers { get; set; }
        public virtual DbSet<EmailToSMSAddress> EmailToSMSAddresses { get; set; }
        public virtual DbSet<ErrorLogItem> ErrorLog { get; set; }
        public virtual DbSet<FreeTextIPAddress> FreeTextIPAddresses { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<GroupMember> GroupMembers { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<MMSFile> MMSFiles { get; set; }
        public virtual DbSet<NpaNxxCity> NpaNxxCities { get; set; }
        public virtual DbSet<NpaNxxThou> NpaNxxThous { get; set; }
        public virtual DbSet<NumberPrice> NumberPricing { get; set; }
        public virtual DbSet<PurchaseTransaction> PurchaseTransactions { get; set; }
        public virtual DbSet<SupportRequest> SupportRequests { get; set; }
        public virtual DbSet<PooledNumber> PooledNumbers { get; set; }
        public virtual DbSet<Models.TimeZone> TimeZones { get; set; }
        public virtual DbSet<ZipLatLong> ZipLatLongs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
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

                entity.Property(e => e.Balance).HasDefaultValueSql("((0))");

                entity.Property(e => e.SMSSegmentCost).HasDefaultValueSql("((0))");

                entity.Property(e => e.MMSSegmentCost).HasDefaultValueSql("((0))");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Enabled).HasDefaultValueSql("((0))");

                entity.Property(e => e.RegisteredAsTrial).HasDefaultValueSql("((0))");

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

                entity.Property(e => e.ComplimentaryNumber).HasColumnType("byte");

                entity.HasOne(e => e.TimeZone).WithOne().HasForeignKey<TextPortCore.Models.TimeZone>(e => e.TimeZoneId);
            });

            modelBuilder.Entity<APIApplication>(entity =>
            {
                entity.ToTable("APIApplications");

                entity.HasKey(e => e.APIApplicationId);

                entity.HasIndex(e => e.AccountId);

                entity.Property(e => e.ApplicationName).IsRequired().HasMaxLength(80).IsUnicode(false);

                entity.Property(e => e.APIToken).IsRequired().HasMaxLength(80).IsUnicode(false);

                entity.Property(e => e.APISecret).IsRequired().HasMaxLength(80).IsUnicode(false);

                entity.Property(e => e.CallbackURL).HasMaxLength(250).IsUnicode(false);

                entity.Property(e => e.CallbackUserName).HasMaxLength(80).IsUnicode(false);

                entity.Property(e => e.CallbackPassword).HasMaxLength(80).IsUnicode(false);

                entity.Property(e => e.CallBackCredentialsRequired).IsRequired();
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

                entity.Property(e => e.TollFree).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<BadEmailDomain>(entity =>
            {
                entity.ToTable("BadEmailDomains");

                entity.HasKey(e => e.DomainId);
            });

            modelBuilder.Entity<BlockedNumber>(entity =>
            {
                entity.ToTable("BlockedNumbers");

                entity.HasKey(e => e.BlockID);

                entity.HasIndex(e => new { e.MobileNumber, e.Direction });

                entity.Property(e => e.DateRequested).HasColumnType("datetime");
            });

            modelBuilder.Entity<BlogPost>(entity =>
            {
                entity.ToTable("BlogPosts");

                entity.HasKey(e => e.PostId);

                entity.HasIndex(e => e.UrlName);
            });

            modelBuilder.Entity<Carrier>(entity =>
            {
                entity.ToTable("Carriers");

                entity.HasKey(e => e.CarrierId);

                // Probably don't need this. Leave out for simplicity. Keep as an example.
                //entity.HasMany(e => e.Countries).WithOne(e => e.Carrier).HasForeignKey(e => e.CarrierId).IsRequired();
            });

            modelBuilder.Entity<CarrierResponseCode>(entity =>
            {
                entity.ToTable("CarrierResponseCodes");

                entity.HasKey(e => e.ResponseCodeId);

                entity.HasIndex(e => e.ResponseCodeId);

                entity.HasIndex(e => new { e.CarrierId, e.ResponseCode });
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

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("Countries");

                entity.HasKey(e => e.CountryId);

                entity.HasIndex(e => e.CountryName);

                entity.HasIndex(e => e.SortOrder);

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

                entity.HasOne(e => e.Carrier).WithMany(e => e.Countries).HasForeignKey(e => e.CarrierId).IsRequired();
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

                entity.Property(e => e.NumberType).HasColumnType("tinyint").HasDefaultValueSql("((0))");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.ExpirationDate).HasColumnType("datetime");

                entity.Property(e => e.AutoRenew).HasDefaultValueSql("((0))");

                entity.Property(e => e.APIApplicationId);

                entity.Property(e => e.Fee).HasColumnType("money");

                //entity.Property(e => e.Provider)
                //    .IsRequired()
                //    .HasMaxLength(10)
                //    .IsUnicode(false);

                entity.Property(e => e.SevenDayReminderSent).HasColumnType("datetime");

                entity.Property(e => e.TwoDayReminderSent).HasColumnType("datetime");

                entity.Property(e => e.VirtualNumber)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(e => e.Carrier).WithOne();

                entity.HasOne(e => e.Country).WithMany();

                entity.HasOne(e => e.Account).WithMany();
            });

            modelBuilder.Entity<EmailToSMSAddress>(entity =>
            {
                entity.ToTable("EmailToSMSAddresses");

                entity.HasKey(e => e.AddressId);

                entity.HasIndex(e => e.AccountId);

                entity.HasIndex(e => e.EmailAddress);
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

                entity.Property(e => e.InnerException)
                   .IsRequired()
                   .HasMaxLength(1024)
                   .IsUnicode(false);

                entity.Property(e => e.ProgramName)
                    .IsRequired()
                    .HasMaxLength(80)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<FreeTextIPAddress>(entity =>
            {
                entity.ToTable("FreeTextIPAddresses");

                entity.HasKey(e => e.IpId);

                entity.HasIndex(e => e.IPAddress);
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

                entity.HasIndex(e => new { e.AccountId, e.MobileNumber });

                entity.Property(e => e.MessageId).HasColumnName("MessageID");

                entity.HasMany(e => e.MMSFiles).WithOne().HasForeignKey(e => e.MessageId).IsRequired(false);

                //entity.HasMany(m => m.MMSFiles).WithOne().IsRequired(false);

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.VirtualNumberId).HasColumnName("VirtualNumberID");

                entity.Property(e => e.DeleteFlag).HasColumnType("datetime");

                entity.Property(e => e.Direction).HasDefaultValueSql("((0))");

                entity.Property(e => e.MessageType).HasDefaultValueSql("((0))");

                entity.Property(e => e.Segments).HasColumnType("smallint");

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

                entity.Property(e => e.SessionId).HasColumnName("UniqueMessageId");

                entity.Property(e => e.MessageText)
                    .HasColumnName("Message")
                    .IsUnicode(true);

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

                entity.Property(e => e.FailureReason)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");
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

            modelBuilder.Entity<NpaNxxCity>(entity =>
            {
                entity.ToTable("NpaNxxCity");

                entity.HasKey(e => e.CityId);

                entity.HasIndex(e => new { e.NPA, e.NXX });
            });

            modelBuilder.Entity<NpaNxxThou>(entity =>
            {
                entity.ToTable("NpaNxxThou");

                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.NPA, e.NXX, e.Thousands });
            });

            modelBuilder.Entity<NumberPrice>(entity =>
            {
                entity.ToTable("NumberPricing");

                entity.HasKey(e => e.PriceId);

                entity.HasIndex(e => new { e.Enabled, e.CountryId, e.SortOrder });
            });

            modelBuilder.Entity<PurchaseTransaction>(entity =>
            {
                entity.ToTable("PurchaseTransactions");

                entity.HasKey(e => e.PurchaseId);

                entity.Property(e => e.PurchaseId).HasColumnName("PurchaseID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.Fee)
                    .IsRequired()
                    .HasColumnType("money");

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

                entity.Property(e => e.SendingNumber)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ReceivingNumber)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Message)
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.RequestorEmail)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.RequestorName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RequestType).HasColumnType("byte");

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            });

            modelBuilder.Entity<PooledNumber>(entity =>
            {
                entity.ToTable("PooledNumbers");

                entity.HasKey(e => e.PooledNumberId);

                entity.Property(e => e.CarrierId);

                entity.Property(e => e.VirtualNumber).HasMaxLength(20);

                entity.Property(e => e.Description).HasMaxLength(40);
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

            modelBuilder.Entity<ZipLatLong>(entity =>
            {
                entity.ToTable("ZipLatLong");

                entity.HasKey(e => e.ZipId);

                entity.HasIndex(e => new { e.Zip });
            });
        }
    }
}