namespace PLDMS.BL.DTOs;

public record Judge0ResponseDTO
{
    public string? stdout { get; set; }
    public string? stderr { get; set; }
    public string? compile_output { get; set; }
    public Judge0StatusDTO? status { get; set; }
}

public record Judge0StatusDTO
{
    public int id { get; set; }
    public string description { get; set; } = null!;
}
