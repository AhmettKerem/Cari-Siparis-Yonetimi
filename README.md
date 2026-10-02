# Cari ve Sipariş Yönetimi

Cari kartlarını ve bu carilere ait siparişleri yönetmek amacıyla geliştirilmiş ASP.NET Core MVC uygulamasıdır.

Uygulama, işletme çalışanlarının müşteri veya firma kayıtlarını oluşturmasını ve bu kayıtlar adına ürün/hizmet siparişleri yönetmesini sağlar. Cari kartı, müşteri veya firmayı temsil eder; uygulama kullanıcı hesabı değildir.

## 1. Kullanılan Teknolojiler

| Teknoloji | Kullanım Amacı |
|---|---|
| C# | Uygulama geliştirme |
| ASP.NET Core MVC | Controller, View ve HTTP isteklerinin yönetimi |
| Razor | Verilerin HTML sayfalarında gösterilmesi |
| Entity Framework Core | Veritabanı işlemleri ve entity ilişkilerinin yönetimi |
| SQL Server | Verilerin kalıcı olarak saklanması |
| EF Core Migrations | Veritabanının oluşturulması ve şema değişikliklerinin uygulanması |
| Bootstrap | Sayfa, tablo, form ve buton tasarımı |
| JavaScript | Dinamik sipariş satırları ve anlık tutar önizlemesi |
| Dependency Injection | Servis ve veritabanı bağımlılıklarının sağlanması |

Projenin hedeflediği .NET sürümü `.csproj` dosyasındaki `TargetFramework` alanından kontrol edilmelidir.

Örneğin:

```xml
<TargetFramework>net8.0</TargetFramework>
```

Bu örnekte .NET 8 SDK gereklidir. Projede farklı bir hedef sürüm varsa buna uygun SDK kurulmalıdır.

## 2. Uygulamanın Özellikleri

### Cari Kart Yönetimi

- Yeni cari kart oluşturma.
- Cari kartları listeleme.
- Cari bilgilerini düzenleme.
- Cari kartları pasife alma ve yeniden aktifleştirme.
- Cari kodunun benzersizliğini kontrol etme.
- Cari kartına logo yükleme, değiştirme ve silme.

Cari kartında aşağıdaki bilgiler tutulur:

- Cari kodu
- Cari adı / firma adı
- Vergi numarası
- Telefon
- E-posta
- Adres
- Aktiflik durumu
- Logo dosya yolu

### Sipariş Yönetimi

- Aktif bir cari adına sipariş oluşturma.
- Bir siparişe birden fazla ürün/hizmet satırı ekleme.
- Siparişleri listeleme.
- Sipariş detaylarını görüntüleme.
- Satır tutarlarını ve genel toplamı hesaplama.
- Siparişi silmeden iptal etme.

İptal edilen sipariş yeniden aktifleştirilemez. Yeni bir sipariş gerektiğinde yeni kayıt oluşturulur.

## 3. Kurulum Gereksinimleri

Projeyi çalıştırmak için:

- Projenin hedeflediği sürümle uyumlu .NET SDK.
- Visual Studio kullanılıyorsa **ASP.NET ve web geliştirme** iş yükü.
- SQL Server veya SQL Server Express.
- İsteğe bağlı SQL Server Management Studio (SSMS).

LocalDB kuruluysa geliştirme ortamında alternatif olarak kullanılabilir.

> SSMS, veritabanını yönetmek için kullanılan araçtır. Yalnızca SSMS kurulması SQL Server veritabanı motorunun kurulduğu anlamına gelmez.

## 4. Projeyi Açma

1. Kaynak kodunu bilgisayarınıza indirin veya kopyalayın.
2. Visual Studio üzerinden `.sln` dosyasını açın.
3. Çözüm dosyası bulunmuyorsa `.csproj` dosyasını açın.
4. MVC uygulamasını başlangıç projesi olarak seçin.
5. NuGet paketlerini geri yükleyin.
6. Projeyi derleyin.

Komut satırı kullanılıyorsa `.csproj` dosyasının bulunduğu klasörde:

```bash
dotnet restore
dotnet build
```

## 5. Veritabanı Bağlantısını Ayarlama

Veritabanı bağlantısı `appsettings.json` dosyasındaki `ConnectionStrings` bölümünde tanımlanır.

