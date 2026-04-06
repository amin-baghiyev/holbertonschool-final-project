using PLDMS.BL.DTOs.DashboardDTOs;

namespace PLDMS.BL.Services.Abstractions;

public interface IDashboardService
{
    Task<AdminDashboardStatsDTO> AdminDashboardStatsAsync();
}