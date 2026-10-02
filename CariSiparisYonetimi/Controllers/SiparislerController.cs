using CariSiparisYonetimi.Services.Interfaces;
using CariSiparisYonetimi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CariSiparisYonetimi.Controllers
{
    public class SiparislerController : Controller
    {
        private readonly ISiparisService _siparisService;
        private readonly ICariService _cariService;

        // DI üzerinden gelen sipariş ve cari servislerini controller içinde kullanmak için saklar.
        public SiparislerController(
            ISiparisService siparisService,
            ICariService cariService)
        {
            _siparisService = siparisService;
            _cariService = cariService;
        }

        // Siparişleri servisten getirerek listeleme ekranına gönderir.
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var siparisler = await _siparisService.ListeleAsync();

            return View(siparisler);
        }

        // Siparişi cari bilgileri ve satırlarıyla detay ekranına gönderir; bulunamazsa 404 döndürür.
        [HttpGet]
        public async Task<IActionResult> Detay(int id)
        {
            var siparis = await _siparisService.IdIleGetirAsync(id);

            if (siparis == null)
            {
                return NotFound();
            }

            return View(siparis);
        }

        // Aktif cari seçeneklerini hazırlayarak yeni sipariş formunu açar.
        [HttpGet]
        public async Task<IActionResult> Ekle()
        {
            var model = new SiparisEkleViewModel();

            await CariSecenekleriniDoldurAsync(model);

            return View(model);
        }

        // Formu doğrular, siparişi servise kaydettirir ve başarılı işlemde detay ekranına yönlendirir.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(
            [Bind("SiparisNo,SiparisTarihi,CariId,Aciklama,Detaylar")]
            SiparisEkleViewModel model)
        {

            if (model.SiparisTarihi.Date > DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(model.SiparisTarihi),
                    "Sipariş tarihi bugünden ileri olamaz.");
            }

            if (!ModelState.IsValid)
            {
                await CariSecenekleriniDoldurAsync(model);

                return View(model);
            }

            model.SiparisNo = model.SiparisNo.Trim();

            if (await _siparisService.SiparisNoKullaniliyorMuAsync(
                model.SiparisNo))
            {
                ModelState.AddModelError(
                    nameof(SiparisEkleViewModel.SiparisNo),
                    "Bu sipariş numarası zaten kullanılıyor.");

                await CariSecenekleriniDoldurAsync(model);

                return View(model);
            }

            try
            {
                var siparisId = await _siparisService.EkleAsync(model);

                TempData["Basari"] = "Sipariş başarıyla oluşturuldu.";

                return RedirectToAction(
                    nameof(Detay),
                    new { id = siparisId });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Sipariş kaydedilemedi. Cari bilgilerini ve sipariş numarasını kontrol edip tekrar deneyiniz.");
            }

            await CariSecenekleriniDoldurAsync(model);

            return View(model);
        }

        // Aktif carileri sipariş formundaki seçim kutusu için seçeneklere dönüştürür.
        private async Task CariSecenekleriniDoldurAsync(
            SiparisEkleViewModel model)
        {
            var cariler = await _cariService.ListeleAsync();

            model.Cariler = cariler
                .Where(c => c.AktifMi)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.CariKodu} - {c.CariAdi}"
                })
                .ToList();
        }

        // Siparişi servis üzerinden iptal eder ve listeye yönlendirir.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IptalEt(int id)
        {
            if (id <= 0)
                return BadRequest();

            var sonuc = await _siparisService.IptalEtAsync(id);

            if (!sonuc)
                return NotFound();

            TempData["Basari"] = "Sipariş iptal edildi.";

            return RedirectToAction(nameof(Index));
        }
    }
}