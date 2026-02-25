# HRSupport — Veri Modeli / Veritabanı Şeması Özeti

Aşağıda ana tablolar ve ilişkiler özetlenmiştir. Tam şema için EF Core migration dosyalarına bakılabilir.

## Ana Tablolar

| Tablo | Açıklama |
|-------|----------|
| **Users** | Sistem kullanıcıları (Admin/İK); giriş için Email, PasswordHash, Role. |
| **Employees** | Çalışanlar. Ad, soyad, e-posta, telefon, TCKN (opsiyonel), CardID, doğum/işe giriş tarihi, Department, Roles, PasswordHash (personel girişi). |
| **Interns** | Stajyerler. Ad, soyad, e-posta, üniversite, bölüm, sınıf, staj başlangıç/bitiş, MentorId (FK → Employees). |
| **LeaveRequests** | İzin talepleri. EmployeeId (FK → Employees veya çalışan/stajyer için ilgili kullanıcı id), StartDate, EndDate, Type, Status, Description. |
| **LeaveApprovalHistory** | İzin onay/red geçmişi. LeaveRequestId, ProcessedByUserId, Action, Comments, ActionDate. |
| **EmployeeLeaveBalances** | Çalışan yıllık izin bakiyesi. EmployeeId, RemainingAnnualLeaveDays. |
| **EmployeeNotes** | Çalışana ait notlar (HR/Yönetici). EmployeeId, NoteText, CreatedByUserId, CreatedByUserName. |
| **InternTasks** | Stajyere verilen görevler. InternId, Title, Description, IsCompleted. |
| **MentorNotes** | Mentor geri bildirim notları. InternId, MentorId (opsiyonel), NoteText, NoteDate. |

## Ortak Alanlar (BaseEntity)

Tablolarda genelde:

- **Id** (PK)
- **CreatedTime**
- **Isactive**
- **IsDeleted** (soft delete)

## İlişkiler (Özet)

- **Interns.MentorId** → Employees.Id (opsiyonel)
- **LeaveRequests.EmployeeId** → Employees.Id veya (stajyer senaryosunda) ilgili kullanıcı kimliği
- **LeaveApprovalHistory.LeaveRequestId** → LeaveRequests.Id
- **EmployeeNotes.EmployeeId** → Employees.Id
- **InternTasks.InternId** → Interns.Id
- **MentorNotes.InternId** → Interns.Id; **MentorNotes.MentorId** → Employees.Id (opsiyonel)
- **EmployeeLeaveBalances.EmployeeId** → Employees.Id

## Enum Değerleri (Sayısal)

- **Department:** Yazilim=1, InsanKaynaklari=2, Satis=3, Muhasebe=4, Pazarlama=5, Operasyon=6, Acente=7.
- **Roles:** Admin, IK, Yönetici, Çalışan, Stajyer.
- **LeaveType:** Yıllık, Hastalık, Mazeret, Ücretsiz, vb.
- **LeaveStatus:** Beklemede=1, Reddedildi=2, Onaylandı=3, RevizyonBekleniyor=4.

Detaylı sütun listesi için `HRSupport.Infrastructure/Migrations` altındaki migration dosyalarına bakınız.