Öncelikle `Program.cs` dosyasındaki aşağıdaki çağrıda kullanılan bağlantı adını kontrol edin:

```csharp
builder.Configuration.GetConnectionString("DefaultConnection")
```

Buradaki ad ile `appsettings.json` içindeki bağlantı anahtarı aynı olmalıdır.

Aşağıdaki örneklerde:

- Bağlantı adı: `DefaultConnection`
- Veritabanı adı: `CariSiparisYonetimiDb`

olarak kabul edilmiştir. Mevcut projedeki isimlere ve bilgisayarınızdaki SQL Server sunucusuna göre düzenleyin.

### SQL Server Express Örneği

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=CariSiparisYonetimiDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### LocalDB Örneği

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=CariSiparisYonetimiDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Bağlantı ayarlarının anlamları:

| Ayar | Açıklama |
|---|---|
| `Server` | SQL Server sunucu veya instance adı |
| `Database` | Kullanılacak veritabanı adı |
| `Trusted_Connection=True` | Windows kimlik doğrulamasını kullanır |
| `TrustServerCertificate=True` | Yerel geliştirmede sunucu sertifikasına güvenilmesini sağlar |

`appsettings.json` dosyasındaki diğer ayarları silmeden bağlantı bölümünü düzenleyin.

`appsettings.Development.json` veya ortam değişkenlerinde aynı bağlantı tanımlanmışsa bu değerler bağlantı ayarını geçersiz kılabilir.

Windows kimlik doğrulaması kullanılırken uygulamayı çalıştıran hesabın SQL Server üzerinde gerekli erişim ve veritabanı oluşturma yetkileri bulunmalıdır.

Gerçek kullanıcı adı ve parolaları kaynak koduna eklemeyin.

## 6. Veritabanının Oluşturulması

Proje **EF Core Code First** yaklaşımını kullanır.

Bu yaklaşımda:

1. Entity sınıfları kodda tanımlanır.
2. İlişkiler ve indeksler configuration dosyalarında yapılandırılır.
3. Migration dosyaları şema değişikliklerini tanımlar.
4. Migration'lar SQL Server'a uygulanarak veritabanı oluşturulur.

### Visual Studio ile Oluşturma

1. Bağlantı ayarını düzenleyin.
2. Projeyi derleyin.
3. **Araçlar → NuGet Paket Yöneticisi → Paket Yöneticisi Konsolu** bölümünü açın.
4. Konsoldaki varsayılan proje olarak `AppDbContext` ve migration dosyalarının bulunduğu MVC projesini seçin.
5. Aşağıdaki komutu çalıştırın:

```powershell
Update-Database
```

Bu komut mevcut migration'ları uygular. Yetkiler uygunsa veritabanı bulunmadığında oluşturulur; tablolar, anahtarlar ve indeksler eklenir.

Mevcut veritabanında yalnızca henüz uygulanmamış migration'lar çalıştırılır.

> Projeyi ilk kez çalıştıran kişinin `Add-Migration` çalıştırması gerekmez. Kaynak koduyla birlikte gelen migration'ları `Update-Database` ile uygulaması yeterlidir.

Sipariş iptal alanını ekleyen migration dahil olmak üzere `Migrations` klasörü teslim edilen kaynak kodunda bulunmalıdır.

### Komut Satırı ile Oluşturma

`.csproj` dosyasının bulunduğu klasörde:

```bash
dotnet restore
dotnet build
dotnet ef database update
```

`dotnet ef` komutu bulunamıyorsa EF Core CLI aracı kurulmalıdır.

Aşağıdaki `X.Y.Z` yerine projedeki EF Core paketleriyle uyumlu sürümü yazın:

```bash
dotnet tool install --global dotnet-ef --version X.Y.Z
```

Yerel araç manifesti bulunan projelerde bunun yerine:

```bash
dotnet tool restore
```

kullanılabilir.

EF CLI işlemleri için `Microsoft.EntityFrameworkCore.Design`, Visual Studio Paket Yöneticisi Konsolu işlemleri için `Microsoft.EntityFrameworkCore.Tools` bağımlılıkları bulunmalıdır.

### Sonraki Model Değişiklikleri

Geliştirici kalıcı bir alan veya ilişki eklerse yeni migration oluşturup uygular:

