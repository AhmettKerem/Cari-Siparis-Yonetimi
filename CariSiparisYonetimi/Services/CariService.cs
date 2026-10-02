using CariSiparisYonetimi.Data;
using CariSiparisYonetimi.Models;
using Microsoft.EntityFrameworkCore;
using CariSiparisYonetimi.Services.Interfaces;

namespace CariSiparisYonetimi.Services
{
    public class CariService : ICariService
    {
        private readonly AppDbContext _context;

        // DI üzerinden gelen veritabanı context'ini servis içinde kullanmak için saklar.
        public CariService(AppDbContext context)
        {
            _context = context;
        }

        // Aktif ve pasif tüm carileri takip etmeden, cari koduna göre sıralı getirir.
        public async Task<List<Cari>> ListeleAsync()
        {
            return await _context.Cariler
                .AsNoTracking()
                .OrderBy(c => c.CariKodu)
                .ToListAsync();
        }

        // Verilen kodun kullanımını kontrol eder; düzenlenen kaydı kendi Id'siyle hariç tutar.
        public async Task<bool> CariKoduKullaniliyorMuAsync(
            string cariKodu,
            int? haricTutulanId = null)
        {
            return await _context.Cariler.AnyAsync(
                c => c.CariKodu == cariKodu &&
                (!haricTutulanId.HasValue || c.Id != haricTutulanId.Value));
        }

        // Verilen cari nesnesini yeni kayıt olarak veritabanına ekler.
        public async Task EkleAsync(Cari cari)
        {
            _context.Cariler.Add(cari);

            await _context.SaveChangesAsync();
        }

        // Id'ye ait cariyi takip etmeden getirir; kayıt bulunamazsa null döndürür.
        public async Task<Cari?> IdIleGetirAsync(int id)
        {
            return await _context.Cariler
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // Cari bilgilerini günceller, logo ve aktiflik durumunu korur; kayıt yoksa false döndürür.
        public async Task<bool> GuncelleAsync(Cari cari)
        {
            var mevcutCari = await _context.Cariler
                .FirstOrDefaultAsync(c => c.Id == cari.Id);

            if (mevcutCari == null)
            {
                return false;
            }

            mevcutCari.CariKodu = cari.CariKodu.Trim();
            mevcutCari.CariAdi = cari.CariAdi.Trim();
            mevcutCari.VergiNumarasi = cari.VergiNumarasi;
            mevcutCari.Telefon = cari.Telefon;
            mevcutCari.Eposta = cari.Eposta;
            mevcutCari.Adres = cari.Adres;

            await _context.SaveChangesAsync();

            return true;
        }

        // Cariyi silmeden pasife alır ve geçmiş ilişkilerini korur; kayıt yoksa false döndürür.
        public async Task<bool> PasifeAlAsync(int id)
        {
            var cari = await _context.Cariler
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cari == null)
            {
                return false;
            }

            cari.AktifMi = false;

            await _context.SaveChangesAsync();

            return true;
        }

        // Cariyi aktif duruma getirir; kayıt bulunamazsa false döndürür.
        public async Task<bool> AktifeAlAsync(int id)
        {
            var cari = await _context.Cariler
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cari == null)
            {
                return false;
            }

            cari.AktifMi = true;

            await _context.SaveChangesAsync();

            return true;
        }

        // Carinin logo yolunu veritabanında günceller; fiziksel dosya işlemi yapmaz.
        public async Task<bool> LogoYolunuGuncelleAsync(
            int cariId,
            string logoYolu)
        {
            var cari = await _context.Cariler
                .FirstOrDefaultAsync(c => c.Id == cariId);

            if (cari == null)
            {
                return false;
            }

            cari.LogoYolu = logoYolu;

            await _context.SaveChangesAsync();

            return true;
        }

        // Logo bağlantısını veritabanından kaldırır, cari bulunma bilgisini ve eski yolu döndürür.
        public async Task<(bool CariBulundu, string? EskiLogoYolu)>
            LogoyuKaldirAsync(int cariId)
        {
            var cari = await _context.Cariler
                .FirstOrDefaultAsync(c => c.Id == cariId);

            if (cari == null)
            {
                return (false, null);
            }

            var eskiLogoYolu = cari.LogoYolu;

            cari.LogoYolu = null;

            await _context.SaveChangesAsync();

            return (true, eskiLogoYolu);
        }
    }
}