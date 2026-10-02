using Microsoft.AspNetCore.Http;
using CariSiparisYonetimi.Services.Interfaces;

namespace CariSiparisYonetimi.Services
{
    public class CariLogoService : ICariLogoService
    {
        private readonly ICariService _cariService;
        private readonly ILogoService _logoService;
        private readonly ILogger<CariLogoService> _logger;

        // DI üzerinden gelen cari servisini, dosya servisini ve logger'ı kullanmak için saklar.
        public CariLogoService(
            ICariService cariService,
            ILogoService logoService,
            ILogger<CariLogoService> logger)
        {
            _cariService = cariService;
            _logoService = logoService;
            _logger = logger;
        }

        // Yeni logoyu kaydedip cariye bağlar; başarıda eski dosyayı, kayıt başarısızlığında yeni dosyayı temizlemeye çalışır.
        public async Task<(bool CariBulundu, bool DosyaTemizlendi)>
            YukleAsync(int cariId, IFormFile dosya)
        {
            var cari = await _cariService.IdIleGetirAsync(cariId);

            if (cari == null)
            {
                return (false, true);
            }

            string? yeniLogoYolu = null;
            var veritabaninaKaydedildi = false;

            try
            {
                yeniLogoYolu = await _logoService.KaydetAsync(dosya);

                veritabaninaKaydedildi =
                    await _cariService.LogoYolunuGuncelleAsync(
                        cariId,
                        yeniLogoYolu);

                if (!veritabaninaKaydedildi)
                {
                    return (false, true);
                }

                var eskiDosyaTemizlendi = true;

                if (!string.IsNullOrWhiteSpace(cari.LogoYolu)
                    && cari.LogoYolu != yeniLogoYolu)
                {
                    eskiDosyaTemizlendi =
                        _logoService.Sil(cari.LogoYolu);
                }

                return (true, eskiDosyaTemizlendi);
            }
            finally
            {
                if (!veritabaninaKaydedildi
                    && yeniLogoYolu != null)
                {
                    var dosyaTemizlendi =
                        _logoService.Sil(yeniLogoYolu);

                    if (!dosyaTemizlendi)
                    {
                        _logger.LogWarning(
                            "Kaydedilemeyen logo dosyası temizlenemedi. " +
                            "CariId: {CariId}, LogoYolu: {LogoYolu}",
                            cariId,
                            yeniLogoYolu);
                    }
                }
            }
        }

        // Logo bağlantısını veritabanından kaldırıp dosyayı siler; cari, logo ve dosya temizliği durumlarını döndürür.
        public async Task<(bool CariBulundu, bool LogoVardi, bool DosyaTemizlendi)>
            SilAsync(int cariId)
        {
            var sonuc =
                await _cariService.LogoyuKaldirAsync(cariId);

            if (!sonuc.CariBulundu)
            {
                return (false, false, true);
            }

            if (string.IsNullOrWhiteSpace(sonuc.EskiLogoYolu))
            {
                return (true, false, true);
            }

            var dosyaTemizlendi =
                _logoService.Sil(sonuc.EskiLogoYolu);

            return (true, true, dosyaTemizlendi);
        }
    }
}