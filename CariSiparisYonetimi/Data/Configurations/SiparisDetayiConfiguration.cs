using CariSiparisYonetimi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CariSiparisYonetimi.Data.Configurations
{
    public class SiparisDetayiConfiguration
        : IEntityTypeConfiguration<SiparisDetayi>
    {
        // Sipariş silindiğinde detaylarının da silinmesini ve miktar/fiyat sütunlarının ondalık hassasiyetlerini tanımlar.
        public void Configure(EntityTypeBuilder<SiparisDetayi> builder)
        {
            builder.HasOne(d => d.Siparis)
                .WithMany(s => s.Detaylar)
                .HasForeignKey(d => d.SiparisId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(d => d.Miktar)
                .HasPrecision(18, 3);

            builder.Property(d => d.BirimFiyat)
                .HasPrecision(18, 2);
        }
    }
}