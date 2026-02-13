<div align="center">

# 📬 ULAK - Modern Email Yönetim Sistemi

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?style=for-the-badge&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=for-the-badge&logo=csharp)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-512BD4?style=for-the-badge&logo=nuget)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=for-the-badge&logo=bootstrap)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

**Modern, hızlı ve güvenli email yönetim platformu**

![Landing Page](screenshots/Giriş.jpg)

</div>

---

## 📋 İçindekiler

- [Hakkında](#-hakkında)
- [Özellikler](#-özellikler)
- [Teknolojiler](#-teknolojiler)
- [Ekran Görüntüleri](#-ekran-görüntüleri)
- [Kurulum](#-kurulum)
- [Kullanım](#-kullanım)
- [Veritabanı Şeması](#-veritabanı-şeması)
- [Proje Yapısı](#-proje-yapısı)
- [Katkıda Bulunma](#-katkıda-bulunma)
- [Lisans](#-lisans)
- [İletişim](#-iletişim)
- [Teşekkürler](#-teşekkürler)

---

## 🎯 Hakkında

**Ulak**, ASP.NET Core MVC kullanılarak geliştirilmiş modern bir email yönetim sistemidir. Proje, gerçek dünya senaryolarını simüle ederek email organizasyonu, güvenlik ve kullanıcı deneyimi odaklı profesyonel bir çözüm sunmaktadır.

### 🌟 Proje Hedefleri

- ✅ Güvenli ve ölçeklenebilir bir email platformu oluşturmak
- ✅ Modern web teknolojilerini entegre etmek
- ✅ Kullanıcı dostu ve responsive bir arayüz sunmak
- ✅ Best practice ve design pattern'leri uygulamak

---

## ✨ Özellikler

### 🔐 Güvenlik & Kimlik Doğrulama

- **ASP.NET Core Identity** ile güvenli kullanıcı yönetimi
- **6 haneli email doğrulama** sistemi
- **Gizlilik politikası** onay mekanizması
- **Email doğrulama zorunluluğu** - Doğrulanmadan giriş yapılamaz
- **Şifre sıfırlama** ve güvenli oturum kontrolü

### 📧 Email Yönetimi

- **Gelen Kutusu**: Alınan mesajları görüntüleme ve yönetme
- **Gönderilenler**: Gönderilen mesajların takibi
- **Taslaklar**: Yarım kalan mesajları kaydetme
- **Çöp Kutusu**: Silinen mesajları geçici saklama
- **Yıldızlı Mesajlar**: Önemli mesajları işaretleme
- **Kategori Sistemi**: Kullanıcı bazlı özel kategoriler (Faturalar, İş, Kişisel, vb.)
- **Gelişmiş Arama**: AJAX ile canlı arama ve filtreleme
- **Toplu İşlemler**: Çoklu mesaj seçimi ve silme
- **Header Bildirimleri**: Okunmamış mesajlar dropdown menüde

### 🎨 Kullanıcı Arayüzü

- **Modern Landing Page**: Minimalist ve etkileyici giriş sayfası
- **Dashboard**: İstatistikler ve görsel grafikler (8 widget)
- **Profil Yönetimi**: 
  - İki tab yapısı (Profil Detayları & Profil Düzenleme)
  - Fotoğraf yükleme
  - Aktivite istatistikleri
- **Rich Text Editor**: Summernote ile zengin metin düzenleme
- **Modal Sistem**: Popup ile mesaj oluşturma
- **Responsive Tasarım**: Mobil uyumlu arayüz
- **Animasyonlar**: Smooth transitions ve hover efektleri

### 📊 İstatistik & Raporlama

- **Dashboard Widgets**: 
  - Toplam Mesaj
  - Gelen Kutusu
  - Gönderilenler
  - Okunmamış
  - Yıldızlı
  - Taslaklar
  - Kategoriler
  - Bugün Gelen
- **Kategori Grafikleri**: Progress bar ile mesaj dağılımı
- **Aktivite Takibi**: Profil tamamlanma, yanıt hızı, kategori kullanımı

---

## 🛠️ Teknolojiler

### Backend

| Teknoloji | Versiyon | Açıklama |
|-----------|----------|----------|
| ![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?logo=dotnet) | 8.0 | Web framework |
| ![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp) | 12.0 | Programlama dili |
| ![Entity Framework](https://img.shields.io/badge/EF%20Core-8.0-512BD4?logo=nuget) | 8.0 | ORM |
| ![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?logo=microsoftsqlserver) | 2022 | Veritabanı |
| ![Identity](https://img.shields.io/badge/Identity-Core-512BD4) | Core | Kimlik yönetimi |

### Frontend

| Teknoloji | Versiyon | Açıklama |
|-----------|----------|----------|
| ![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?logo=bootstrap) | 5.3 | CSS Framework |
| ![jQuery](https://img.shields.io/badge/jQuery-3.7-0769AD?logo=jquery) | 3.7 | JavaScript kütüphanesi |
| ![Summernote](https://img.shields.io/badge/Summernote-0.8-orange) | 0.8 | WYSIWYG Editor |
| ![Boxicons](https://img.shields.io/badge/Boxicons-2.1-blue) | 2.1 | Icon library |

### Design Patterns & Architecture

- **MVC Pattern**: Model-View-Controller mimarisi
- **Repository Pattern**: Veri erişim katmanı soyutlaması
- **DTO Pattern**: Veri transfer nesneleri
- **Service Layer**: İş mantığı katmanı
- **Dependency Injection**: Bağımlılık enjeksiyonu

---

## 📸 Ekran Görüntüleri

### 🏠 Landing Page & Kimlik Doğrulama

<table>
  <tr>
    <td width="50%">
      <img src="screenshots/Giriş.jpg" alt="Landing Page"/>
      <p align="center"><b>Landing Page</b><br/>Modern ve minimalist giriş sayfası</p>
    </td>
    <td width="50%">
      <img src="screenshots/1kayıt_olma.jpg" alt="Kayıt Olma"/>
      <p align="center"><b>Kayıt Ekranı</b><br/>Kullanıcı kayıt formu</p>
    </td>
  </tr>
  <tr>
    <td width="50%">
      <img src="screenshots/2kayıt_olurken_gizlilik_metnini_kabul_ediyorum_popup_u_.jpg" alt="Gizlilik Politikası"/>
      <p align="center"><b>Gizlilik Politikası</b><br/>KVKK uyumlu onay popup'ı</p>
    </td>
    <td width="50%">
      <img src="screenshots/3kayıt_olduktan_sonra_e_mail_doğrulama_kodu_.jpg" alt="Email Doğrulama"/>
      <p align="center"><b>Email Doğrulama</b><br/>6 haneli doğrulama kodu</p>
    </td>
  </tr>
  <tr>
    <td width="50%">
      <img src="screenshots/51email_confirm_ekranı_.jpg" alt="Email Confirm"/>
      <p align="center"><b>Doğrulama Ekranı</b><br/>Kod giriş ekranı</p>
    </td>
    <td width="50%">
      <img src="screenshots/5Doğrulama_kodu_girilmeden_giriş_yapılamaz.jpg" alt="Email Zorunluluğu"/>
      <p align="center"><b>Güvenlik Kontrolü</b><br/>Email doğrulama zorunluluğu</p>
    </td>
  </tr>
  <tr>
    <td colspan="2">
      <img src="screenshots/4giriş_yapma.jpg" alt="Giriş Yapma"/>
      <p align="center"><b>Giriş Ekranı</b><br/>Güvenli login sistemi</p>
    </td>
  </tr>
</table>

### 📧 Email Yönetimi

<table>
  <tr>
    <td width="50%">
      <img src="screenshots/6E_MAİL_GİRİŞ_.jpg" alt="Gelen Kutusu"/>
      <p align="center"><b>Gelen Kutusu</b><br/>Ana mesaj listesi</p>
    </td>
    <td width="50%">
      <img src="screenshots/7YILDIZLI_MESAJ.jpg" alt="Yıldızlı Mesajlar"/>
      <p align="center"><b>Yıldızlı Mesajlar</b><br/>Önemli mesajlar</p>
    </td>
  </tr>
  <tr>
    <td width="50%">
      <img src="screenshots/8KATEGORİYE_GÖRE_MESAJLAR_FATURALAR.jpg" alt="Kategori Filtreleme"/>
      <p align="center"><b>Kategori - Faturalar</b><br/>Kategoriye göre filtreleme</p>
    </td>
    <td width="50%">
      <img src="screenshots/KİŞİSEL_KATEGORİYE_GÖRE.jpg" alt="Kişisel Kategori"/>
      <p align="center"><b>Kategori - Kişisel</b><br/>Kullanıcı kategorileri</p>
    </td>
  </tr>
  <tr>
    <td width="50%">
      <img src="screenshots/9SEARCH_BAR_A_GÖRE_MESAJ_ARAMA_.jpg" alt="Arama"/>
      <p align="center"><b>Gelişmiş Arama</b><br/>AJAX ile canlı arama</p>
    </td>
    <td width="50%">
      <img src="screenshots/10MAİL_DETAY_SAYFASI.jpg" alt="Mesaj Detay"/>
      <p align="center"><b>Mesaj Detayı</b><br/>Mesaj okuma ekranı</p>
    </td>
  </tr>
  <tr>
    <td width="50%">
      <img src="screenshots/111MESAJ_YAZ_POPUP_MODAL.jpg" alt="Mesaj Yazma"/>
      <p align="center"><b>Mesaj Oluştur</b><br/>Modal popup ile mesaj yazma</p>
    </td>
    <td width="50%">
      <img src="screenshots/GELEN_MESAJLAR_HEADERDA_İCON_ŞEKLİNDE_GÖZÜKÜYOR.jpg" alt="Header Bildirimleri"/>
      <p align="center"><b>Header Bildirimleri</b><br/>Okunmamış mesajlar dropdown</p>
    </td>
  </tr>
</table>

### 📊 Dashboard & Profil

<table>
  <tr>
    <td colspan="2">
      <img src="screenshots/11ANA_SAYFA_İSTATİSTİK_EKRANI_.jpg" alt="Dashboard"/>
      <p align="center"><b>Dashboard</b><br/>8 widget ile istatistikler ve grafikler</p>
    </td>
  </tr>
  <tr>
    <td width="50%">
      <img src="screenshots/12PROFİL_DETAY_SAYFASI.jpg" alt="Profil Detay"/>
      <p align="center"><b>Profil Detayları</b><br/>Kullanıcı bilgileri ve istatistikler</p>
    </td>
    <td width="50%">
      <img src="screenshots/13PROFİL_DÜZENLEME_SAYFASI.jpg" alt="Profil Düzenleme"/>
      <p align="center"><b>Profil Düzenleme</b><br/>Bilgi güncelleme ve fotoğraf yükleme</p>
    </td>
  </tr>
</table>

### ⚙️ Ayarlar

<table>
  <tr>
    <td>
      <img src="screenshots/14AYARLAR_SAYFASI.jpg" alt="Ayarlar"/>
      <p align="center"><b>Ayarlar Sayfası</b><br/>Şifre değiştirme ve hesap yönetimi</p>
    </td>
  </tr>
</table>

---

