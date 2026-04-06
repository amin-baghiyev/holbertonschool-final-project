namespace PLDMS.BL.DTOs;

public record CodeSubmissionResultDTO
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public int TotalTests { get; set; }
    public int PassedTests { get; set; }
    public List<TestCaseResultDTO> TestResults { get; set; } = new();
}

public record TestCaseResultDTO
{
    public int TestCaseIndex { get; set; }
    public bool Passed { get; set; }
    public bool IsExample { get; set; }
    public string? ExpectedOutput { get; set; }
    public string? ActualOutput { get; set; }
    public string? ErrorMessage { get; set; }
}