```powershell
Add-Migration DegisiklikAdi
Update-Database
```

Yalnızca View, ViewModel, controller veya servis kodunun değişmesi, EF veritabanı modeli değişmedikçe migration gerektirmez.

Normal kullanıcı kayıtları `SaveChangesAsync()` üzerinden kaydedilir. Her cari veya sipariş ekleme işleminden sonra `Update-Database` çalıştırılmaz.

## 7. Uygulamayı Çalıştırma

### Visual Studio

1. SQL Server bağlantısını ayarlayın.
2. `Update-Database` komutunu çalıştırın.
3. MVC projesini başlangıç projesi olarak seçin.
4. **F5** veya **Ctrl+F5** ile uygulamayı başlatın.
5. Açılan tarayıcıda ana sayfayı görüntüleyin.
6. **Cariler** ve **Siparişler** bağlantılarıyla ilgili sayfalara geçin.

### Komut Satırı

Proje klasöründe:

```bash
dotnet run
```

Terminaldeki `Now listening on` satırında gösterilen adresi tarayıcıda açın.

Port numarası geliştirme ayarlarına göre değişebilir. Sabit bir localhost portu varsayılmamalıdır.

Yerel HTTPS sertifikası nedeniyle uyarı oluşursa:

```bash
dotnet dev-certs https --trust
```

komutu kullanılabilir.

> Yeni oluşturulan veritabanında önce en az bir aktif cari eklenmelidir. Sipariş oluşturmak için aktif bir cari gerekir.

Migration işlemi veritabanının yapısını oluşturur. Başka bir bilgisayardaki mevcut cari, sipariş ve logo dosyalarını otomatik olarak taşımaz.

## 8. Kullanım Senaryoları

### Cari Ekleme ve Düzenleme

1. **Cariler** sayfasını açın.
2. Cari ekleme butonuna basın.
3. Cari kodu, cari adı ve diğer bilgileri girin.
4. Kaydedin.
5. Listeden **Düzenle** butonuyla mevcut bilgileri güncelleyin.

Düzenleme mevcut kaydı günceller; yeni cari oluşturmaz. Kaydın `Id` değeri korunur.

Cari kodu başka bir kayda ait olamaz. Ancak cari kendi kodu değiştirilmeden düzenlenebilir.

### Cariyi Pasife Alma

Pasife alma işleminde cari silinmez:

```text
AktifMi = false
```

- Cari kaydı korunur.
- Geçmiş siparişlerle bağlantısı korunur.
- Yeni sipariş oluşturma listesinden çıkarılır.
- Gerektiğinde yeniden aktifleştirilebilir.

### Logo İşlemleri

- JPG/JPEG/PNG dosyaları kabul edilir.
- Dosya boyutu en fazla 2 MB olabilir.
- Dosya uzantısı ve içeriği kontrol edilir.
- Logo yüklenebilir, değiştirilebilir ve silinebilir.

Görseller:

```text
wwwroot/logos
```

klasöründe tutulur.

Veritabanında ise `LogoYolu` alanında dosyanın erişim yolu saklanır. Görselin kendisi veritabanında saklanmaz.

Uygulamanın logo klasörüne yazma yetkisi bulunmalıdır. Mevcut veriler başka ortama taşınırken logo dosyaları da ayrıca taşınmalıdır.

### Sipariş Oluşturma

1. **Siparişler → Yeni Sipariş** sayfasını açın.
2. Benzersiz sipariş numarasını girin.
3. Sipariş tarihini seçin.
4. Aktif bir cari seçin.
5. İsteğe bağlı açıklama girin.
6. Bir veya daha fazla ürün/hizmet satırı ekleyin.
7. Miktar ve birim fiyatları girin.
8. Siparişi kaydedin.
9. Liste veya detay sayfasından kontrol edin.

Geçerli gelecek tarihli siparişlere izin verilir. Mevcut kapsamda “sipariş tarihi bugünden ileri olamaz” kuralı veya ayrı teslim tarihi alanı bulunmaz.

### Sipariş İptali

1. Sipariş listesinde **İptal Et** butonuna basın.
2. Onay penceresinde işlemi onaylayın.
3. Siparişin durumu **İptal Edildi** olarak değişir.

