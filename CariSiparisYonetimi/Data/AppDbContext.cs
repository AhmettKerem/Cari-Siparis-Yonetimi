using CariSiparisYonetimi.Models;
using Microsoft.EntityFrameworkCore;
using CariSiparisYonetimi.Data.Configurations;

namespace CariSiparisYonetimi.Data
{
    public class AppDbContext : DbContext
    {
        // DI üzerinden gelen veritabanı ayarlarını üst sınıf DbContext'e iletir.
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cari> Cariler { get; set; }

        public DbSet<Siparis> Siparisler { get; set; }

        public DbSet<SiparisDetayi> SiparisDetaylari { get; set; }

        // Entity yapılandırmalarını EF Core modeline uygulayarak eşleme ve ilişki kurallarını tanımlar.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CariConfiguration());

            modelBuilder.ApplyConfiguration(new SiparisConfiguration());

            modelBuilder.ApplyConfiguration(new SiparisDetayiConfiguration());
        }
    }
}