using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using CariSiparisYonetimi.Services.Interfaces;

namespace CariSiparisYonetimi.Services
{
    public class LogoService : ILogoService
    {
        private readonly IWebHostEnvironment _environment;

        private const long MaksimumDosyaBoyutu = 2 * 1024 * 1024;

        // DI üzerinden gelen ortam bilgisini, wwwroot yoluna erişmek için saklar.
        public LogoService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        // Boyut, uzantı ve dosya imzasını kontrol edip logoyu benzersiz adla kaydeder ve web yolunu döndürür.
        public async Task<string> KaydetAsync(IFormFile dosya)
        {
            if (dosya.Length == 0)
            {
                throw new ArgumentException("Dosya boş olamaz.");
            }

            if (dosya.Length > MaksimumDosyaBoyutu)
            {
                throw new ArgumentException("Logo dosyası en fazla 2 MB olabilir.");
            }

            var uzanti = Path.GetExtension(dosya.FileName)
                .ToLowerInvariant();

            if (uzanti != ".jpg" &&
                uzanti != ".jpeg" &&
                uzanti != ".png")
            {
                throw new ArgumentException("Yalnızca JPG veya PNG dosyası yükleyebilirsiniz.");
            }

            await using var kaynak = dosya.OpenReadStream();

            var baslik = new byte[8];
            var okunan = await kaynak.ReadAsync(baslik.AsMemory());

            var pngMi = okunan == 8 &&
                baslik[0] == 0x89 &&
                baslik[1] == 0x50 &&
                baslik[2] == 0x4E &&
                baslik[3] == 0x47 &&
                baslik[4] == 0x0D &&
                baslik[5] == 0x0A &&
                baslik[6] == 0x1A &&
                baslik[7] == 0x0A;

            var jpegMi = okunan >= 3 &&
                baslik[0] == 0xFF &&
                baslik[1] == 0xD8 &&
                baslik[2] == 0xFF;

            if ((uzanti == ".png" && !pngMi) ||
                (uzanti != ".png" && !jpegMi))
            {
                throw new ArgumentException(
                    "Dosyanın içeriği seçilen resim türüyle uyuşmuyor.");
            }

            var klasorYolu = Path.Combine(
                _environment.WebRootPath, "logos");

            Directory.CreateDirectory(klasorYolu);

            var dosyaAdi = $"{Guid.NewGuid():N}{uzanti}";
            var dosyaYolu = Path.Combine(klasorYolu, dosyaAdi);

            await using var hedef = new FileStream(
                dosyaYolu, FileMode.CreateNew);

            await dosya.CopyToAsync(hedef);

            return $"/logos/{dosyaAdi}";
        }

        // Beklenen logo yolu biçimini kontrol ederek dosyayı siler; karşılanan dosya veya erişim hatalarında false döndürür.
        public bool Sil(string logoYolu)
        {
            var dosyaAdi = Path.GetFileName(logoYolu);

            if (string.IsNullOrWhiteSpace(dosyaAdi) ||
                logoYolu != $"/logos/{dosyaAdi}")
            {
                return false;
            }

            var dosyaYolu = Path.Combine(
                _environment.WebRootPath,
                "logos",
                dosyaAdi);

            try
            {
                File.Delete(dosyaYolu);

                return true;
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }
    }
}