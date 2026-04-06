using PLDMS.BL.Common;
using PLDMS.BL.DTOs;
using PLDMS.BL.Services.Abstractions;
using PLDMS.Core.Enums;
using System.Net.Http.Json;

namespace PLDMS.BL.Services.Concretes;

public class JudgeService : IJudgeService
{
    private readonly HttpClient _httpClient;

    public JudgeService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Judge0ResponseDTO> ExecuteCodeAsync(ProgrammingLanguage language, string sourceCode, string? stdin = null)
    {
        var requestDto = new Judge0RequestDTO
        {
            LanguageId = (int)language,
            SourceCode = sourceCode,
            Stdin = stdin
        };

        string jsonPayload = System.Text.Json.JsonSerializer.Serialize(requestDto);

        using var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:2358/submissions?base64_encoded=false&wait=true");
        request.Content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");
        request.Headers.Add("X-Auth-User", "mysecretkey123");

        using var response = await _httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new BaseException($"Execution failed with status: {response.StatusCode}");
        }

        var result = await response.Content.ReadFromJsonAsync<Judge0ResponseDTO>();
        return result ?? throw new BaseException("Failed to decode response from Judge0.");
    }
}