Veritabanında:

```text
IptalMi = true
```

olarak kaydedilir.

- Sipariş ve detayları silinmez.
- Tutar bilgileri geçmişi göstermek için korunur.
- İptal butonu artık gösterilmez.
- Yeniden aktifleştirme işlemi bulunmaz.
- Yeni sipariş için yeni kayıt oluşturulur.

## 9. Veritabanı Yapısı

### Tablolar

| Tablo | Veritabanında Saklanan Alanlar |
|---|---|
| Cariler | Id, CariKodu, CariAdi, VergiNumarasi, Telefon, Eposta, Adres, AktifMi, LogoYolu |
| Siparisler | Id, SiparisNo, SiparisTarihi, CariId, Aciklama, IptalMi |
| SiparisDetaylari | Id, SiparisId, UrunHizmetAdi, Miktar, BirimFiyat |

### Anahtarlar ve İlişkiler

- Her tabloda `Id` birincil anahtardır.
- `Siparisler.CariId`, `Cariler.Id` alanına bağlanan yabancı anahtardır.
- `SiparisDetaylari.SiparisId`, `Siparisler.Id` alanına bağlanan yabancı anahtardır.
- Bir carinin sıfır veya birden fazla siparişi olabilir.
- Her sipariş tek cariye aittir.
- Bir sipariş birden fazla detay satırı içerebilir.
- Her detay satırı tek siparişe aittir.
- Uygulama sipariş oluştururken en az bir detay satırı ister.

Her iki ilişki de **bire çok** ilişkisidir.

### Veritabanı Kuralları

- `CariKodu` benzersizdir.
- `SiparisNo` benzersizdir.
- Siparişi bulunan carinin fiziksel olarak silinmesi `DeleteBehavior.Restrict` ile engellenir.
- Sipariş fiziksel olarak silinirse detayları `DeleteBehavior.Cascade` ile silinir.
- Sipariş iptali silme işlemi değildir; detaylar korunur.
- Miktar alanı `decimal(18,3)` hassasiyetindedir.
- Birim fiyat alanı `decimal(18,2)` hassasiyetindedir.

Navigation property'ler ilişkili C# nesnelerini temsil eder; ayrıca SQL sütunları değildir.

## 10. Tutar Hesaplama

Hesaplamalar:

```text
Satır tutarı = Miktar × Birim fiyat
Sipariş toplamı = Bütün satır tutarlarının toplamı
```

Modelde:

```csharp
[NotMapped]
public decimal Tutar => Miktar * BirimFiyat;
```

kullanılır.

`[NotMapped]`, bu özellik için veritabanında sütun oluşturulmamasını sağlar.

Miktar ve birim fiyat zaten saklandığı için satır tutarı ve sipariş toplamı ayrıca tutulmaz. Böylece miktar veya fiyat değiştiğinde tutarın eski kalması riski önlenir.

JavaScript, sipariş oluşturma ekranında anlık önizleme sağlar. Kayıtlı siparişin tutarı, saklanan miktar ve birim fiyat üzerinden hesaplanır.

Tutarlar ekranda Türkçe sayı biçimiyle iki ondalık basamak üzerinden gösterilir.

İptal edilen siparişlerde tutar geçmiş bilgi olarak görüntülenir. İleride ciro raporu eklenirse iptal edilmiş siparişlerin rapor toplamına dahil edilmesi iş kurallarına göre ayrıca ele alınmalıdır.

## 11. Projenin Katmanları

Proje tek uygulama içinde klasörler üzerinden sorumluluklara ayrılmıştır.

| Klasör / Dosya | Sorumluluk |
|---|---|
| Models | Veritabanına eşlenen entity sınıfları |
| ViewModels | Formlara özel veri ve doğrulama yapıları |
| Data / AppDbContext.cs | EF Core bağlamı ve DbSet tanımları |
| Data / Configurations | İlişkiler, indeksler ve sütun ayarları |
| Interfaces | Servis metotlarının sözleşmeleri |
| Services | İş kuralları, kayıt ve logo işlemleri |
| Controllers | HTTP istekleri, form kontrolleri ve yönlendirmeler |
| Views | Razor sayfaları |
| Migrations | Veritabanı şema değişiklikleri |
| wwwroot | Statik dosyalar ve logolar |
| Program.cs | DI, veritabanı ve uygulama yapılandırması |
| appsettings.json | Bağlantı ve uygulama ayarları |

