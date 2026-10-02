using System.ComponentModel.DataAnnotations;

namespace CariSiparisYonetimi.ViewModels
{
    public class SiparisSatiriViewModel
    {
        [Required(ErrorMessage = "Ürün veya hizmet adı zorunludur.")]
        [StringLength(150)]
        [Display(Name = "Ürün / Hizmet Adı")]
        public string UrunHizmetAdi { get; set; } = string.Empty;

        [Range(
            typeof(decimal),
            "0.001",
            "999999999999999.999",
            ParseLimitsInInvariantCulture = true,
            ErrorMessage = "Miktar en az 0,001 olmalı ve izin verilen sınırı aşmamalıdır.")]
        [Display(Name = "Miktar")]
        public decimal Miktar { get; set; } = 1;

        [Range(
            typeof(decimal),
            "0",
            "9999999999999999.99",
            ParseLimitsInInvariantCulture = true,
            ErrorMessage = "Birim fiyat negatif olamaz ve izin verilen sınırı aşamaz.")]
        [Display(Name = "Birim Fiyat")]
        public decimal BirimFiyat { get; set; }
    }
}