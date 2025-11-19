using Microsoft.EntityFrameworkCore;
using backend.Domain.Models;

namespace backend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<FormSubmission> FormSubmissions { get; set; }
    public DbSet<FormDefinition> FormDefinitions { get; set; }
}

