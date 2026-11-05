# 🚀 پلتفرم سازمانی مدیریت وظایف تیمی | TTM
<p align="right">
  <a href="README.md">🌐 Read in English (نسخه انگلیسی)</a>
</p>
<div dir="rtl">

سامانه‌ای برای ثبت کارهای مشترک یک تیم، تقسیم آن‌ها میان اعضا و پیگیری وضعیت هر وظیفه تا پایان.

ساخته‌شده با **ASP.NET Core 8 MVC و Web API**، **Clean Architecture**، **DDD** و **CQRS**.

> مستندات کامل (کاربران، توسعه‌دهندگان و بررسی فنی) در صفحه `/Home/Docs` در دسترس است.

---

## قابلیت‌ها

- ثبت‌نام و ورود با ایمیل و رمز عبور
- ساخت تیم و تعریف عنوان نقش برای خود
- دعوت اعضا با ایمیل یا شناسه، پیشنهاد سطح دسترسی و عنوان نقش
- پذیرش، رد و لغو دعوتنامه
- سلسله‌مراتب سه‌سطحی: مالک، سرپرست و کارمند
- چرخه کامل وظیفه: ایجاد، شروع، اعلام نتیجه، بستن یا درخواست تلاش مجدد
- ددلاین اختیاری و بستن خودکار وظیفه با شکست در ددلاین
- رابط وب واکنش‌گرا با حالت تاریک و ناوبری موبایل

---

## اجرای محلی

### پیش‌نیازها

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server یا پایگاه داده سازگار با تنظیمات پروژه

### مراحل

```bash
git clone https://github.com/ofc-parviz/ttm
cd ttm
dotnet restore
dotnet build
dotnet ef database update --project TeamTaskManagement.Persistence --startup-project Front/TeamTaskManagement.Web
dotnet run --project Front/TeamTaskManagement.Web
```

رشته اتصال به پایگاه داده و تنظیمات JWT را در `appsettings.Development.json` یا **User Secrets** قرار دهید. این مقادیر را در مخزن کامیت نکنید.

اجرای تست‌ها:

```bash
dotnet test
```

---

## ساختار سلوشن

سلوشن از ۸ پروژه تشکیل شده است. وابستگی‌ها همیشه به سمت لایه‌های داخلی هستند.

```text
TeamTaskManagement.sln
├─ Front/
│  └─ TeamTaskManagement.Web            MVC · Razor · Web API · Cookie · JWT
├─ Shared/
│  └─ TeamTaskManagement.Shared         Result<T> و قراردادهای مشترک
├─ TeamTaskManagement.Application       Commands · Queries · Behaviors · Validators
├─ TeamTaskManagement.Domain            Aggregates · Entities · ValueObjects · Enums · Errors · Events · Specifications
│  └─ Shared/                           نوع‌های کمکی مخصوص دامین
├─ TeamTaskManagement.Infrastructure    سرویس‌های خارجی: هش رمز، صدور JWT، احراز هویت، ایمیل
├─ TeamTaskManagement.Persistence       EF Core · Configurations · Repositories · QuerySpecifications · انتشار رویدادهای دامین
└─ Tests/
   ├─ TeamTaskManagement.Domain.Tests       تست‌های واحد دامین
   └─ TeamTaskManagement.IntegrationTests   تست‌های API و پایگاه داده
```

### مسئولیت لایه‌ها

| پروژه | مسئولیت |
|---|---|
| `Web` | صفحات Razor، Web API، احراز هویت با Cookie و JWT، ثبت وابستگی‌ها (Composition Root) |
| `Application` | Command و Query با MediatR، مدل‌های Response، Pipeline Behaviors، اعتبارسنجی با FluentValidation |
| `Domain` | اگریگیت‌ها، موجودیت‌ها، Value Object ها، Enum های وضعیت، خطاهای دامین، رویدادها، Specification ها و اینترفیس‌های Repository |
| `Shared` | نوع نتیجه `Result<T>` و قراردادهای مشترک |
| `Infrastructure` | هش کردن رمز عبور، صدور توکن JWT، احراز هویت و ارسال ایمیل |
| `Persistence` | EF Core، پیکربندی Entity ها، پیاده‌سازی Repository ها، QuerySpecification بر پایه Specification های دامین، انتشار رویدادهای دامین |

### قانون وابستگی

- `Web` به `Application` و برای ثبت سرویس‌ها به `Infrastructure` و `Persistence` وابسته است.
- `Application` به `Domain` و `Shared` وابسته است.
- `Infrastructure` و `Persistence` به `Domain` و `Application` وابسته‌اند.
- `Domain` فقط به `Shared` وابسته است و به ASP.NET، EF Core یا MediatR وابستگی ندارد.

---

## مدل دامین

سه اگریگیت ریشه وجود دارد: `Team`، `User` و `TeamTask`. ارجاع بین آن‌ها فقط با شناسه (`Guid`) انجام می‌شود.

