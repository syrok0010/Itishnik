# ITishnik

Образовательная веб-платформа для поддержки изучения дисциплины "Алгоритмы и структуры данных" в НИУ ВШЭ.

---

## 🌟 О проекте

Алгоритмы и структуры данных — фундаментальный навык, проверяемый на собеседованиях в любой IT-компании. Однако существующие ресурсы для их изучения часто фрагментированы: есть книги с теорией, и есть тренажеры, сфокусированные исключительно на написании кода с жесткой автоматической проверкой. Это не всегда позволяет развить именно аналитическое мышление.

Платформа **ITishnik** предлагает иной подход. Мы смещаем акцент с безошибочной реализации на конкретном языке программирования на глубокое понимание и анализ. Наш сервис предоставляет студентам и преподавателям НИУ ВШЭ инструменты для эффективного обучения через задачи нестандартных форматов:

- Анализ результата выполнения кода;
- Рефакторинг и улучшение существующих решений;
- Поиск логических ошибок в предложенном алгоритме.

Ответы на задачи принимаются в гибком виде — это может быть псевдокод, текстовое пояснение или конечный результат. В связи с этим в проекте отсутствует традиционное автоматическое тестирование. Проверка решений осуществляется непосредственно преподавателем, который выставляет итоговую оценку.

Платформа построена на ролевой модели:
- **Студент** видит свои задания и дедлайны, решает аналитические задачи и может оставить комментарий-фидбэк по ним.
- **Преподаватель** управляет курсом, загружает задачи, проверяет решения студентов и может использовать генеративный ИИ как для помощи в составлении новых заданий, так и для содействия в ручной проверке ответов.

## 🛠️ Технологический стек

Проект построен с использованием современных и надежных технологий:

