using backend.Domain.Models;

namespace backend.Services;

public interface ISubmissionService
{
    Task<FormSubmission> CreateSubmissionAsync(string formKey, string data);
    Task<FormSubmission?> GetSubmissionByIdAsync(Guid id);
    Task<IEnumerable<FormSubmission>> SearchSubmissionsAsync(string? formKey = null, string? search = null, int page = 1, int pageSize = 50);
}

