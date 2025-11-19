using backend.Domain.Models;

namespace backend.Repositories;

public interface ISubmissionRepository
{
    Task<FormSubmission> AddAsync(FormSubmission submission);
    Task<FormSubmission?> GetByIdAsync(Guid id);
    Task<IEnumerable<FormSubmission>> SearchAsync(string? formKey = null, string? search = null, int page = 1, int pageSize = 50);
}