### İstek Akışı

Kullanıcı formu gönderdiğinde:

1. HTTP isteği ilgili controller action'ına ulaşır.
2. Controller form verilerini ve doğrulama sonucunu kontrol eder.
3. Controller, interface üzerinden servisi çağırır.
4. Servis iş kurallarını uygulayıp AppDbContext'i kullanır.
5. EF Core SQL Server üzerinde gerekli işlemi gerçekleştirir.
6. Controller sonucu kullanıcıya mesaj veya sayfa olarak döndürür.

Okuma işlemlerinde controller View'a model gönderir; Razor bu verilerle HTML oluşturur.

### Dependency Injection

Servisler ve AppDbContext, `Program.cs` üzerinden DI sistemine kaydedilir. Controller ve servis constructor'larında ihtiyaç duyulan bağımlılıklar alınır.

Bu yaklaşım sorumlulukların ayrılmasını ve somut servis sınıflarına doğrudan bağımlılığın azaltılmasını sağlar.

### MVC Endpoint'leri

Varsayılan route:

```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

Örneğin:

```text
/Siparisler/Detay/5
```

adresi, SiparislerController içindeki Detay action'ına `id = 5` ile ulaşır.

Kayıt bulunamazsa 404 yanıtı verilir.

Proje MVC endpoint'leri içerir; ayrı bir REST API veya harici API entegrasyonu bulunmaz.

## 12. Temel Kullanıcı Giriş Kontrolleri

### Cari

- Cari kodu zorunludur.
- Cari adı zorunludur.
- Cari kodu benzersiz olmalıdır.
- Alan uzunluğu sınırları uygulanır.
- Düzenleme sırasında kaydın kendi kodu mükerrer kabul edilmez.

### Sipariş

- Sipariş numarası zorunlu ve benzersiz olmalıdır.
- Geçerli ve aktif bir cari seçilmelidir.
- En az bir detay satırı bulunmalıdır.
- Ürün/hizmet adı zorunludur.
- Miktar en az `0,001` olmalıdır.
- Birim fiyat negatif olamaz; sıfır kabul edilir.
- Sayısal üst sınırlar kontrol edilir.
- Miktar en fazla üç, birim fiyat en fazla iki ondalık basamak içermelidir.
- Geçersiz tarih ve sayısal girişler reddedilir.

### Logo

- Dosya seçilmelidir.
- Desteklenen dosya türü kullanılmalıdır.
- Dosya içeriği kontrol edilir.
- Dosya boyutu sınırı uygulanır.
- Başarısız yüklemede mevcut logo korunmalıdır.

Veri değiştiren form işlemlerinde anti-forgery doğrulaması kullanılır. Kontroller yalnızca tarayıcıya bırakılmaz; controller ve servis tarafında da uygulanır.

## 13. Manuel Test Senaryoları

| Test | Beklenen Sonuç |
|---|---|
| Geçerli cari ekleme | Kayıt listede ve veritabanında görünür |
| Cari bilgilerini düzenleme | Aynı Id ile mevcut kayıt güncellenir |
| Aynı cari koduyla ikinci kayıt | Kaydetme engellenir |
| Kendi kodunu koruyarak düzenleme | İşlem başarılı olur |
| Başka carinin kodunu kullanma | İşlem reddedilir |
| Logo yükleme/değiştirme/silme | Ekran, dosya ve LogoYolu uyumlu güncellenir |
| Pasif cari | Yeni sipariş seçiminde görünmez |
| Çok satırlı sipariş | Satırlar ve cari bağlantısı korunur |
| Hatalı miktar/fiyat | Hatalı kayıt oluşturulmaz |
| Sipariş iptalinden vazgeçme | Durum değişmez |
| Sipariş iptalini onaylama | IptalMi true olur; detaylar korunur |
| Sayfayı yenileme | Kaydedilmiş bilgiler korunur |

Tutar testi için:

| Ürün/Hizmet | Miktar | Birim Fiyat | Satır Tutarı |
|---|---:|---:|---:|
| Test A | 2 | 100 | 200 |
| Test B | 3 | 50 | 150 |
| Test C | 1,5 | 80 | 120 |
| **Toplam** | | | **470 TL** |

## 14. SQL ile Kontrol

SSMS'de uygulamanın veritabanını seçin. Aşağıdaki cari ve sipariş numaralarını kendi test kayıtlarınıza göre değiştirin.

### Cari ve Logo Bilgileri

```sql
SELECT Id, CariKodu, CariAdi, AktifMi, LogoYolu
FROM Cariler
WHERE CariKodu = N'TEST001';
```

### Siparişin Cari Bağlantısı ve İptal Durumu

```sql
SELECT
    s.Id,
    s.SiparisNo,
    s.CariId,
    c.CariKodu,
    c.CariAdi,
    s.IptalMi
