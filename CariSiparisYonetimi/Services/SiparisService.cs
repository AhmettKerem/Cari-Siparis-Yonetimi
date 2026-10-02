using CariSiparisYonetimi.Data;
using CariSiparisYonetimi.Models;
using CariSiparisYonetimi.Services.Interfaces;
using CariSiparisYonetimi.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CariSiparisYonetimi.Services
{
    public class SiparisService : ISiparisService
    {
        private readonly AppDbContext _context;

        // DI üzerinden gelen veritabanı context'ini servis içinde kullanmak için saklar.
        public SiparisService(AppDbContext context)
        {
            _context = context;
        }

        // Siparişleri cari bilgileri ve satırlarıyla, en yeni tarih önce olacak şekilde getirir.
        public async Task<List<Siparis>> ListeleAsync()
        {
            return await _context.Siparisler
                .AsNoTracking()
                .Include(s => s.Cari)
                .Include(s => s.Detaylar)
                .OrderByDescending(s => s.SiparisTarihi)
                .ThenByDescending(s => s.Id)
                .ToListAsync();
        }

        // Id'ye ait siparişi cari bilgileri ve satırlarıyla getirir; bulunamazsa null döndürür.
        public async Task<Siparis?> IdIleGetirAsync(int id)
        {
            return await _context.Siparisler
                .AsNoTracking()
                .Include(s => s.Cari)
                .Include(s => s.Detaylar)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        // Başındaki ve sonundaki boşlukları temizlenen sipariş numarasının kullanımını kontrol eder.
        public async Task<bool> SiparisNoKullaniliyorMuAsync(
            string siparisNo)
        {
            var temizSiparisNo = siparisNo.Trim();

            return await _context.Siparisler
                .AnyAsync(s => s.SiparisNo == temizSiparisNo);
        }

        // Formu ve iş kurallarını doğrular, siparişi satırlarıyla birlikte kaydeder ve Id'sini döndürür.
        public async Task<int> EkleAsync(SiparisEkleViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.SiparisNo))
            {
                throw new ArgumentException(
                    "Sipariş numarası zorunludur.");
            }

            var siparisNo = model.SiparisNo.Trim();

            if (siparisNo.Length > 30)
            {
                throw new ArgumentException(
                    "Sipariş numarası en fazla 30 karakter olabilir.");
            }

            if (model.SiparisTarihi == default)
            {
                throw new ArgumentException(
                    "Geçerli bir sipariş tarihi giriniz.");
            }

            // Bugün ve geçmiş tarihler kabul edilir; gelecek tarihler engellenir.
            if (model.SiparisTarihi.Date > DateTime.Today)
            {
                throw new ArgumentException(
                    "Sipariş tarihi bugünden ileri olamaz.");
            }

            if (model.Aciklama?.Length > 500)
            {
                throw new ArgumentException(
                    "Açıklama en fazla 500 karakter olabilir.");
            }

            if (model.Detaylar == null || model.Detaylar.Count == 0)
            {
                throw new ArgumentException(
                    "En az bir sipariş satırı ekleyiniz.");
            }

            var aktifCariVar = await _context.Cariler
                .AnyAsync(c => c.Id == model.CariId && c.AktifMi);

            if (!aktifCariVar)
            {
                throw new ArgumentException(
                    "Sipariş için mevcut ve aktif bir cari seçiniz.");
            }

            if (await SiparisNoKullaniliyorMuAsync(siparisNo))
            {
                throw new ArgumentException(
                    "Bu sipariş numarası zaten kullanılıyor.");
            }

            var siparis = new Siparis
            {
                SiparisNo = siparisNo,
                SiparisTarihi = model.SiparisTarihi.Date,
                CariId = model.CariId,
                Aciklama = model.Aciklama?.Trim()
            };

            foreach (var satir in model.Detaylar)
            {
                if (satir == null ||
                    string.IsNullOrWhiteSpace(satir.UrunHizmetAdi))
                {
                    throw new ArgumentException(
                        "Her satır için ürün veya hizmet adı giriniz.");
                }

                var urunHizmetAdi = satir.UrunHizmetAdi.Trim();

                if (urunHizmetAdi.Length > 150)
                {
                    throw new ArgumentException(
                        "Ürün veya hizmet adı en fazla 150 karakter olabilir.");
                }

                if (satir.Miktar <= 0 ||
                    satir.Miktar > 999999999999999.999m)
                {
                    throw new ArgumentException(
                        "Miktar sıfırdan büyük ve izin verilen sınırlar içinde olmalıdır.");
                }

                if (satir.BirimFiyat < 0 ||
                    satir.BirimFiyat > 9999999999999999.99m)
                {
                    throw new ArgumentException(
                        "Birim fiyat negatif olamaz ve izin verilen sınırı aşamaz.");
                }

                if (decimal.Round(satir.Miktar, 3) != satir.Miktar)
                {
                    throw new ArgumentException(
                        "Miktar en fazla 3 ondalık basamak içerebilir.");
                }

                if (decimal.Round(satir.BirimFiyat, 2) != satir.BirimFiyat)
                {
                    throw new ArgumentException(
                        "Birim fiyat en fazla 2 ondalık basamak içerebilir.");
                }

                siparis.Detaylar.Add(new SiparisDetayi
                {
                    UrunHizmetAdi = urunHizmetAdi,
                    Miktar = satir.Miktar,
                    BirimFiyat = satir.BirimFiyat
                });
            }

            _context.Siparisler.Add(siparis);

            await _context.SaveChangesAsync();

            return siparis.Id;
        }

        // Siparişin iptal durumunu günceller ve veritabanına kaydeder.
        public async Task<bool> IptalEtAsync(int id)
        {
            var siparis = await _context.Siparisler
                .FirstOrDefaultAsync(s => s.Id == id);

            if (siparis is null)
                return false;

            if (siparis.IptalMi)
                return true;

            siparis.IptalMi = true;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}