- **Team:** شامل `TeamMember` و `TeamInvite` به‌عنوان فرزند. ساخت تیم، مالک را همزمان به‌عنوان عضو اول ثبت می‌کند. سازنده‌ها و متدهای تغییر فرزندان `internal` هستند.
- **TeamTask:** اگریگیت جداگانه برای چرخه عمر وظیفه. مجوز هر عملیات بر اساس `ActingMember` (شناسه و سطح دسترسی عضو درخواست‌کننده) تصمیم گرفته می‌شود.
- **User:** اگریگیت مستقل برای کاربر. ساخت کاربر رویداد `UserRegisteredEvent` را منتشر می‌کند.

### سطح دسترسی

| عملیات | مالک | سرپرست | کارمند |
|---|:---:|:---:|:---:|
| ویرایش اطلاعات تیم | ✅ | — | — |
| ارسال دعوتنامه | ✅ | ✅ (فقط نقش کارمند) | — |
| تغییر نقش و سطح دسترسی اعضا | ✅ | — | — |
| حذف عضو | ✅ | — | — |
| لغو دعوتنامه | ✅ | ✅ (فقط دعوتنامه‌های خودش) | — |
| ایجاد وظیفه | ✅ | ✅ | ✅ (فقط برای خود) |
| انجام وظایف محول‌شده | ✅ | ✅ | ✅ |
| ترک تیم | — | ✅ | ✅ |

---

## قوانین کسب‌وکار

### تیم و دعوت

- تیم همیشه یک مالک دارد. مالک نه از طریق دعوت قابل واگذاری است و نه از طریق تغییر نقش قابل تنزل.
- کاربر عضو فعال دعوتنامه جدید نمی‌گیرد. برای هر کاربر در هر تیم حداکثر یک دعوتنامه «ارسال‌شده» وجود دارد.
- پذیرش یک دعوتنامه، سایر دعوتنامه‌های باز همان کاربر در همان تیم را به‌صورت خودکار لغو می‌کند.
- مالک تیم نمی‌تواند از تیم خارج شود.
- عضو خارج‌شده یا حذف‌شده با دعوتنامه تازه می‌تواند دوباره بپیوندد. رکورد عضو حذف نمی‌شود.

### وظیفه

- وظیفه با ددلاین گذشته ساخته یا ویرایش نمی‌شود. ددلاین اختیاری است.
- وظیفه‌ای که بسته شده، قابل ویرایش جزئیات، ددلاین، محول‌کننده یا مسئول نیست.
- تغییر مسئول وظیفه، وضعیت آن را به «در انتظار» برمی‌گرداند.

### انتقال وضعیت وظیفه

| از وضعیت | به وضعیت | متد | چه کسی |
|---|---|---|---|
| `Pending` | `OnGoing` | `Start` | مسئول |
| `OnGoing`، `AcceptedToTryAgain` | `ClaimedAsComplete` | `ClaimComplete` | مسئول |
| `OnGoing`، `AcceptedToTryAgain` | `ClaimedAsFailure` | `ClaimFailure` | مسئول |
| `ClaimedAsComplete`، `ClaimedAsFailure` | `OrderedToTryAgain` | `OrderRetry` | محول‌کننده یا مالک |
| `OrderedToTryAgain` | `AcceptedToTryAgain` | `AcceptRetry` | مسئول |
| `OrderedToTryAgain` | `RejectedToTryAgain` | `RejectRetry` | مسئول |
| `ClaimedAsComplete`، `AcceptedToTryAgain` | `ClosedAsComplete` | `CloseAsComplete` | محول‌کننده یا مالک |
| `ClaimedAsFailure`، `RejectedToTryAgain` | `ClosedAsFailure` | `CloseAsFailure` | محول‌کننده یا مالک |
| هر وضعیت باز | `ClosedAsDeadlineFailure` | `CloseAsDeadlineFailure` | فرایند زمان‌بندی‌شده |
| هر وضعیت باز | `Pending` | `RotateAssignee` | محول‌کننده یا مالک |

---

## احراز هویت

سامانه دو کانال دارد که هسته آن یکی است:

- **رابط وب (MVC):** پس از ورود، هویت کاربر در **Cookie** نگهداری می‌شود.
- **رابط برنامه‌نویسی (API):** با `POST /api/auth/login` توکن `accessToken` دریافت می‌شود. درخواست‌های محافظت‌شده باید این هدر را داشته باشند:

```http
Authorization: Bearer <accessToken>
```

هش کردن رمز عبور و صدور JWT در لایه `Infrastructure` انجام می‌شود. دسترسی کاربر به تیم و وظیفه در لایه دامین بررسی می‌شود؛ دکمه‌های رابط کاربری فقط برای راحتی کاربر هستند.

---

## تست‌ها

- **`Domain.Tests`:** تست واحد قواعد کسب‌وکار، انتقال وضعیت وظیفه و جریان دعوتنامه. بدون نیاز به پایگاه داده.
- **`IntegrationTests`:** جریان کامل API و تعامل با پایگاه داده. پیش از اجرا تنظیمات اتصال به پایگاه داده آزمایشی را بررسی کنید.

---

## مرجع API

