using CariSiparisYonetimi.Models;
using CariSiparisYonetimi.ViewModels;

namespace CariSiparisYonetimi.Services.Interfaces
{
    public interface ISiparisService
    {
        // Siparişleri ilişkili cari bilgileri ve satırlarıyla birlikte getirir.
        Task<List<Siparis>> ListeleAsync();

        // Siparişi cari bilgileri ve satırlarıyla getirir; bulunamazsa null döndürür.
        Task<Siparis?> IdIleGetirAsync(int id);

        // Verilen sipariş numarasının başka bir kayıtta kullanılıp kullanılmadığını kontrol eder.
        Task<bool> SiparisNoKullaniliyorMuAsync(string siparisNo);

        // Form verilerini kontrol ederek siparişi ve satırlarını kaydeder, oluşan Id'yi döndürür.
        Task<int> EkleAsync(SiparisEkleViewModel model);

        // Siparişi silmeden iptal eder; kayıt bulunamazsa false döndürür.
        Task<bool> IptalEtAsync(int id);
    }
}