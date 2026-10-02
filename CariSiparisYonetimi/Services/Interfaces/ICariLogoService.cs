using Microsoft.AspNetCore.Http;

namespace CariSiparisYonetimi.Services.Interfaces
{
    public interface ICariLogoService
    {
        Task<(bool CariBulundu, bool DosyaTemizlendi)>
            YukleAsync(int cariId, IFormFile dosya);

        Task<(bool CariBulundu, bool LogoVardi, bool DosyaTemizlendi)>
            SilAsync(int cariId);
    }
}