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

### Локальный запуск без Docker

Если Docker недоступен, можно запустить проект с локальным PostgreSQL.

1. Установите PostgreSQL и создайте БД:

```sql
CREATE USER college_lms_user WITH PASSWORD 'college_lms_pass';
CREATE DATABASE college_lms OWNER college_lms_user;
```

2. Запустите backend:

```bash
cd src/CollegeLms.Api
dotnet run
```

Либо задайте connection string через переменную окружения:

```bash
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=college_lms;Username=college_lms_user;Password=college_lms_pass"
dotnet run --project src/CollegeLms.Api
```

API будет доступен на `http://localhost:5000`. Swagger UI: `http://localhost:5000/swagger`.

При первом запуске автоматически применяются миграции и создаётся администратор.

**Возможные проблемы с Docker DNS:**
Если `docker compose up` не может скачать образы — проверьте DNS. Добавьте в `/etc/docker/daemon.json`:
```json
{ "dns": ["8.8.8.8", "8.8.4.4"] }
```
Затем перезапустите Docker: `sudo systemctl restart docker`.

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
