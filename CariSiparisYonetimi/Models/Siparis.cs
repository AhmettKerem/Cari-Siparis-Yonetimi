using System.ComponentModel.DataAnnotations;

namespace CariSiparisYonetimi.Models
{
    public class Siparis
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Sipariş numarası zorunludur.")]
        [StringLength(30)]
        public string SiparisNo { get; set; } = string.Empty;

        public DateTime SiparisTarihi { get; set; } = DateTime.Today;

        [Range(1, int.MaxValue, ErrorMessage = "Bir cari seçiniz.")]
        public int CariId { get; set; }

        public Cari Cari { get; set; } = null!;

        [StringLength(500)]
        public string? Aciklama { get; set; }

        // Siparişin iptal edilip edilmediğini tutar; sipariş kaydı silinmez.
        public bool IptalMi { get; set; } = false;

        public List<SiparisDetayi> Detaylar { get; set; } = new List<SiparisDetayi>();
    }
}