using Decrypt.Logic.Dtos;
using FluentResults;

namespace Decrypt.Logic.Abstractions;

/// <summary>
/// Abstraction of the service to combine data for dashboard
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// Returns projection for dashboard 
    /// </summary>
    Task<Result<DashboardDto>> GetDashboardAsync();
}