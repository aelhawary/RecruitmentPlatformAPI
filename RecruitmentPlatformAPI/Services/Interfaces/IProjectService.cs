using RecruitmentPlatformAPI.DTOs;
using RecruitmentPlatformAPI.DTOs.Auth;
using RecruitmentPlatformAPI.DTOs.Profile;
using RecruitmentPlatformAPI.DTOs.Reference;
using RecruitmentPlatformAPI.Models.Core;
using RecruitmentPlatformAPI.Models.Reference;
using RecruitmentPlatformAPI.Models.Authentication;
using RecruitmentPlatformAPI.Models.Assessment;
using RecruitmentPlatformAPI.Services;
using RecruitmentPlatformAPI.Services.Interfaces;

namespace RecruitmentPlatformAPI.Services.Interfaces
{
    public interface IProjectService
    {
        /// <summary>
        /// Add a new project for the authenticated job seeker
        /// </summary>
        Task<ProjectResponseDto> AddProjectAsync(int userId, AddProjectDto dto);

        /// <summary>
        /// Update an existing project
        /// </summary>
        Task<ProjectResponseDto> UpdateProjectAsync(int userId, int projectId, UpdateProjectDto dto);

        /// <summary>
        /// Soft delete a project and reorder remaining projects
        /// </summary>
        Task<ApiResponse<bool>> DeleteProjectAsync(int userId, int projectId);

        /// <summary>
        /// Get all active projects for the authenticated job seeker
        /// </summary>
        Task<List<ProjectDto>> GetProjectsAsync(int userId);
    }
}
