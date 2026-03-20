# College LMS

Система управления обучением для колледжа. Замена устаревшего сайта (2007 г.) на современную LMS.

## Стек технологий

- **Backend**: .NET 8 (ASP.NET Core Web API)
- **Frontend**: Angular 17+ (standalone components, signals, Angular Material)
- **БД**: PostgreSQL + Entity Framework Core
- **Auth**: JWT + refresh tokens (BCrypt)

## Быстрый старт

### Запуск через Docker

```bash
docker-compose up
```

Это поднимет PostgreSQL и API. Админ-аккаунт создастся автоматически:
- Email: `admin@college.local`
- Пароль: `admin123`

### Запуск frontend

```bash
cd college-lms-client
npm install
ng serve
```

Приложение будет доступно на `http://localhost:4200`.

### Запуск backend вручную

```bash
cd src/CollegeLms.Api
dotnet run
```

API будет доступен на `http://localhost:5000`. Swagger UI: `http://localhost:5000/swagger`.

## Структура проекта

```
├── src/
│   ├── CollegeLms.Domain/          # Сущности, Enums, Интерфейсы
│   ├── CollegeLms.Infrastructure/  # EF Core, PostgreSQL, файловое хранилище
│   └── CollegeLms.Api/             # Controllers, Services, Auth, DTOs
├── college-lms-client/             # Angular 17+ приложение
├── tests/                          # Unit и integration тесты
├── docker-compose.yml              # PostgreSQL + API
└── Dockerfile                      # Multi-stage build
```

## Возможности

- Аутентификация и авторизация (Admin, Teacher, Student)
- Страницы преподавателей с дисциплинами
- Дисциплины с календарным планом и материалами
- Тестовая система с авто-оцениванием и настраиваемыми порогами
- Журнал (средняя без нулей) и рейтинг (средняя с нулями + % посещаемости)
- Расписание занятий по группам
- Объявления
- Админ-панель для управления пользователями, группами и дисциплинами
