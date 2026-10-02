using CariSiparisYonetimi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CariSiparisYonetimi.Data.Configurations
{
    public class SiparisConfiguration : IEntityTypeConfiguration<Siparis>
    {
        // Sipariş numarasını benzersiz yapar ve cari ilişkisini tanımlayarak siparişi bulunan carinin silinmesini engeller.
        public void Configure(EntityTypeBuilder<Siparis> builder)
        {
            builder.HasIndex(s => s.SiparisNo)
                .IsUnique();

            builder.HasOne(s => s.Cari)
                .WithMany()
                .HasForeignKey(s => s.CariId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}