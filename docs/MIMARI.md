# HRSupport — Mimari Doküman

## Katmanlar ve Sorumluluklar

### 1. HRSupport.Domain

- **Ne işe yarar:** İş kurallarının ve veri modelinin çekirdeği. Hiçbir dış katmana (EF, UI, API) bağımlı değildir.
- **İçerik:**
  - **Entities:** Employee, Intern, LeaveRequest, User, EmployeeNote, InternTask, MentorNote, EmployeeLeaveBalance, LeaveApprovalHistory, BaseEntity.
  - **Enums:** Department, Roles, LeaveType, LeaveStatus.
- **Bağımlılık:** Yok.

### 2. HRSupport.Application

- **Ne işe yarar:** Use case'ler (iş akışları), DTO'lar, repository arayüzleri, validasyon. Tüm iş mantığı burada veya Domain'de tutulur; Controller sadece isteği alıp handler'a iletir.
- **İçerik:**
  - **Features:** Her modül için Commands/Queries (MediatR), Handler'lar.
  - **DTOs:** API ve UI için veri taşıma nesneleri.
  - **Interfaces:** IEmployeeRepository, ILeaveRequestRepository, IInternRepository, IEmployeeNoteRepository, IInternTaskRepository, IMentorNoteRepository, ICurrentUserService, IJwtTokenGenerator vb.
  - **Validators:** FluentValidation (örn. CreateEmployeeValidator).
  - **Common:** Result<T>, PasswordHelper.
- **Bağımlılık:** Sadece Domain.

### 3. HRSupport.Infrastructure

- **Ne işe yarar:** Veri erişimi ve dış servisler. Repository'lerin somut implementasyonları, EF Core DbContext, migration'lar.
- **İçerik:**
  - **Context:** ApplicationDbContext (SQL Server).
  - **Repositories:** EmployeeRepository, LeaveRequestRepository, InternRepository, EmployeeNoteRepository, InternTaskRepository, MentorNoteRepository, UserRepository.
  - **Migrations:** Veritabanı şema değişiklikleri.
- **Bağımlılık:** Application + Domain. Presentation’a referans vermez.

### 4. HRSupport.WebAPI

- **Ne işe yarar:** REST API uç noktaları, JWT kimlik doğrulama, Swagger.
- **İçerik:**
  - **Controllers:** Auth, Employee, Intern, LeaveRequest, Dashboard.
  - **Services:** JwtTokenGenerator, CurrentUserService, NormalizeRoleClaimsTransformation.
  - **Program.cs:** DI kayıtları (DbContext, repository’ler, MediatR, JWT).
- **Bağımlılık:** Application + Infrastructure.

### 5. HRSupport.UI

- **Ne işe yarar:** Kullanıcı arayüzü (MVC). Sayfalar, formlar, API’ye HTTP istekleri (Session’daki JWT ile).
- **İçerik:**
  - **Controllers:** Home, Auth, Employee, Intern, LeaveRequest, Dashboard, PersonelPanel.
  - **Views:** Razor (.cshtml).
  - **Filters:** RequireLoginFilter.
  - **Handlers:** BearerTokenHandler (API isteklerine token ekler).
- **Bağımlılık:** Application (doğrudan Infrastructure’a referans yok).

### 6. HRSupport.Tests

- **Ne işe yarar:** Unit testler (xUnit + Moq). Use case’lerin doğru davranışını doğrular.
- **İçerik:** LeaveRequestValidationTests, CreateEmployeeValidatorTests, AddEmployeeNoteHandlerTests vb.
- **Bağımlılık:** Application.

---

## Bağımlılık Yönü

```
    [UI]  ──►  [WebAPI]  ──►  [Application]  ──►  [Domain]
                  │                  │
                  └──► [Infrastructure]
```

- Domain: hiçbir katmana bağımlı değil.
- Application: sadece Domain.
- Infrastructure: Application + Domain.
- WebAPI: Application + Infrastructure.
- UI: Application (API ile iletişim HTTP üzerinden).

---

## Örnek Akış: Çalışan İzin Talebi Nasıl İşleniyor?

1. **UI:** Kullanıcı “İzin Talebi Oluştur” sayfasında formu doldurur; POST ile `HRSupport.UI` → `LeaveRequestController.Create` çalışır.
2. **UI:** Controller, Session’daki JWT ile `POST /api/LeaveRequest` isteğini WebAPI’ye gönderir; body’de CreateLeaveRequestCommand (EmployeeId, StartDate, EndDate, Type, Description) gider.
3. **WebAPI:** `LeaveRequestController.CreateLeaveRequest` isteği alır; MediatR ile `CreateLeaveRequestCommand` handler’a gönderir.
4. **Application:** `CreateLeaveRequestHandler`:
   - Rol kontrolü (Çalışan/Stajyer sadece kendi EmployeeId’si için talep açabilir).
   - Personel var mı kontrolü.
   - **Validasyon:** `StartDate > EndDate` ise `Result.Failure("Başlangıç tarihi bitişten büyük olamaz.")` döner.
   - LeaveRequest entity oluşturur, `ILeaveRequestRepository.AddAsync` çağırır.
5. **Infrastructure:** `LeaveRequestRepository.AddAsync` EF Core ile kaydı veritabanına yazar.
6. **Application:** Handler `Result.Success(leaveRequestId)` döner.
7. **WebAPI:** 200 OK ve result’ı JSON olarak döner.
8. **UI:** Başarılıysa listeye yönlendirir; hata mesajını gösterir.

İş kuralları (tarih kontrolü, yetki) Controller’da değil, Application (handler) içindedir; veri erişimi Infrastructure’dadır.
