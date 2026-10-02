using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CariSiparisYonetimi.Models
{
    public class SiparisDetayi
    {
        public int Id { get; set; }

        public int SiparisId { get; set; }

        public Siparis Siparis { get; set; } = null!;

        [Required(ErrorMessage = "Ürün veya hizmet adı zorunludur.")]
        [StringLength(150)]
        public string UrunHizmetAdi { get; set; } = string.Empty;

        public decimal Miktar { get; set; }

        public decimal BirimFiyat { get; set; }

        [NotMapped]
        public decimal Tutar => Miktar * BirimFiyat;
    }
}