using CariSiparisYonetimi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CariSiparisYonetimi.Data.Configurations
{
    public class CariConfiguration : IEntityTypeConfiguration<Cari>
    {
        // Aynı cari koduyla birden fazla kayıt oluşmasını önlemek için benzersiz index tanımlar.
        public void Configure(EntityTypeBuilder<Cari> builder)
        {
            builder.HasIndex(c => c.CariKodu)
                .IsUnique();
        }
    }
}