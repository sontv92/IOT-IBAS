using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace IOITWebApp.Models.EF
{
    public partial class CNTTVNWebContext : DbContext
    {
        public CNTTVNWebContext()
        {
        }

        public CNTTVNWebContext(DbContextOptions<CNTTVNWebContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Action> Action { get; set; }
        public virtual DbSet<Attactment> Attactment { get; set; }
        public virtual DbSet<Attribuite> Attribuite { get; set; }
        public virtual DbSet<Bank> Bank { get; set; }
        public virtual DbSet<Block> Block { get; set; }
        public virtual DbSet<Branch> Branch { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<CategoryMapping> CategoryMapping { get; set; }
        public virtual DbSet<CategoryRank> CategoryRank { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<Config> Config { get; set; }
        public virtual DbSet<ConfigTable> ConfigTable { get; set; }
        public virtual DbSet<ConfigTableItem> ConfigTableItem { get; set; }
        public virtual DbSet<ConfigThumb> ConfigThumb { get; set; }
        public virtual DbSet<Contact> Contact { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<CustomerAddress> CustomerAddress { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<District> District { get; set; }
        public virtual DbSet<Function> Function { get; set; }
        public virtual DbSet<FunctionRole> FunctionRole { get; set; }
        public virtual DbSet<Language> Language { get; set; }
        public virtual DbSet<Manufacturer> Manufacturer { get; set; }
        public virtual DbSet<Menu> Menu { get; set; }
        public virtual DbSet<MenuItem> MenuItem { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderItem> OrderItem { get; set; }
        public virtual DbSet<PaymentHistory> PaymentHistory { get; set; }
        public virtual DbSet<Position> Position { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductAttribuite> ProductAttribuite { get; set; }
        public virtual DbSet<ProductCustomer> ProductCustomer { get; set; }
        public virtual DbSet<ProductImage> ProductImage { get; set; }
        public virtual DbSet<ProductReview> ProductReview { get; set; }
        public virtual DbSet<Province> Province { get; set; }
        public virtual DbSet<Related> Related { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Slide> Slide { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }
        public virtual DbSet<TypeAttribute> TypeAttribute { get; set; }
        public virtual DbSet<TypeAttributeItem> TypeAttributeItem { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserMapping> UserMapping { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<Wards> Wards { get; set; }
        public virtual DbSet<Website> Website { get; set; }
        public virtual DbSet<DATHANG_TEMP> DATHANG_TEMP { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder()
                                   .SetBasePath(Directory.GetCurrentDirectory())
                                   .AddJsonFile("appsettings.json");

                var configuration = builder.Build();
                string con = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(con);
            }
        }
        public static void RunSQLNonQueryNoShowError(string sqlNonQuery)
        {
            var builder = new ConfigurationBuilder()
                                   .SetBasePath(Directory.GetCurrentDirectory())
                                   .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            string con1 = configuration.GetConnectionString("DefaultConnection");
            SqlConnection con = new SqlConnection(con1);
            try
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandTimeout = 1000 * 10;

                SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter();
                DataTable dt = new System.Data.DataTable();

                cmd.CommandText = sqlNonQuery;
                cmd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
            }
            finally
            {
                con.Close();
            }
        }
        public static DataTable GetDataBySql(string selectSQL)
        {
            var builder = new ConfigurationBuilder()
                                  .SetBasePath(Directory.GetCurrentDirectory())
                                  .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            string con1 = configuration.GetConnectionString("DefaultConnection");
            SqlConnection con = new SqlConnection(con1);
            try
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand(); cmd.CommandTimeout = 180;

                cmd.CommandText = string.Format("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");
                cmd.ExecuteNonQuery();

                SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter();
                DataTable dt = new System.Data.DataTable();

                cmd.CommandText = selectSQL;
                da.SelectCommand = cmd;

                da.Fill(dt);
                con.Close();
                return dt;
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                con.Close();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Action>(entity =>
            {
                entity.Property(e => e.ActionName).HasMaxLength(500);

                entity.Property(e => e.ActionType).HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Ipaddress)
                    .HasColumnName("IPAddress")
                    .HasMaxLength(50);

                entity.Property(e => e.Logs).HasColumnType("ntext");

                entity.Property(e => e.TargetType).HasMaxLength(1000);
            });

            modelBuilder.Entity<Attactment>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.Thumb).HasColumnType("ntext");

                entity.Property(e => e.Url).HasColumnType("ntext");
            });

            modelBuilder.Entity<Attribuite>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Bank>(entity =>
            {
                entity.Property(e => e.AccountId).HasMaxLength(100);

                entity.Property(e => e.AccountName).HasMaxLength(500);

                entity.Property(e => e.BranchName).HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.Note).HasColumnType("ntext");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Block>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.Contents).HasColumnType("ntext");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Icon).HasMaxLength(50);

                entity.Property(e => e.IconFa).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Branch>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.Avatar).HasMaxLength(500);

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.Contents).HasColumnType("ntext");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(500);

                entity.Property(e => e.Lat)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Long)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.Contents).HasColumnType("ntext");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.Icon).HasMaxLength(50);

                entity.Property(e => e.IconFa).HasMaxLength(50);

                entity.Property(e => e.Image).HasMaxLength(2000);

                entity.Property(e => e.MetaDescription).HasMaxLength(500);

                entity.Property(e => e.MetaKeyword).HasMaxLength(300);

                entity.Property(e => e.MetaTitle).HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Url).HasMaxLength(1000);
            });

            modelBuilder.Entity<CategoryMapping>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<CategoryRank>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("ntext");

                entity.Property(e => e.Icon).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.Contents).HasColumnType("ntext");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(e => e.Address).HasColumnType("ntext");

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.ContactEmail).HasMaxLength(500);

                entity.Property(e => e.ContactName).HasMaxLength(200);

                entity.Property(e => e.ContactPhone).HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(500);

                entity.Property(e => e.Fax).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.Representative).HasMaxLength(200);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Config>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.EmailColorBody).HasMaxLength(200);

                entity.Property(e => e.EmailColorFooter).HasMaxLength(200);

                entity.Property(e => e.EmailColorHeader).HasMaxLength(200);

                entity.Property(e => e.EmailDisplayName).HasMaxLength(200);

                entity.Property(e => e.EmailEnableSsl).HasColumnName("EmailEnableSSL");

                entity.Property(e => e.EmailHost).HasMaxLength(500);

                entity.Property(e => e.EmailLogo).HasMaxLength(500);

                entity.Property(e => e.EmailPasswordHash).HasMaxLength(500);

                entity.Property(e => e.EmailSender).HasMaxLength(500);

                entity.Property(e => e.EmailUserName).HasMaxLength(500);

                entity.Property(e => e.ExchangRate).HasColumnType("money");

                entity.Property(e => e.OpAccessCode).HasMaxLength(50);

                entity.Property(e => e.OpKey).HasMaxLength(100);

                entity.Property(e => e.OpMerchant).HasMaxLength(50);

                entity.Property(e => e.OpPassword).HasMaxLength(50);

                entity.Property(e => e.OpUser).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Website).HasMaxLength(500);
            });

            modelBuilder.Entity<ConfigTable>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<ConfigTableItem>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DataType).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.Note).HasColumnType("ntext");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<ConfigThumb>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(500);

                entity.Property(e => e.FullName).HasMaxLength(500);

                entity.Property(e => e.Note).HasColumnType("ntext");

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Flag).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.Address).HasColumnType("ntext");

                entity.Property(e => e.Avata).HasColumnType("ntext");

                entity.Property(e => e.Birthday).HasColumnType("datetime");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.FullName).HasMaxLength(500);

                entity.Property(e => e.KeyRandom).HasMaxLength(8);

                entity.Property(e => e.LastLoginAt).HasColumnType("datetime");

                entity.Property(e => e.Note).HasColumnType("ntext");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.Sex).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Username).HasMaxLength(500);
            });

            modelBuilder.Entity<CustomerAddress>(entity =>
            {
                entity.Property(e => e.CustomerAddressId).ValueGeneratedNever();

                entity.Property(e => e.Address).HasMaxLength(1000);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.Note).HasColumnType("ntext");

                entity.Property(e => e.Phone).HasMaxLength(200);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(10);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Function>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Icon).HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Note).HasMaxLength(2000);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Url).HasMaxLength(200);
            });

            modelBuilder.Entity<FunctionRole>(entity =>
            {
                entity.Property(e => e.ActiveKey).HasMaxLength(20);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Function)
                    .WithMany(p => p.FunctionRole)
                    .HasForeignKey(d => d.FunctionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FunctionRole_Function");
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Flag).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Manufacturer>(entity =>
            {
                entity.Property(e => e.Address).HasColumnType("ntext");

                entity.Property(e => e.AvatarOwner).HasMaxLength(500);

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.Contents).HasColumnType("ntext");

                entity.Property(e => e.Country).HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("ntext");

                entity.Property(e => e.Email).HasMaxLength(500);

                entity.Property(e => e.Fax).HasMaxLength(50);

                entity.Property(e => e.Logo).HasMaxLength(1000);

                entity.Property(e => e.MetaDescription).HasMaxLength(500);

                entity.Property(e => e.MetaKeywords).HasMaxLength(200);

                entity.Property(e => e.MetaTitle).HasMaxLength(500);

                entity.Property(e => e.Mobile).HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.NickName).HasMaxLength(500);

                entity.Property(e => e.Owner).HasMaxLength(500);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Url).HasMaxLength(500);

                entity.Property(e => e.Website).HasMaxLength(500);
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Note).HasColumnType("ntext");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<MenuItem>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<News>(entity =>
            {
                entity.Property(e => e.Author).HasMaxLength(1000);

                entity.Property(e => e.Contents).HasColumnType("ntext");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DateEndOn).HasColumnType("datetime");

                entity.Property(e => e.DateStartActive).HasColumnType("datetime");

                entity.Property(e => e.DateStartOn).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.Image).HasColumnType("ntext");

                entity.Property(e => e.LinkVideo).HasMaxLength(1000);

                entity.Property(e => e.MetaDescription).HasMaxLength(500);

                entity.Property(e => e.MetaKeyword).HasMaxLength(300);

                entity.Property(e => e.MetaTitle).HasMaxLength(500);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Url).HasMaxLength(1000);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.CustomerNote).HasColumnType("ntext");

                entity.Property(e => e.OrderDelivery).HasColumnType("money");

                entity.Property(e => e.OrderDiscount).HasColumnType("money");

                entity.Property(e => e.OrderPaid).HasColumnType("money");

                entity.Property(e => e.OrderTax).HasColumnType("money");

                entity.Property(e => e.OrderTotal).HasColumnType("money");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("('2015-09-20T16:17:38.112Z')");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.PriceDiscount).HasColumnType("money");

                entity.Property(e => e.PriceTax).HasColumnType("money");

                entity.Property(e => e.PriceTotal).HasColumnType("money");
            });

            modelBuilder.Entity<PaymentHistory>(entity =>
            {
                entity.Property(e => e.PaymentHistoryId).ValueGeneratedNever();

                entity.Property(e => e.AccessCode).HasMaxLength(10);

                entity.Property(e => e.AgainLink).HasMaxLength(500);

                entity.Property(e => e.Amount).HasMaxLength(50);

                entity.Property(e => e.Card).HasMaxLength(20);

                entity.Property(e => e.CardList).HasMaxLength(300);

                entity.Property(e => e.CardUid).HasMaxLength(100);

                entity.Property(e => e.Command).HasMaxLength(20);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Currency).HasMaxLength(5);

                entity.Property(e => e.CustomerEmail).HasMaxLength(200);

                entity.Property(e => e.CustomerId).HasMaxLength(50);

                entity.Property(e => e.CustomerPhone).HasMaxLength(20);

                entity.Property(e => e.Locale).HasMaxLength(5);

                entity.Property(e => e.MerchTxnRef).HasMaxLength(50);

                entity.Property(e => e.Merchant).HasMaxLength(50);

                entity.Property(e => e.Message).HasMaxLength(500);

                entity.Property(e => e.OrderInfo).HasMaxLength(50);

                entity.Property(e => e.PayChannel).HasMaxLength(50);

                entity.Property(e => e.ReturnUrl)
                    .HasColumnName("ReturnURL")
                    .HasMaxLength(500);

                entity.Property(e => e.SecureHash).HasMaxLength(100);

                entity.Property(e => e.TicketNo).HasMaxLength(20);

                entity.Property(e => e.TransactionNo).HasMaxLength(100);

                entity.Property(e => e.TxnResponseCode).HasMaxLength(10);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Position>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.Contents).HasColumnType("ntext");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DateEndOn).HasColumnType("datetime");

                entity.Property(e => e.DateStartActive).HasColumnType("datetime");

                entity.Property(e => e.DateStartOn).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("ntext");

                entity.Property(e => e.Image).HasColumnType("ntext");

                entity.Property(e => e.ImageLeft).HasMaxLength(500);

                entity.Property(e => e.ImageRight).HasMaxLength(500);

                entity.Property(e => e.LinkYoutube)
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.MetaDescription).HasMaxLength(500);

                entity.Property(e => e.MetaKeyword).HasMaxLength(300);

                entity.Property(e => e.MetaTitle).HasMaxLength(200);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.NotePromotion).HasColumnType("ntext");

                entity.Property(e => e.NoteTech).HasColumnType("ntext");

                entity.Property(e => e.PriceImport).HasColumnType("money");

                entity.Property(e => e.PriceOther).HasColumnType("money");

                entity.Property(e => e.PriceSale).HasColumnType("money");

                entity.Property(e => e.PriceSpecial).HasColumnType("money");

                entity.Property(e => e.ProductAttributes).HasColumnType("ntext");

                entity.Property(e => e.ProductNote).HasColumnType("ntext");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Url).HasMaxLength(1000);
            });

            modelBuilder.Entity<ProductAttribuite>(entity =>
            {
                entity.HasKey(e => e.ProductAttributesId);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Value).HasMaxLength(500);
            });

            modelBuilder.Entity<ProductCustomer>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Image).HasColumnType("ntext");

                entity.Property(e => e.Name).HasMaxLength(500);
            });

            modelBuilder.Entity<ProductReview>(entity =>
            {
                entity.Property(e => e.Contents).HasColumnType("ntext");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Province>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(10);

                entity.Property(e => e.Lang).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Related>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Slide>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.Image).HasColumnType("ntext");

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Url).HasMaxLength(1000);
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(1000);

                entity.Property(e => e.Url).HasMaxLength(1000);
            });

            modelBuilder.Entity<TypeAttribute>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<TypeAttributeItem>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(100);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FullName).HasMaxLength(100);

                entity.Property(e => e.KeyLock).HasMaxLength(20);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RegEmail).HasMaxLength(50);

                entity.Property(e => e.TokenSince).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<UserMapping>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRole)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRole_User");
            });

            modelBuilder.Entity<Wards>(entity =>
            {
                entity.HasKey(e => e.WardId);

                entity.Property(e => e.Code).HasMaxLength(20);

                entity.Property(e => e.Name).HasMaxLength(500);
            });

            modelBuilder.Entity<Website>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(1000);

                entity.Property(e => e.Banner).HasMaxLength(1000);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.GoogleAnalitics).HasMaxLength(50);

                entity.Property(e => e.GuaRanTeePhone).HasMaxLength(50);

                entity.Property(e => e.Hotline).HasMaxLength(50);

                entity.Property(e => e.Hotmail).HasMaxLength(500);

                entity.Property(e => e.LinkFacebookPage).HasMaxLength(1000);

                entity.Property(e => e.LinkGooglePlus).HasMaxLength(1000);

                entity.Property(e => e.LinkInstagram).HasMaxLength(1000);

                entity.Property(e => e.LinkLinkedIn).HasMaxLength(1000);

                entity.Property(e => e.LinkOther1).HasMaxLength(1000);

                entity.Property(e => e.LinkOther2).HasMaxLength(1000);

                entity.Property(e => e.LinkOther3).HasMaxLength(1000);

                entity.Property(e => e.LinkTwitter).HasMaxLength(1000);

                entity.Property(e => e.LinkYoutube).HasMaxLength(1000);

                entity.Property(e => e.LogoFooter).HasMaxLength(1000);

                entity.Property(e => e.LogoHeader).HasMaxLength(1000);

                entity.Property(e => e.MetaDescription).HasMaxLength(500);

                entity.Property(e => e.MetaKeyword).HasMaxLength(300);

                entity.Property(e => e.MetaTitle).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.TechNiQuePhone).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Url).HasMaxLength(300);
            });
        }
    }
}