تمام پاسخ‌ها JSON هستند. در خطاها، فیلد `code` برای برنامه و `fa_message` برای نمایش فارسی به کاربر در نظر گرفته شده است.

### احراز هویت و پروفایل

| متد | مسیر | هدف |
|---|---|---|
| `POST` | `/api/auth/register` | ثبت‌نام |
| `POST` | `/api/auth/login` | ورود و دریافت توکن |
| `GET` | `/api/profile` | مشاهده پروفایل |
| `PUT` | `/api/profile` | ویرایش پروفایل |
| `POST` | `/api/profile/new-password` | تغییر رمز عبور |

### تیم‌ها

| متد | مسیر | هدف |
|---|---|---|
| `GET` | `/api/teams` | فهرست تیم‌های کاربر |
| `GET` | `/api/teams/{id}` | جزئیات تیم |
| `POST` | `/api/teams` | ساخت تیم |
| `PUT` | `/api/teams` | ویرایش تیم |
| `DELETE` | `/api/teams/{teamId}/leave` | ترک تیم |
| `DELETE` | `/api/teams/{teamId}/members/{memberId}` | حذف عضو |
| `PUT` | `/api/teams/{teamId}/members/{memberId}/role` | تغییر نقش عضو |

### دعوتنامه‌ها

| متد | مسیر | هدف |
|---|---|---|
| `GET` | `/api/invites` | فهرست دعوتنامه‌ها |
| `POST` | `/api/invites` | ارسال دعوتنامه |
| `DELETE` | `/api/invites` | لغو دعوتنامه |
| `POST` | `/api/invites/accept` | پذیرش دعوتنامه |
| `PUT` | `/api/invites/reject` | رد دعوتنامه |

### وظایف

| متد | مسیر | هدف |
|---|---|---|
| `GET` | `/api/tasks/for-me` | وظایف مسئول‌شده برای کاربر |
| `GET` | `/api/tasks/for-team` | وظایف یک تیم |
| `GET` | `/api/tasks/{id}` | جزئیات وظیفه |
| `POST` | `/api/tasks` | ایجاد وظیفه |
| `PUT` | `/api/tasks/details` | ویرایش جزئیات |
| `PUT` | `/api/tasks/deadline` | تغییر مهلت |
| `PUT` | `/api/tasks/assignee` | تغییر مسئول |
| `PUT` | `/api/tasks/creator` | تغییر محول‌کننده |
| `POST` | `/api/tasks/{taskId}/start` | شروع انجام وظیفه |
| `POST` | `/api/tasks/{taskId}/claim-complete` | اعلام تکمیل |
| `POST` | `/api/tasks/{taskId}/claim-failure` | اعلام شکست |
| `POST` | `/api/tasks/{taskId}/close-complete` | بستن با تکمیل |
| `POST` | `/api/tasks/{taskId}/close-failure` | بستن با شکست |
| `POST` | `/api/tasks/{taskId}/order-retry` | دستور تلاش مجدد |
| `POST` | `/api/tasks/{taskId}/accept-retry` | پذیرش تلاش مجدد |
| `POST` | `/api/tasks/{taskId}/reject-retry` | رد تلاش مجدد |

---

## تصمیمات معماری

| تصمیم | دلیل | پیامد |
|---|---|---|
| Clean Architecture با قانون وابستگی یک‌طرفه | قواعد کسب‌وکار مستقل از فریم‌ورک و پایگاه داده تست‌پذیر باشند | پروژه‌های بیشتر و افزودن قابلیت ساده، چند لایه را لمس می‌کند |
| تیم به‌عنوان اگریگیت ریشه | قواعد یکتایی مالک و لغو خودکار دعوت در یک مرز اجرا شوند | تیم شامل تاریخچه کامل اعضا است؛ سقفی برای تعداد اعضا وجود ندارد |
| وظیفه اگریگیت جداگانه | چرخه عمر وظیفه مستقل از تغییرات عضویت باشد | بررسی عضویت مسئول در لایه Application انجام می‌شود |
| ارجاع بین اگریگیت‌ها با شناسه | وابستگی مستقیم بین User، Team و TeamTask ایجاد نشود | برای نمایش نام کاربران باید خواندن جداگانه انجام شود |
| Specification در دامین و QuerySpecification در Persistence | قاعده انتخاب داده یک بار تعریف شود | هر Specification جدید باید در Persistence هم پیاده‌سازی شود |
| انتشار رویدادهای دامین در Persistence | دامین از جزئیات انتشار بی‌اطلاع باشد | پس از ذخیره موفق تراکنش منتشر می‌شوند؛ تحویل حداکثر یک‌بار است |
| نتیجه `Result<T>` به‌جای استثنا برای خطاهای دامین | خطاهای قابل پیش‌بینی بخشی از قرارداد باشند | هر خطای جدید باید در `DomainErrors` تعریف شود |
| احراز هویت دوکاناله | وب با Cookie و API با JWT، هر دو به یک هسته وصل شوند | مدیریت انقضا و ابطال توکن جداگانه طراحی می‌شود |
