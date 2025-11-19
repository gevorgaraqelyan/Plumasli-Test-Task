using backend.Domain.Models;
using backend.Repositories;

namespace backend.Services;

public class SubmissionService : ISubmissionService
{
    private readonly ISubmissionRepository _repository;

    public SubmissionService(ISubmissionRepository repository)
    {
        _repository = repository;
    }

    public async Task<FormSubmission> CreateSubmissionAsync(string formKey, string data)
    {
        var submission = new FormSubmission
        {
            Id = Guid.NewGuid(),
            FormKey = formKey,
            CreatedAt = DateTime.UtcNow,
            Data = data
        };

        return await _repository.AddAsync(submission);
    }

    public async Task<FormSubmission?> GetSubmissionByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<FormSubmission>> SearchSubmissionsAsync(string? formKey = null, string? search = null, int page = 1, int pageSize = 50)
    {
        return await _repository.SearchAsync(formKey, search, page, pageSize);
    }
}

