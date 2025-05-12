DotnetStore 🛒

DotnetStore, modern bir e-ticaret platformu olarak tasarlanmış, ASP.NET Core MVC ile geliştirilmiş bir web uygulamasıdır. Kullanıcıların ürünleri listeleyip detaylarını görüntüleyebildiği, satın aldıkları ürünlere yorum yapabildiği ve adminlerin ürünleri yönetebildiği bir sistem sunar. Güvenli kimlik doğrulama, sepet yönetimi ve mobil uyumlu arayüz ile eksiksiz bir alışveriş deneyimi sağlar. Şu anda Angular tabanlı bir frontend’e geçiş süreci devam etmektedir.
🎯 Projenin Amacı
DotnetStore, hem kullanıcı dostu bir alışveriş platformu hem de geliştiriciler için öğrenme ve özelleştirme imkanı sunan bir e-ticaret uygulamasıdır. Temel hedefleri:

Kullanıcılar için: Ürün keşfi, yorum yapma ve güvenli alışveriş.
Adminler için: Ürün ve kategori yönetimi.
Geliştiriciler için: ASP.NET Core MVC ve modern web teknolojileri ile ölçeklenebilir bir proje örneği.

📋 Temel Özellikler

Ürün Listeleme ve Filtreleme: Kategorilere göre ürünleri listeleme ve detaylı ürün bilgileri.
Yorum Sistemi: Satın alınan ürünlere 1-5 puan ve yazılı yorum ekleme.
Kimlik Doğrulama: Kayıt, oturum açma ve rol tabanlı yetkilendirme (Kullanıcı ve Admin).
Sepet Yönetimi: Ürünleri sepete ekleme ve sipariş oluşturma.
Admin Paneli: Ürün ekleme, düzenleme, silme ve yorum yönetimi.
Mobil Uyumluluk: Bootstrap 5 ile responsive tasarım.

🛠 Kullanılan Teknolojiler
DotnetStore, modern web geliştirme teknolojilerini bir araya getirir:

Backend:
ASP.NET Core 9.0 (MVC mimarisi)
Entity Framework Core (veritabanı işlemleri)
ASP.NET Core Identity (kimlik doğrulama ve yetkilendirme)


Veritabanı:
Microsoft SQL Server 2019 (ilişkisel veritabanı)


Frontend:
Razor Pages ve Views (MVC)
Bootstrap 5 (responsive UI)
HTML, CSS, JavaScript


Devam Eden Çalışmalar:
Angular 19 (yeni frontend için)
REST API (DotnetStoreApi projesi ile)


Araçlar:
Visual Studio 2022 (IDE)
Git (versiyon kontrol)
Postman (API testi)



🚀 Kurulum
DotnetStore’u yerel ortamınızda çalıştırmak için aşağıdaki adımları izleyin.
Gereksinimler

.NET 9.0 SDK
SQL Server 2019+
Visual Studio 2022 veya VS Code

Adımlar

Repoyu Klonlayın:
git clone https://github.com/kullanici-adin/DotnetStore.git
cd DotnetStore


Veritabanını Kurun:

appsettings.json’da bağlantı dizesini güncelleyin:"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=DotnetStore;Trusted_Connection=True;MultipleActiveResultSets=true"
}


Migration’ları çalıştırın:dotnet ef migrations add InitialCreate
dotnet ef database update




Projeyi Çalıştırın:
dotnet restore
dotnet build
dotnet run


Tarayıcıda: https://localhost:5001 (port değişebilir).


Test Kullanıcıları:

Kullanıcı: deneme@gmail.com (Parola: 123456)
Admin: arifsahin (Parola: 123456)



📸 Ekran Görüntüleri
(Yakında eklenecek: Ürün listesi, ürün detayı, yorum formu)
🛠 Gelecek Planlar

Angular frontend’e tam geçiş.
REST API ile kullanıcı deneyimini modernize etme.
Ödeme entegrasyonu ve gelişmiş sepet özellikleri.
Daha fazla dökümantasyon ve test senaryosu.

📜 Lisans
Bu proje MIT License altında lisanslanmıştır.


