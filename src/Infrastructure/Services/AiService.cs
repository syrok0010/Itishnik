using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Models;
using Microsoft.Extensions.Configuration;

namespace Itishnik.Infrastructure.Services;

public class AiService(HttpClient httpClient, IConfiguration configuration) : IAiService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _configuration = configuration;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<AiVerdictResponse> EvaluateSolutionAsync(int maxScore, string taskText, string solutionText, string referenceText)
    {
        const string modelName = "gemini-2.0-flash";
        var requestUrl = $"/v1beta/models/{modelName}:generateContent?key={_configuration["GoogleAi:ApiKey"]}";
        var prompt = GenerateGradePrompt(maxScore, taskText, solutionText, referenceText);
        var requestPayload = new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } },
            generationConfig = new
            {
                responseMimeType = "application/json",
                temperature = 0.1,
                maxOutputTokens = 50
            }
        };

        var response = await _httpClient.PostAsJsonAsync(requestUrl, requestPayload);
        response.EnsureSuccessStatusCode();
        JsonNode? root = await response.Content.ReadFromJsonAsync<JsonNode>();
        
        var innerJsonString = root?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.GetValue<string>();

        if (string.IsNullOrWhiteSpace(innerJsonString))
            throw new FormatException("Ошибка: Gemini API вернул пустой или некорректный ответ.");

        var verdict = JsonSerializer.Deserialize<AiVerdictResponse>(innerJsonString, JsonOptions);
    
        if (verdict is null)
            throw new NullReferenceException("Вердикт пуст после десериализации вложенного JSON.");

        return verdict;
    }

    public async Task<AiGeneratedTaskResponse> GenerateTaskAsync(string taskText, List<string> tags, string additionalInformation)
    {
        const string modelName = "gemini-2.0-flash";
        var requestUrl = $"/v1beta/models/{modelName}:generateContent?key={_configuration["GoogleAi:ApiKey"]}";
        var prompt = GenerateTaskPrompt(taskText, tags, additionalInformation);
        var requestPayload = new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } },
            generationConfig = new
            {
                responseMimeType = "application/json",
                temperature = 0.3,
                maxOutputTokens = 8192
            }
        };

        var response = await _httpClient.PostAsJsonAsync(requestUrl, requestPayload);
        response.EnsureSuccessStatusCode();
        JsonNode? root = await response.Content.ReadFromJsonAsync<JsonNode>();
        
        var innerJsonString = root?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.GetValue<string>();

        if (string.IsNullOrWhiteSpace(innerJsonString))
            throw new FormatException("Ошибка: Gemini API вернул пустой или некорректный ответ.");
        
        var generatedTask = JsonSerializer.Deserialize<AiGeneratedTaskResponse>(innerJsonString, JsonOptions);
    
        if (generatedTask is null)
            throw new NullReferenceException("Сгенерированная задача пуста после десериализации.");

        return generatedTask;
    }

    private string GenerateTaskPrompt(string taskText, List<string> tags, string additionalInformation)
    {
        var tagsString = tags.Count != 0 ? string.Join(", ", tags) : "Нет";
        
        return $$"""
            Роль: Ты — эксперт в области составления учебных задач по курсу "Алгоритмы и структуры данных". Твоя задача — сгенерировать новую задачу на основе примера и вернуть ее в строго заданном JSON формате.

            Твоя задача: На основе предоставленной задачи-примера и ключевых особенностей, сформулированных преподавателем, создай НОВУЮ, уникальную задачу.

            Входные данные от преподавателя для генерации:

            *   [ИСХОДНАЯ ЗАДАЧА]
                {{taskText}}

            *   [КЛЮЧЕВЫЕ ОСОБЕННОСТИ НОВОЙ ЗАДАЧИ]
                {{additionalInformation}}

            *   [ЖЕЛАТЕЛЬНЫЕ ТЭГИ ДЛЯ КОНТЕКСТА]
                {{tagsString}}

            ИНСТРУКЦИЯ ПО ГЕНЕРАЦИИ И ФОРМАТИРОВАНИЮ:

            1.  **Проанализируй** исходную задачу, чтобы понять ее суть, основной алгоритм и сложность.
            2.  **Придумай** совершенно новый сюжет и контекст для задачи.
            3.  **Скомпонуй** весь текст задачи в одно поле `text`. Используй Markdown для форматирования (заголовки `##`, списки `-`, блоки кода ```). Поле `text` должно содержать:
                *   Легенду (сюжет).
                *   Формальную постановку условия.
                *   Описание формата входных данных.
                *   Описание формата выходных данных.
                *   Ограничения.
                *   Как минимум один наглядный пример с вводом и выводом.
            4.  **Скомпонуй** полное решение в одно поле `solution`. Это поле должно включать:
                *   Описание алгоритмической идеи.
                *   Анализ временной и пространственной сложности.
                *   Псевдокод или готовый код на C++/Python.
            5.  **Упакуй** результат в ОДИН валидный JSON-объект. Не добавляй никакого текста, комментариев или объяснений до или после JSON.

            СТРУКТУРА ВЫХОДНОГО JSON ДОЛЖНА БЫТЬ СТРОГО СЛЕДУЮЩЕЙ:
            ```json
            {
              "name": "Название новой задачи",
              "text": "## Легенда\nТекст легенды...\n\n## Условие\nТекст условия...\n\n## Формат ввода\nОписание...\n\n## Формат вывода\nОписание...\n\n## Ограничения\n- 1 <= N <= 100000\n\n## Пример\n**Ввод:**\n```\n5\n1 2 3 4 5\n```\n**Вывод:**\n```\nYES\n```",
              "solution": "## Идея решения\nОсновная идея заключается в использовании...\n\n## Сложность\n- Временная: O(N log N)\n- Пространственная: O(1)\n\n## Псевдокод\n```\nfunction solve(data):\n  // ... логика решения\n  return result\n```"
            }
            ```
            """;
    }

    private string GenerateGradePrompt(int maxScore, string taskText, string solutionText, string referenceText)
    {
        return $$"""
               Ты — эксперт-ассистент по оценке заданий для курса "Алгоритмы и структуры данных". Твоя задача — проанализировать решение студента, сравнить его с эталонным решением и выставить оценку по заданным критериям.

               **Максимальный балл за задачу:** 
               {{maxScore}}
               
               **Решение студента:**
               {{solutionText}}
               
               **Текст задачи:**
               {{taskText}}
               
               **Эталонное решение:**
               {{referenceText}}
               
               ТРЕБОВАНИЯ К ОТВЕТУ:
               Верни ответ СТРОГО в формате JSON. Не добавляй никаких пояснений или текста вне JSON.
               JSON должен содержать ТОЛЬКО одно поле "score" с числовым значением от 0 до {maxScore}.
               
               Пример правильного ответа:
               8
               """;
    }
}