### Frontend
![Angular](https://img.shields.io/badge/Angular-DD0031?style=for-the-badge&logo=angular&logoColor=white)
![Taiga UI](https://img.shields.io/badge/Taiga%20UI-7756F2?style=for-the-badge&logo=data:image/svg+xml;base64,PHN2ZyB2aWV3Qm94PSIwIDAgMjQgMjQiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyI+PHBhdGggZmlsbD0id2hpdGUiIGQ9Ik0xMiAyQTEwIDEwIDAgMSAwIDEyIDIyQTEwIDEwIDAgMCAwIDEyIDJtMC0yYTEyIDEyIDAgMSAxIDAgMjRBMTIgMTIgMCAwIDEgMTIgMG01LjU1IDEyLjY0YTQgNCAwIDAgMC0uNDQtMS4zMWMtLjM3LS42OC0uODItMS4xMS0xLjQ3LTEuNTZhMy4zNiAzLjM2IDAgMCAxLTEuNTctLjY3Yy0uMzQtLjE2LS41LS4yOC0uNS0uNDIgMC0uMi4xMy0uMzQuMzgtLjM0YTItMiAwIDAgMSAxLjA3LjM2bC4zNC0xLjM0YTMuMjYgMy4yNiAwIDAgMC0xLjU4LS40NWMtMS4zOSAwLTIuMTkuNzMtMi4xOSAxLjc5IDAgLjUuMjUuODkuNzUgMS4yNi4zNS4yNy44LjUzIDEuMjMuNzZsLjQyLjIzYy41My4yOC44Ni41My44NiAxIDAgLjU3LS40MSAxLTIuMDYgMS0uNjkgMC0xLjM4LS4yMy0xLjg5LS42MWwtLjQzIDEuMzJjLjU4LjQ0IDEuNDIuNzIgMi4zOC43MiAxLjcyIDAgMi44OC0uODggMi44OC0yLjI0IDAtLjQ4LS4xNi0uOS0uNDUtMS4yN20tNy40NS0zLjA4aC0uOTVWOC4yMWgtMS44MXYxLjM1aC0uOTV2MS4xOWg0LjY2di0xLjE5WiIvPjwvc3ZnPg==)
![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-38B2AC?style=for-the-badge&logo=tailwind-css&logoColor=white)

### Backend
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Entity Framework Core](https://img.shields.io/badge/EF_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)

### Database & AI
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)
![Gemini API](https://img.shields.io/badge/Gemini_API-4285F4?style=for-the-badge&logo=google-gemini&logoColor=white)

---

## 📂 Структура проекта

Проект имеет интегрированную структуру, типичную для ASP.NET Core приложений с SPA-фронтендом.

```
/
├── src/                  # Корневая папка с исходным кодом всего решения
│   └── Web/              # Основной проект ASP.NET Core (Backend)
│       └── ClientApp/    # Исходный код Frontend на Angular
└── README.md
```

---

## 🚀 Начало работы

Это руководство поможет вам настроить и запустить проект локально.

### ✅ Пререквизиты

Перед началом убедитесь, что у вас установлено следующее программное обеспечение:

*   [.NET SDK 9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
*   [Node.js 20.x (включая npm)](https://nodejs.org/en/)
*   [PostgreSQL](https://www.postgresql.org/download/) (СУБД должна быть установлена и запущена)
*   **API-ключ для Gemini** (его можно получить в [Google AI Studio](https://aistudio.google.com/))

### ⚙️ Установка и запуск

#### 1. Настройка Backend и User Secrets

Для безопасного хранения чувствительных данных (ключи API, пароли, строки подключения) мы используем механизм **User Secrets**. Это предотвращает попадание секретов в систему контроля версий.

1.  Перейдите в директорию веб-проекта:
    ```bash
    cd src/Web
    ```

2.  Измените файл `appsettings.Development.json` и скопируйте в него следующую конфигурацию. Она содержит нечувствительные параметры для окружения разработки.
    ```json
    {
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft": "Warning",
          "Microsoft.Hosting.Lifetime": "Information",
          "Microsoft.EntityFrameworkCore.Database.Command": "Information"
        }
      },
      "AllowedHosts": "*",
      "GeminiProxyAddress": "http://127.0.0.1:1080",
      "EmailSettings": {
        "SmtpServer": "smtp.yandex.ru",
        "Port": 465,
        "FromName": "Твой портал \"Айтишник\"",
        "FromEmail": "tvoy.itishnik@yandex.ru",
        "Username": "tvoy.itishnik"
      }
    }
    ```
    > **Примечание:** Если вы не используете прокси для доступа к Gemini API, удалите строку `GeminiProxyAddress`.
    > Измените `EmailSettings` по необходимости

3.  Инициализируйте user-secrets и установите ваши секретные значения. **Выполняйте эти команды, находясь в папке `src/Web`**.
    ```bash
    # Строка подключения к вашей локальной базе данных PostgreSQL
    dotnet user-secrets set "ConnectionStrings:ItishnikDb" "Host=localhost;Port=5432;Database=itishnik_db;Username=postgres;Password=your_password"

    # Ваш API-ключ от Google Gemini
    dotnet user-secrets set "GoogleAi:ApiKey" "YOUR_GEMINI_API_KEY_HERE"

    # Пароль от почтового ящика, указанного в EmailSettings
    dotnet user-secrets set "EmailSettings:Password" "YOUR_EMAIL_APP_PASSWORD_HERE"
    ```

4.  Примените миграции к базе данных. Эта команда создаст все необходимые таблицы.
    ```bash
    dotnet ef database update
    ```

#### 2. Запуск Frontend

1.  Откройте **новый терминал** и перейдите в директорию клиентского приложения:
    ```bash
    cd src/Web/ClientApp
    ```
2.  Установите все необходимые зависимости:
    ```bash
    npm install
    ```

#### 3. Запуск Backend

1.  Вернитесь в **первый терминал** (где вы находитесь в `src/Web`).
2.  Запустите серверную часть:
    ```bash
    dotnet run
    ```
    Будет запущен Backend и Frontend на `https://localhost:44447`.

Теперь у вас полностью развернута локальная среда для работы над проектом!

---

## 👥 Команда проекта

*   **Вадим Сыров** - Team Lead, Full-stack Developer - [GitHub](https://github.com/syrok0010)
*   **Владислав Баранов** - Backend Developer - [GitHub](https://github.com/VladZF)
*   **Валерия Лапшина** - Frontend Developer - [GitHub](https://github.com/valeriii25)
