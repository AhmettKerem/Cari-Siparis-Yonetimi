using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CariSiparisYonetimi.ViewModels
{
    public class SiparisEkleViewModel
    {
        [Required(ErrorMessage = "Sipariş numarası zorunludur.")]
        [StringLength(30)]
        [Display(Name = "Sipariş No")]
        public string SiparisNo { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Sipariş Tarihi")]
        public DateTime SiparisTarihi { get; set; } = DateTime.Today;

        [Range(1, int.MaxValue, ErrorMessage = "Bir cari seçiniz.")]
        [Display(Name = "Cari")]
        public int CariId { get; set; }

        [StringLength(500)]
        [Display(Name = "Açıklama")]
        public string? Aciklama { get; set; }

        [Required(ErrorMessage = "En az bir sipariş satırı ekleyiniz.")]
        [MinLength(1, ErrorMessage = "En az bir sipariş satırı ekleyiniz.")]
        public List<SiparisSatiriViewModel> Detaylar { get; set; }
            = new List<SiparisSatiriViewModel>
            {
                new SiparisSatiriViewModel()
            };

        [ValidateNever]
        public List<SelectListItem> Cariler { get; set; }
            = new List<SelectListItem>();
    }
}