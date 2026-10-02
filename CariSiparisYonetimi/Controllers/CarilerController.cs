using CariSiparisYonetimi.Models;
using CariSiparisYonetimi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CariSiparisYonetimi.Services.Interfaces;

namespace CariSiparisYonetimi.Controllers
{
    public class CarilerController : Controller
    {
        private readonly ICariService _cariService;
        private readonly ICariLogoService _cariLogoService;

        // DI üzerinden gelen cari ve cari logo servislerini controller içinde kullanmak için saklar.
        public CarilerController(
            ICariService cariService,
            ICariLogoService cariLogoService)
        {
            _cariService = cariService;
            _cariLogoService = cariLogoService;
        }

        // Tüm carileri servisten getirerek listeleme ekranına gönderir.
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cariler = await _cariService.ListeleAsync();

            return View(cariler);
        }

        // Yeni cari oluşturmak için boş cari nesnesiyle ekleme ekranını açar.
        [HttpGet]
        public IActionResult Ekle()
        {
            return View(new Cari());
        }

        // Form bilgilerini doğrular, cari kodunun kullanımını kontrol eder ve yeni cariyi kaydeder.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(
            [Bind("CariKodu,CariAdi,VergiNumarasi,Telefon,Eposta,Adres")]
            Cari cari)
        {
            if (!ModelState.IsValid)
            {
                return View(cari);
            }

            cari.CariKodu = cari.CariKodu.Trim();
            cari.CariAdi = cari.CariAdi.Trim();

            if (await _cariService.CariKoduKullaniliyorMuAsync(
                cari.CariKodu))
            {
                ModelState.AddModelError(
                    nameof(Cari.CariKodu),
                    "Bu cari kodu zaten kullanılıyor.");

                return View(cari);
            }

            try
            {
                await _cariService.EkleAsync(cari);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Cari kaydedilemedi. Cari kodunu kontrol edip tekrar deneyiniz.");

                return View(cari);
            }

            return RedirectToAction(nameof(Index));
        }

        // Id'ye ait cariyi düzenleme ekranına gönderir; kayıt bulunamazsa 404 döndürür.
        [HttpGet]
        public async Task<IActionResult> Duzenle(int id)
        {
            var cari = await _cariService.IdIleGetirAsync(id);

            if (cari == null)
            {
                return NotFound();
            }

            return View(cari);
        }

        // Id ve form bilgilerini doğrular, cari kodu çakışmasını kontrol eder ve cariyi günceller.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duzenle(
            int id,
            [Bind("Id,CariKodu,CariAdi,VergiNumarasi,Telefon,Eposta,Adres")]
            Cari cari)
        {
            if (id != cari.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(cari);
            }

            cari.CariKodu = cari.CariKodu.Trim();
            cari.CariAdi = cari.CariAdi.Trim();

            if (await _cariService.CariKoduKullaniliyorMuAsync(
                cari.CariKodu,
                cari.Id))
            {
                ModelState.AddModelError(
                    nameof(Cari.CariKodu),
                    "Bu cari kodu başka bir cari tarafından kullanılıyor.");

                return View(cari);
            }

            try
            {
                var guncellendi =
                    await _cariService.GuncelleAsync(cari);

                if (!guncellendi)
                {
                    return NotFound();
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Cari güncellenemedi. Bilgileri kontrol edip tekrar deneyiniz.");

                return View(cari);
            }

            return RedirectToAction(nameof(Index));
        }

        // Cariyi silmeden pasife alır ve işlem sonucunu listeleme ekranında bildirir.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PasifeAl(int id)
        {
            try
            {
                var pasifeAlindi =
                    await _cariService.PasifeAlAsync(id);

                if (!pasifeAlindi)
                {
                    return NotFound();
                }

                TempData["Basari"] = "Cari pasife alındı.";
            }
            catch (DbUpdateException)
            {
                TempData["Hata"] =
                    "Cari pasife alınamadı. Tekrar deneyiniz.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Cariyi aktif duruma getirir ve işlem sonucunu listeleme ekranında bildirir.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AktifeAl(int id)
        {
            try
            {
                var aktifeAlindi =
                    await _cariService.AktifeAlAsync(id);

                if (!aktifeAlindi)
                {
                    return NotFound();
                }

                TempData["Basari"] = "Cari aktife alındı.";
            }
            catch (DbUpdateException)
            {
                TempData["Hata"] =
                    "Cari aktife alınamadı. Tekrar deneyiniz.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Cari bilgileri ve mevcut logo yoluyla logo yükleme ekranını açar.
        [HttpGet]
        public async Task<IActionResult> LogoYukle(int id)
        {
            var cari = await _cariService.IdIleGetirAsync(id);

            if (cari == null)
            {
                return NotFound();
            }

            var model = new LogoYukleViewModel
            {
                CariId = cari.Id,
                CariAdi = cari.CariAdi,
                MevcutLogoYolu = cari.LogoYolu
            };

            return View(model);
        }

        // Id ve form bilgilerini doğrular, logo yükleme işlemini servise iletir ve sonucu kullanıcıya bildirir.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoYukle(
            int id,
            [Bind("CariId,LogoDosyasi")] LogoYukleViewModel model)
        {
            if (id != model.CariId)
            {
                return BadRequest();
            }

            var cari = await _cariService.IdIleGetirAsync(id);

            if (cari == null)
            {
                return NotFound();
            }

            model.CariAdi = cari.CariAdi;
            model.MevcutLogoYolu = cari.LogoYolu;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var sonuc = await _cariLogoService.YukleAsync(
                    id,
                    model.LogoDosyasi!);

                if (!sonuc.CariBulundu)
                {
                    return NotFound();
                }

                TempData["Basari"] =
                    "Logo başarıyla kaydedildi.";

                if (!sonuc.DosyaTemizlendi)
                {
                    TempData["Hata"] =
                        "Yeni logo kaydedildi ancak önceki logo dosyası temizlenemedi.";
                }

                return RedirectToAction(
                    nameof(LogoYukle),
                    new { id });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(
                    nameof(LogoYukleViewModel.LogoDosyasi),
                    ex.Message);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Logo bilgisi veritabanına kaydedilemedi. Tekrar deneyiniz.");
            }
            catch (IOException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Logo dosyası kaydedilemedi. Tekrar deneyiniz.");
            }
            catch (UnauthorizedAccessException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Logo klasörüne dosya yazma izni bulunmuyor.");
            }

            return View(model);
        }

        // Cari logosunu kaldırma işlemini servise iletir ve logo ile dosya temizliği sonucunu bildirir.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoSil(int id)
        {
            try
            {
                var sonuc =
                    await _cariLogoService.SilAsync(id);

                if (!sonuc.CariBulundu)
                {
                    return NotFound();
                }

                if (!sonuc.LogoVardi)
                {
                    TempData["Hata"] =
                        "Bu cariye ait bir logo bulunmuyor.";
                }
                else if (!sonuc.DosyaTemizlendi)
                {
                    TempData["Hata"] =
                        "Logo cariden kaldırıldı ancak dosya diskten silinemedi.";
                }
                else
                {
                    TempData["Basari"] =
                        "Logo başarıyla silindi.";
                }
            }
            catch (DbUpdateException)
            {
                TempData["Hata"] =
                    "Logo kaldırılamadı. Lütfen tekrar deneyiniz.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}