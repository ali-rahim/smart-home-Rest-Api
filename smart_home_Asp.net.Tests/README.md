# Smart Home API — Test Suite

## ساختار

```
smart_home_Asp.net.Tests/
├── Helpers/
│   └── CustomWebApplicationFactory.cs   # WebApplicationFactory ایزوله
├── Unit/
│   ├── Domain/
│   │   ├── EntityTests.cs               # Home, Room, CompositeEntity
│   │   └── DeviceTests.cs               # Light, Fan, Sensors, Alarm
│   ├── Services/
│   │   ├── DeviceFactoryTests.cs
│   │   ├── DeviceManagerTests.cs
│   │   ├── RoomManagerTests.cs
│   │   └── HomeServiceTests.cs
│   └── Middleware/
│       └── ExceptionHandlingMiddlewareTests.cs
└── Integration/
    ├── RoomsApiTests.cs                 # /rooms, /room/{id}
    ├── DevicesApiTests.cs               # /devices, /device/{id}, Turn_on_off, sensor_value
    ├── RoomDevicesApiTests.cs           # /rooms/{id}/devices/...
    ├── ConfigApiTests.cs                # /config
    └── FullWorkflowTests.cs             # سناریوی end-to-end کامل
```

## پوشش تست

| لایه | چه چیزهایی تست شده |
|------|---------------------|
| Domain | ساخت Entity، قوانین parent/child، duplicate، not-found، همه Device typeها و capabilityها |
| DeviceFactory | همه enumها + نوع نامعتبر |
| DeviceManager | CRUD، capability filter، toggle، sensor value، خطاها |
| RoomManager | CRUD اتاق، attach/detach دستگاه، filter داخل اتاق |
| HomeService | orchestration کامل + rollback وقتی اتاق وجود ندارد |
| Middleware | 404 / 409 / 400 / 500 mapping |
| API | تمام endpointها + capability query + workflow کامل |

## اجرا

از ریشه solution یا پوشه تست:

```bash
dotnet test smart_home_Asp.net.Tests
```

با جزئیات بیشتر:

```bash
dotnet test smart_home_Asp.net.Tests --logger "console;verbosity=detailed"
```

فقط Unit:

```bash
dotnet test --filter "FullyQualifiedName~Unit"
```

فقط Integration:

```bash
dotnet test --filter "FullyQualifiedName~Integration"
```

## نکته مهم درباره Exception Middleware

تست‌های `ExceptionHandlingMiddlewareTests` و بخشی از Integration که انتظار **404 / 409** دارند،
به نسخه **اصلاح‌شده** Middleware وابسته‌اند.

اگر هنوز Middleware قدیمی (همیشه ۵۰۰) داری:
1. اول کد اصلاح‌شده‌ای که قبلاً دادم را جایگزین کن
2. بعد `dotnet test` بزن

تست‌های Integration برای وضعیت‌های خطا از `BeOneOf(NotFound, InternalServerError)` استفاده می‌کنند
تا هم با Middleware قدیم و هم جدید کار کنند؛ ولی تست Unit Middleware سخت‌گیرانه است.
