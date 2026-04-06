using System.Text.Json.Serialization;

namespace PLDMS.BL.DTOs;

public record Judge0RequestDTO
{
    [JsonPropertyName("language_id")]
    public int LanguageId { get; set; }

    [JsonPropertyName("source_code")]
    public string SourceCode { get; set; }

    [JsonPropertyName("stdin")]
    public string? Stdin { get; set; }
}