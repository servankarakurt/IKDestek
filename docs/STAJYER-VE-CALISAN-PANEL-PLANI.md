# Stajyer ve Çalışan Panel Planı

## 1. Stajyer (Intern) girişi – karşılaşacakları ekran

| Özellik | Açıklama |
|--------|----------|
| **Kalan yıllık izin günü** | Sadece kendi kalan gün sayısı (API: my-balance veya benzeri; Stajyer için) |
| **Son 5 izin talebi** | Kendi talepleri, son 5 kayıt (tarih sıralı) |
| **İzin alma ekranı** | Sadece “talep oluştur” formu (mevcut LeaveRequest/Create) |
| **İzin talepleri tablosu** | Kendi taleplerinin listesi, durum (Beklemede / Onaylandı / Reddedildi) |
| **Mentor bilgisi** | MentorId’ye göre mentor adı, e-posta, telefon – sayfada küçük bir blok |
| **Kısıtlamalar** | Stajyer başka stajyer ekleyemez/güncelleyemez; sadece kendi verisi görünür. Menüde “Stajyerler” yok; `/Intern/Index` vb. erişim engellenecek. |

---

## 2. Çalışan (Employee – “Çalışan” rolü) girişi – karşılaşacakları ekran

| Özellik | Açıklama |
|--------|----------|
| **Kalan yıllık izin günü** | Sadece kendi kalan gün |
| **Son 5 izin talebi** | Kendi talepleri, son 5 kayıt |
| **İzin alma ekranı** | Talep oluşturma formu |
| **İzin talepleri tablosu** | Kendi talepleri, durum bilgisi |
| **Aynı birimdeki iş arkadaşları** | Aynı Department’taki çalışanlar listesi; **e-posta belirgin** |
| **Mentor isen: mentee listesi** | MentorId’si kendisi olan stajyerler, listenin en altında |
| **Kısıtlamalar** | Sadece kendi verisi; başka çalışan ekleme/güncelleme yok. Çalışanlar listesi sadece “aynı birim + e-posta” ve “mentee” için. |

---

## 3. Teknik adımlar (sıra ile)

### Faz A – Yetkilendirme ve menü
1. **Stajyer:** `Intern/Index`, `Intern/Create`, `Intern/Edit` erişimini engelle (sadece Admin/IK/Yönetici görsün). Menü zaten rol ile ayrılıyor; controller’da role kontrolü ekle.
2. **Çalışan:** `Employee/Index` (tüm listeyi açma) ve CRUD’u rol ile kısıtla; “Çalışan” rolü sadece kendi panelini görsün.

### Faz B – Stajyer paneli (tek sayfa veya dashboard)
3. **Stajyer Dashboard / Panel sayfası:**  
   - Kalan izin günü (API: Stajyer için balance – gerekirse backend’de InternId ile).  
   - Son 5 izin talebi (API’de “son 5” filtresi veya UI’da slice).  
   - “İzin talebi oluştur” linki → mevcut LeaveRequest/Create.  
   - “Taleplerim” tablosu (kendi talepleri).  
   - Mentor bilgisi kutusu: MentorId’den mentor çek (API: Employee by id veya “my-mentor” endpoint).

4. **Backend (gerekirse):**  
   - İzin talepleri şu an `EmployeeId` ile; Stajyer için **InternId** veya “requested by current user” mantığı gerekebilir.  
   - Stajyer için “my-balance” ve “my leave requests” API’lerinin current user (Intern) ile çalışması.

### Faz C – Çalışan paneli
5. **Çalışan Dashboard / Panel sayfası:**  
   - Kalan izin, son 5 talep, izin oluştur linki, taleplerim tablosu (Stajyer ile aynı mantık).  
   - **Aynı birimdeki çalışanlar:** Aynı Department’taki kişiler, e-posta vurgulu (API: örn. “colleagues” veya mevcut Employee listesini department’a göre filtrele).  
   - **Mentor ise:** MentorId’si kendisi olan stajyerler listesi (API: Interns where MentorId = current user id), sayfanın altında.

6. **Backend:**  
   - “Aynı birimdeki çalışanlar” endpoint’i (department’a göre, current user hariç veya dahil).  
   - “Benim mentee’lerim” endpoint’i (Interns filtered by MentorId = current employee id).

### Faz D – Menü ve yönlendirme
7. **Menü:**  
   - Stajyer girişinde: Anasayfa yerine veya ek olarak “Panelim” → Stajyer paneli.  
   - Çalışan girişinde: “Panelim” → Çalışan paneli (izin + birim arkadaşları + mentee’ler).  

8. **Anasayfa:**  
   - Stajyer/Çalışan için Home/Index istenirse panel sayfasına yönlendirilebilir veya panel içeriği anasayfada gösterilebilir.

---

## 4. Kısa uygulama sırası

1. **Yetkilendirme:** Stajyer için Intern controller; Çalışan için Employee controller kısıtlamaları.  
2. **API:** Gerekli endpoint’ler (my-balance zaten var; son 5 talep mevcut listeden; colleagues by department; mentees by mentor).  
3. **Stajyer panel view:** Tek sayfa: balance, son 5, link create, tablo, mentor kutusu.  
4. **Çalışan panel view:** Aynı izin blokları + birim arkadaşları (e-posta ile) + mentee listesi.  
5. **Menü ve Home:** Rol bazlı “Panelim” linki ve gerekirse yönlendirme.

Bu plana göre adım adım kod tarafında ilerlenebilir.
