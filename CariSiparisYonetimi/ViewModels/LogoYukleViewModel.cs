using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CariSiparisYonetimi.ViewModels
{
    public class LogoYukleViewModel
    {
        public int CariId { get; set; }

        public string? CariAdi { get; set; }

        public string? MevcutLogoYolu { get; set; }

        [Required(ErrorMessage = "Bir logo dosyası seçiniz.")]
        public IFormFile? LogoDosyasi { get; set; }
    }
}