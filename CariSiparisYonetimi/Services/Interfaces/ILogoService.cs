using Microsoft.AspNetCore.Http;

namespace CariSiparisYonetimi.Services.Interfaces
{
    public interface ILogoService
    {
        Task<string> KaydetAsync(IFormFile dosya);

        bool Sil(string logoYolu);
    }
}