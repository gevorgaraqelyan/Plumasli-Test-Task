using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Domain.Models;

namespace backend.Repositories;

public class SubmissionRepository : ISubmissionRepository
{
    private readonly ApplicationDbContext _context;

    public SubmissionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<FormSubmission> AddAsync(FormSubmission submission)
    {
        _context.FormSubmissions.Add(submission);
        await _context.SaveChangesAsync();
        return submission;
    }

    public async Task<FormSubmission?> GetByIdAsync(Guid id)
    {
        return await _context.FormSubmissions.FindAsync(id);
    }

    public async Task<IEnumerable<FormSubmission>> SearchAsync(string? formKey = null, string? search = null, int page = 1, int pageSize = 50)
    {
        var query = _context.FormSubmissions.AsQueryable();

        if (!string.IsNullOrWhiteSpace(formKey))
        {
            query = query.Where(s => s.FormKey == formKey);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(s => 
                s.Data.ToLower().Contains(searchLower) || 
                s.FormKey.ToLower().Contains(searchLower));
        }

        query = query.OrderByDescending(s => s.CreatedAt);

        var skip = (page - 1) * pageSize;
        return await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();
    }
}

