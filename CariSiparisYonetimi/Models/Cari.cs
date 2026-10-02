using System.ComponentModel.DataAnnotations;

namespace CariSiparisYonetimi.Models
{
    public class Cari
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Cari kodu zorunludur.")]
        [StringLength(30)]
        public string CariKodu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cari adı zorunludur.")]
        [StringLength(150)]
        public string CariAdi { get; set; } = string.Empty;

        public string? VergiNumarasi { get; set; }

        public string? Telefon { get; set; }

        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string? Eposta { get; set; }

        public string? Adres { get; set; }

        public string? LogoYolu { get; set; }

        public bool AktifMi { get; set; } = true;
    }
}