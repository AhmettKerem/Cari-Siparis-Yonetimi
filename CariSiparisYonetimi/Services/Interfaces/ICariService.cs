using CariSiparisYonetimi.Models;

namespace CariSiparisYonetimi.Services.Interfaces
{
    public interface ICariService
    {
        Task<List<Cari>> ListeleAsync();

        Task<Cari?> IdIleGetirAsync(int id);

        Task<bool> CariKoduKullaniliyorMuAsync(
            string cariKodu,
            int? haricTutulanId = null);

        Task EkleAsync(Cari cari);

        Task<bool> GuncelleAsync(Cari cari);

        Task<bool> PasifeAlAsync(int id);

        Task<bool> AktifeAlAsync(int id);

        Task<bool> LogoYolunuGuncelleAsync(
            int cariId,
            string logoYolu);

        Task<(bool CariBulundu, string? EskiLogoYolu)>
            LogoyuKaldirAsync(int cariId);
    }
}