FROM Siparisler AS s
INNER JOIN Cariler AS c ON c.Id = s.CariId
WHERE s.SiparisNo = N'TEST-SIP-001';
```

### Satır Tutarları

```sql
SELECT
    d.UrunHizmetAdi,
    d.Miktar,
    d.BirimFiyat,
    d.Miktar * d.BirimFiyat AS SatirTutari
FROM SiparisDetaylari AS d
INNER JOIN Siparisler AS s ON s.Id = d.SiparisId
WHERE s.SiparisNo = N'TEST-SIP-001';
```

### Sipariş Toplamı

```sql
SELECT
    s.SiparisNo,
    SUM(d.Miktar * d.BirimFiyat) AS SiparisToplami
FROM Siparisler AS s
INNER JOIN SiparisDetaylari AS d ON d.SiparisId = s.Id
WHERE s.SiparisNo = N'TEST-SIP-001'
GROUP BY s.SiparisNo;
```

Bu sorgular veri değiştirmez. SatırTutari ve SiparisToplami sorgu sırasında hesaplanır; veritabanına sütun olarak eklenmez.

## 15. Sık Karşılaşılan Sorunlar

| Sorun | Kontrol |
|---|---|
| SQL Server bağlantı hatası | Sunucu adı, SQL servisi, kimlik doğrulama ve bağlantı ayarları |
| Tablo bulunamaması | Doğru veritabanında Update-Database çalıştırılması |
| IptalMi sütunu bulunamaması | İlgili migration'ın uygulanması |
| Derleme / SDK hatası | Hedef .NET sürümü ve NuGet paketleri |
| Sipariş detayında 404 | Var olan siparişin Id değeriyle erişim |
| Cari listesinin boş olması | En az bir aktif cari bulunması |
| Logo görünmemesi | LogoYolu, wwwroot/logos içeriği ve statik dosya ayarları |
| Logo yüklenememesi | Dosya türü, boyutu ve klasör yazma yetkisi |

## 16. Kapsam ve Teslim

Uygulamanın yerel ortamda çalıştırılması yeterlidir. Ödev kapsamında canlıya alma veya cloud deployment zorunlu değildir.

Teslim edilen kaynak kodunda:

- Proje ve çözüm dosyaları.
- Entity, servis, controller ve View dosyaları.
- Configuration ve migration dosyaları.
- Gerekli statik dosyalar.
- Bağlantı yapılandırması.
- README dosyası.

bulunmalıdır.

Mevcut kapsamda aşağıdaki özellikler bulunmaz:

- Kullanıcı giriş sistemi ve yetkilendirme.
- Çoklu firma yapısı.
- Stok takibi.
- Fatura, KDV ve iskonto.
- E-posta veya PDF üretimi.
- Ayrı REST API geliştirme.
- Gelişmiş raporlama.
- Docker ve cloud deployment.
- Otomatik test paketi.
- Sipariş düzenleme ve fiziksel sipariş silme ekranları.

## 17. İleride Eklenebilecek Özellikler

İhtiyaç oluşması halinde:

- Kullanıcı ve rol yönetimi.
- Sipariş durumlarının genişletilmesi.
- İptal zamanı ve iptal nedeni.
- Filtreleme, arama ve sayfalama.
- İş kurallarına uygun raporlama.

eklenebilir.

Bu özellikler mevcut uygulamanın parçası değildir; olası geliştirme fikirleridir. Yeni kalıcı alan veya ilişkiler eklenirse veritabanı modeli ve migration'lar birlikte güncellenmelidir.